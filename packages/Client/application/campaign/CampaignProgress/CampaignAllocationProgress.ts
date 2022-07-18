import { Component, component, html, property, nothing } from '@3mo/modelx'
import { Campaign } from 'sdk'

@component('solid-campaign-allocation-progress')
export class CampaignAllocationProgress extends Component {
	@property({ type: Object }) campaign?: Campaign

	protected override get template() {
		return !this.campaign?.allocation ? nothing : html`
			<mo-flex direction='horizontal' justifyContent='space-around'>
				${this.getAmountTemplate('Allocated Funds', this.campaign.allocation.totalFundAmount)}
				${this.getAmountTemplate('Allocated Refunds', this.campaign.allocation.totalRefundAmount)}
			</mo-flex>
		`
	}

	private getAmountTemplate(heading: string, amount: number) {
		return html`
			<mo-flex textAlign='center'>
				<mo-div foreground='var(--mo-color-gray)'>${heading}</mo-div>
				<solid-amount fontWeight='bold' fontSize='var(--mo-font-size-l)' foreground='var(--mo-accent)' value=${amount}></solid-amount>
			</mo-flex>
		`
	}
}

declare global {
	interface HTMLElementTagNameMap {
		'solid-campaign-allocation-progress': CampaignAllocationProgress
	}
}