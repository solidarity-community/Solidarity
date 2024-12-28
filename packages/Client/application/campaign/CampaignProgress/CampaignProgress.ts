import { Component, component, html, property, event } from '@a11d/lit'
import { type Campaign, CampaignStatus } from 'application'

/** @fires balanceChange */
@component('solid-campaign-progress')
export class CampaignProgress extends Component {
	@event() readonly balanceChange!: EventDispatcher<number>

	@property({ type: Object }) campaign?: Campaign
	@property({ type: Boolean }) alwaysShowValidationApprovalThreshold = false

	protected override get template() {
		switch (this.campaign?.status) {
			case CampaignStatus.Funding:
				return html`
					<solid-campaign-funding-progress
						.campaign=${this.campaign}
						@balanceChange=${(e: CustomEvent<number>) => this.balanceChange.dispatch(e.detail)}
					></solid-campaign-funding-progress>
				`
			case CampaignStatus.Validation:
				return html`<solid-campaign-validation-progress .campaign=${this.campaign} ?alwaysShowApprovalThreshold=${this.alwaysShowValidationApprovalThreshold}></solid-campaign-validation-progress>`
			case CampaignStatus.Allocation:
				return html`<solid-campaign-allocation-progress .campaign=${this.campaign}></solid-campaign-allocation-progress>`
			default:
				return html.nothing
		}
	}
}

declare global {
	interface HTMLElementTagNameMap {
		'solid-campaign-progress': CampaignProgress
	}
}