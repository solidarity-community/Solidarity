import { component, css, html, event, style } from '@a11d/lit'
import { DialogComponent, NotificationComponent } from '@a11d/lit-application'
import { Task } from '@lit/task'
import { Campaign } from 'application'

@component('solid-dialog-campaign-validation-vote')
export class DialogCampaignValidationVote extends DialogComponent<{ readonly campaign: Campaign }> {
	@event() static readonly voteCast: EventDispatcher

	readonly balanceFetchTask = new Task(this, () => Campaign.getBalance(this.parameters.campaign.id!), () => [])
	get balance() { return this.balanceFetchTask.value }
	private readonly voteFetchTask = new Task(this, () => Campaign.getVote(this.parameters.campaign.id!), () => [])
	get vote() { return this.voteFetchTask.value }


	override async confirm(...parameters: Parameters<DialogComponent<{ readonly campaign: Campaign }>['confirm']>) {
		const share = await Campaign.getShare(this.parameters.campaign.id!)
		if (!share) {
			NotificationComponent.notifyAndThrowError('You didn\'t donate to this campaign.')
		}
		return super.confirm(...parameters)
	}

	static override get styles() {
		return css`
			mo-dialog {
				--mo-dialog-width: min-content;
			}

			mo-loading-button[data-pre-disabled] {
				--mo-button-color-accent: var(--mo-color-gray);
			}
		`
	}

	protected override get template() {
		return html`
			<mo-dialog heading='Vote'>
				<mo-flex gap='16px'>
					<mo-flex>
						<div>Endorse or oppose the integrity of "${this.parameters.campaign.title}".</div>
						<div>
							Voting ends
							<solid-timer .end=${this.parameters.campaign.validation?.expiration}
								${style({ color: 'var(--mo-color-accent)', fontWeight: 'bold' })}
							></solid-timer>
							and your vote weighs
							<span ${style({ color: 'var(--mo-color-accent)', fontWeight: 'bold' })}>${(this.balance! / this.balance! * 100).formatAsPercent()}</span>
							towards the integrity of the campaign.
						</div>
					</mo-flex>

					<mo-flex direction='horizontal' gap='20px'>
						${this.getVoteButtonTemplate('Endorse', true)}
						${this.getVoteButtonTemplate('Oppose', false)}
					</mo-flex>
				</mo-flex>
			</mo-dialog>
		`
	}

	private getVoteButtonTemplate(text: string, vote: boolean) {
		return html`
			<mo-loading-button type='outlined'
				${style({ flex: '1fr', height: '100px' })}
				?data-pre-disabled=${this.vote !== undefined && this.vote !== vote}
				@click=${() => this.handleVoteButtonClick(vote)}
			>
				${this.vote !== vote ? html.nothing : html`
					<mo-icon-button icon='verified' ${style({ position: 'absolute', left: '-4px', top: '-4px', fontSize: '28px' })}></mo-icon-button>
				`}
				<mo-flex>
					<div ${style({ fontSize: '24px' })}>${text}</div>
					<div ${style({ color: 'var(--mo-color-gray)', textTransform: 'initial' })}>the integrity of this campaign</div>
				</mo-flex>
			</mo-loading-button>
		`
	}

	private async handleVoteButtonClick(vote: boolean) {
		await Campaign.vote(this.parameters.campaign.id!, vote)
		DialogCampaignValidationVote.voteCast.dispatch()
		await this.voteFetchTask.run()
	}
}