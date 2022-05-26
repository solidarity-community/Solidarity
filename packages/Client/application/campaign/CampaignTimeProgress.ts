import { component, property, nothing, html } from '@3mo/model'
import { Campaign } from 'sdk'
import { Progress } from 'application'

@component('solid-campaign-time-progress')
export class CampaignTimeProgress extends Progress {
	@property({ type: Object }) campaign!: Campaign

	private endDate = new MoDate().add({ hours: 1, minutes: 1 })

	protected get progress() { return 0.35 }
	protected get progressTemplate() { return html`<solid-timer .end=${this.endDate}></solid-timer>` }

	protected override get template() {
		return !this.campaign ? nothing : super.template
	}
}

declare global {
	interface HTMLElementTagNameMap {
		'solid-campaign-time-progress': CampaignTimeProgress
	}
}