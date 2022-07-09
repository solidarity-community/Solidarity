import { component, property, nothing, state, html, FormatHelper } from '@3mo/model'
import { Campaign, CampaignService } from 'sdk'
import { DialogVote, Progress, TimerController } from 'application'

@component('solid-campaign-validation-progress')
export class CampaignValidationProgress extends Progress {
	private static readonly votesFetchInterval = 10_000

	private fetchVotes = async () => {
		if (this.campaign.id) {
			this.votes = await CampaignService.getVotes(this.campaign.id)
		}
	}

	protected _ = new TimerController(this, CampaignValidationProgress.votesFetchInterval, this.fetchVotes)

	@property({ type: Boolean, reflect: true }) alwaysShowApprovalThreshold = false
	@property({ type: Object, updated(this: CampaignValidationProgress) { this.fetchVotes() } }) campaign!: Campaign

	@state() private votes: Awaited<ReturnType<typeof CampaignService.getVotes>> = { balance: 0, endorsedBalance: 0, approvalThreshold: 0 }


	protected override connected() {
		DialogVote.voteCast.subscribe(this.fetchVotes)
	}

	protected override disconnected() {
		DialogVote.voteCast.unsubscribe(this.fetchVotes)
	}

	protected get heading() { return 'Validation' }
	protected get progress() { return this.votes.endorsedBalance / this.votes.balance }
	protected get value() {
		return html`
			<mo-flex direction='horizontal' gap='4px' foreground='var(--mo-color-gray)' alignItems='baseline'>
				<mo-flex direction='horizontal' gap='3px' alignItems='baseline'>
					<solid-amount foreground='var(--mo-color-foreground)' value=${this.votes.balance}></solid-amount>
					<mo-div fontSize='var(--mo-font-size-s)'>Raised</mo-div>
				</mo-flex>
				<mo-div>●</mo-div>
				<mo-div>
					<mo-div foreground='var(--mo-color-foreground)'>${FormatHelper.percent(this.votes.endorsedBalance / this.votes.balance * 100)}%</mo-div>
					<mo-div fontSize='var(--mo-font-size-s)'>Endorsed</mo-div>
				</mo-div>
			</mo-flex>
		`
	}

	protected override get template() {
		return !this.campaign ? nothing : html`
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
			<mo-flex position='relative'>
				${super.progressBarTemplate}
				<span>
					<mo-flex direction='horizontal' gap='6px' alignItems='center'>
						<mo-div fontSize='var(--mo-font-size-s)' foreground='var(--mo-color-gray)'>Approval</mo-div>
						<mo-div>${FormatHelper.percent(this.votes.approvalThreshold * 100)}%</mo-div>
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