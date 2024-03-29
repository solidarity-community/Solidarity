import { component, css, DialogComponent, FormatHelper, html, NotificationHost, event, nothing, state } from '@3mo/model'
import { Campaign, CampaignService } from 'sdk'

@component('solid-dialog-campaign-validation-vote')
export class DialogCampaignValidationVote extends DialogComponent<{ readonly campaign: Campaign }> {
	@event() static readonly voteCast: EventDispatcher

	@state() private share?: number
	@state() private balance?: number
	@state() private vote?: boolean

	private async fetchShare() {
		this.share = await CampaignService.getShare(this.parameters.campaign.id!)
	}

	private async fetchBalance() {
		this.balance = await CampaignService.getBalance(this.parameters.campaign.id!)
	}

	private async fetchVote() {
		this.vote = await CampaignService.getVote(this.parameters.campaign.id!)
	}

	override async confirm(...parameters: Parameters<DialogComponent<{ readonly campaign: Campaign }>['confirm']>) {
		await Promise.all([ this.fetchShare(), this.fetchBalance() ])

		if (this.share === 0) {
			NotificationHost.instance.notifyAndThrowError('You didn\'t donate to this campaign.')
		}
	
		return await super.confirm(...parameters)
	}

	static override get styles() {
		return css`
			mo-loading-button[data-pre-disabled] {
				--mdc-theme-primary: var(--mo-color-gray);
			}
		`
	}

	protected override get template() {
		return html`
			<mo-dialog heading='Vote'>
				<mo-flex gap='16px'>
					<mo-flex>
						<mo-div>Endorse or oppose the integrity of "${this.parameters.campaign.title}".</mo-div>
						<mo-div>
							Voting ends
							<solid-timer foreground='var(--mo-accent)' fontWeight='bold' .end=${this.parameters.campaign.validation?.expiration}></solid-timer>
							and your vote weighs
							<mo-div foreground='var(--mo-accent)' fontWeight='bold'>${FormatHelper.percent(this.share! / this.balance! * 100)}%</mo-div>
							towards the integrity of the campaign.
						</mo-div>
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
			<mo-loading-button type='outlined' width='*' height='100px'
				?data-pre-disabled=${this.vote !== undefined && this.vote !== vote}
				@click=${() => this.handleVoteButtonClick(vote)}
			>
				${this.vote !== vote ? nothing : html`
					<mo-icon-button position='absolute' icon='verified' left='-4px' top='-4px' fontSize='28px'></mo-icon-button>
				`}
				<mo-flex>
					<mo-div fontSize='24px'>${text}</mo-div>
					<mo-div foreground='var(--mo-color-gray)' style='text-transform: initial'>the integrity of this campaign</mo-div>
				</mo-flex>
			</mo-loading-button>
		`
	}

	private async handleVoteButtonClick(vote: boolean) {
		await CampaignService.vote(this.parameters.campaign.id!, vote)
		DialogCampaignValidationVote.voteCast.dispatch()
		await this.fetchVote()
	}
}