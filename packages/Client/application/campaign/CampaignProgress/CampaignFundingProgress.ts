import { component, property, nothing, state, html } from '@3mo/model'
import { Campaign, CampaignService } from 'sdk'
import { Progress, TimerController } from 'application'

@component('solid-campaign-funding-progress')
export class CampaignFundingProgress extends Progress {
	private static readonly balanceFetchInterval = 10_000

	protected _ = new TimerController(this, CampaignFundingProgress.balanceFetchInterval, this.fetchBalance.bind(this))

	@property({ type: Object, updated(this: CampaignFundingProgress) { this.fetchBalance() } }) campaign!: Campaign

	@state() private balance = 0

	private async fetchBalance() {
		if (this.campaign.id) {
			this.balance = await CampaignService.getBalance(this.campaign.id)
		}
	}

	protected get heading() { return 'Funds Raised' }
	protected get progress() { return this.balance / this.campaign.totalExpenditure }
	protected get value() {
		return html`
			<mo-flex direction='horizontal' gap='2px' foreground='var(--mo-color-gray)'>
				<solid-amount foreground='var(--mo-color-foreground)' value=${this.balance}></solid-amount>
				/
				<solid-amount value=${this.campaign.totalExpenditure}></solid-amount>
			</mo-flex>
		`
	}

	protected override get template() {
		return !this.campaign ? nothing : super.template
	}
}

declare global {
	interface HTMLElementTagNameMap {
		'solid-campaign-funding-progress': CampaignFundingProgress
	}
}