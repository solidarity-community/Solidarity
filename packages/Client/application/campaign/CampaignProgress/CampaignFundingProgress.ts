import { component, property, state, html, event, style } from '@a11d/lit'
import { IntervalController } from '@3mo/interval-controller'
import { Campaign, Progress } from 'application'

/** @fires balanceChange */
@component('solid-campaign-funding-progress')
export class CampaignFundingProgress extends Progress {
	private static readonly balanceFetchInterval = 10_000

	@event() readonly balanceChange!: EventDispatcher<number>

	@property({ type: Object, updated(this: CampaignFundingProgress) { this.fetchBalance() } }) campaign!: Campaign

	@state() private balance = 0

	protected _ = new IntervalController(this, CampaignFundingProgress.balanceFetchInterval, this.fetchBalance.bind(this))

	private async fetchBalance() {
		if (this.campaign.id) {
			this.balance = await Campaign.getBalance(this.campaign.id)
			this.balanceChange.dispatch(this.balance)
		}
	}

	protected get heading() { return 'Funds Raised' }
	protected get progress() { return this.balance / this.campaign.totalExpenditure }
	protected get value() {
		return html`
			<mo-flex direction='horizontal' gap='2px' ${style({ color: 'var(--mo-color-gray)' })}>
				<solid-amount value=${this.balance} ${style({ color: 'var(--mo-color-foreground)' })}></solid-amount>
				/
				<solid-amount value=${this.campaign.totalExpenditure}></solid-amount>
			</mo-flex>
		`
	}

	protected override get template() {
		return !this.campaign ? html.nothing : super.template
	}
}

declare global {
	interface HTMLElementTagNameMap {
		'solid-campaign-funding-progress': CampaignFundingProgress
	}
}