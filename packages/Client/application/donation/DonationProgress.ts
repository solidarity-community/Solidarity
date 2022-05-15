import { Component, component, css, html, property, nothing } from '@3mo/model'
import { Campaign } from 'sdk'
import { Progress } from 'application'

@component('solid-donation-progress')
export class DonationProgress extends Progress {
	@property({ type: Object }) campaign!: Campaign

	protected get progress() { return 0.75 }
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