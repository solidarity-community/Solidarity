import { component, property, nothing, state, event, updated } from '@3mo/model'
import { Campaign, CampaignService } from 'sdk'
import { Progress } from 'application'

/** @fires balanceChange */
@component('solid-donation-progress')
export class DonationProgress extends Progress {
	private static readonly balanceFetchInterval = 10_000

	@event() readonly balanceChange!: EventDispatcher<number>

	@property({
		type: Object,
		async updated(this: DonationProgress) {
			if (this.campaign.id) {
				this.balance = await CampaignService.getBalance(this.campaign.id)
				this.balanceChange.dispatch(this.balance)
			}
		}
	}) campaign!: Campaign

	@state({ updated(this: DonationProgress) { this.balanceChange.dispatch(this.balance) } }) private balance = 0

	private timerId?: number

	protected override connected() {
		this.timerId = window.setInterval(() => this.fetchBalance(), DonationProgress.balanceFetchInterval)
	}

	protected override disconnected() {
		window.clearInterval(this.timerId)
	}

	private async fetchBalance() {
		if (this.campaign.id) {
			this.balance = await CampaignService.getBalance(this.campaign.id)
		}
	}

	protected get progress() { return this.balance / this.campaign.totalExpenditure }
	protected get progressTemplate() { return `${this.progress * 100} %` }

	protected override get template() {
		return !this.campaign ? nothing : super.template
	}
}

declare global {
	interface HTMLElementTagNameMap {
		'solid-donation-progress': DonationProgress
	}
}