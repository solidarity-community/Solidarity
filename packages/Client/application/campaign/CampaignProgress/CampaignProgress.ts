import { Component, component, html, nothing, property } from '@3mo/model'
import { Campaign, CampaignStatus } from 'sdk'

@component('solid-campaign-progress')
export class CampaignProgress extends Component {
	@property({ type: Object }) campaign?: Campaign
	@property({ type: Boolean }) alwaysShowValidationApprovalThreshold = false

	protected override get template() {
		switch (this.campaign?.status) {
			case CampaignStatus.Funding:
				return html`<solid-campaign-funding-progress .campaign=${this.campaign}></solid-campaign-funding-progress>`
			case CampaignStatus.Validation:
				return html`<solid-campaign-validation-progress .campaign=${this.campaign} ?alwaysShowApprovalThreshold=${this.alwaysShowValidationApprovalThreshold}></solid-campaign-validation-progress>`
			case CampaignStatus.Allocation:
				return html`<solid-campaign-allocation-progress .campaign=${this.campaign}></solid-campaign-allocation-progress>`
			default:
				return nothing
		}
	}
}

declare global {
	interface HTMLElementTagNameMap {
		'solid-campaign-progress': CampaignProgress
	}
}