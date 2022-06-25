import { component, property, nothing, html } from '@3mo/model'
import { Campaign } from 'sdk'
import { Progress } from 'application'

@component('solid-campaign-time-progress')
export class CampaignTimeProgress extends Progress {
	@property({ type: Object }) campaign!: Campaign

	protected get progress() { return this.campaign.remainingTimePercentage }
	protected get progressTemplate() { return html`<solid-timer .end=${this.campaign.targetAllocationDate}></solid-timer>` }

	protected override get template() {
		return !this.campaign ? nothing : super.template
	}
}

declare global {
	interface HTMLElementTagNameMap {
		'solid-campaign-time-progress': CampaignTimeProgress
	}
}