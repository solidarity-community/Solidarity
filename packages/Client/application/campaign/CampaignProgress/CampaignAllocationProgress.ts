import { Component, component, html, property, css } from '@a11d/lit'
import { type Campaign } from 'application'

@component('solid-campaign-allocation-progress')
export class CampaignAllocationProgress extends Component {
	@property({ type: Object }) campaign?: Campaign

	static override get styles() {
		return css`
			.amount { text-align: center; }
			.amount > div { color: var(--mo-color-gray); }
			.amount > solid-amount {
				font-weight: bold;
				font-size: large;
				color: var(--mo-color-accent);
			}
		`
	}

	protected override get template() {
		return !this.campaign?.allocation ? html.nothing : html`
			<mo-flex direction='horizontal' justifyContent='space-around'>
				${this.getAmountTemplate('Allocated Funds', this.campaign.allocation.totalFundAmount)}
				${this.getAmountTemplate('Allocated Refunds', this.campaign.allocation.totalRefundAmount)}
			</mo-flex>
		`
	}

	private getAmountTemplate(heading: string, amount: number) {
		return html`
			<mo-flex class='amount'>
				<div>${heading}</div>
				<solid-amount value=${amount}></solid-amount>
			</mo-flex>
		`
	}
}

declare global {
	interface HTMLElementTagNameMap {
		'solid-campaign-allocation-progress': CampaignAllocationProgress
	}
}