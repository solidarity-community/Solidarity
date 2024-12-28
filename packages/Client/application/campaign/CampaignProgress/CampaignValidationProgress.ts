import { component, property, state, html, style } from '@a11d/lit'
import { IntervalController } from '@3mo/interval-controller'
import { Campaign, DialogCampaignValidationVote, Progress } from 'application'

@component('solid-campaign-validation-progress')
export class CampaignValidationProgress extends Progress {
	private static readonly votesFetchInterval = 10_000

	private fetchVotes = async () => {
		if (this.campaign.id) {
			this.votes = await Campaign.getVotes(this.campaign.id)
		}
	}

	protected _ = new IntervalController(this, CampaignValidationProgress.votesFetchInterval, this.fetchVotes)

	@property({ type: Boolean, reflect: true }) alwaysShowApprovalThreshold = false
	@property({ type: Object, updated(this: CampaignValidationProgress) { this.fetchVotes() } }) campaign!: Campaign

	@state() private votes: Awaited<ReturnType<typeof Campaign.getVotes>> = { balance: 0, endorsedBalance: 0, approvalThreshold: 0 }


	protected override connected() {
		DialogCampaignValidationVote.voteCast.subscribe(this.fetchVotes)
	}

	protected override disconnected() {
		DialogCampaignValidationVote.voteCast.unsubscribe(this.fetchVotes)
	}

	protected get heading() { return 'Validation' }
	protected get progress() { return this.votes.endorsedBalance / this.votes.balance }
	protected get value() {
		return html`
			<mo-flex direction='horizontal' gap='4px' alignItems='baseline' ${style({ color: 'var(--mo-color-gray)' })}>
				<mo-flex direction='horizontal' gap='3px' alignItems='baseline'>
					<solid-amount ${style({ color: 'var(--mo-color-foreground)' })} value=${this.votes.balance}></solid-amount>
					<div ${style({ fontSize: 'small' })}>Raised</div>
				</mo-flex>
				<div>●</div>
				<div>
					<div ${style({ color: 'var(--mo-color-foreground)' })}>${(this.votes.endorsedBalance / this.votes.balance).formatAsPercent()}</div>
					<div ${style({ fontSize: 'small' })}>Endorsed</div>
				</div>
			</mo-flex>
		`
	}

	protected override get template() {
		return !this.campaign ? html.nothing : html`
			<style>
				mo-linear-progress {
					position: relative;
				}

				mo-linear-progress::after {
					content: ' ';
					position: absolute;
					height: calc(100% + 4px);
					width: 4px;
					background: var(--mo-color-foreground);
					left: ${this.votes.approvalThreshold * 100}%;
					top: -2px;
					bottom: -2px;
				}

				:host(:hover) span {
					display: inline;
				}

				:host(:not([alwaysShowApprovalThreshold])) span {
					display: none;
				}

				span {
					position: absolute;
					left: ${this.votes.approvalThreshold * 100}%;
					top: 8px;
					transform: translateX(-50%);
				}
			</style>
			${super.template}
		`
	}

	protected override get progressBarTemplate() {
		return html`
			<mo-flex ${style({ position: 'relative' })}>
				${super.progressBarTemplate}
				<span>
					<mo-flex direction='horizontal' gap='6px' alignItems='center'>
						<div ${style({ fontSize: 'small', color: 'var(--mo-color-gray)' })}>Approval</div>
						<div>${this.votes.approvalThreshold.formatAsPercent()}</div>
					</mo-flex>
				</span>
			</mo-flex>
		`
	}
}

declare global {
	interface HTMLElementTagNameMap {
		'solid-campaign-validation-progress': CampaignValidationProgress
	}
}