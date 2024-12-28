import { Component, component, css, html, join, property, style } from '@a11d/lit'
import { IntervalController } from '@3mo/interval-controller'

@component('solid-timer')
export class Timer extends Component {
	@property({ type: Object }) end = new DateTime

	protected _ = new IntervalController(this, 1000, this.requestUpdate.bind(this))

	static override get styles() {
		return css`
			:host {
				white-space: normal;
			}
		`
	}

	protected get remainingTimeSpan() {
		return new DateTime().until(this.end)
	}

	protected override get template() {
		return this.remainingTimeSpan.days >= 1
			? this.overOneDayTemplate
			: this.belowOneDayTemplate
	}

	protected get overOneDayTemplate() {
		return html`${this.remainingTimeSpan.format({ style: 'short' })}`
	}

	protected get belowOneDayTemplate() {
		const hours = this.remainingTimeSpan.milliseconds < 0 ? 0 : Math.floor((this.remainingTimeSpan.milliseconds % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60))
		const minutes = this.remainingTimeSpan.milliseconds < 0 ? 0 : Math.floor((this.remainingTimeSpan.milliseconds % (1000 * 60 * 60)) / (1000 * 60))
		const seconds = this.remainingTimeSpan.milliseconds < 0 ? 0 : Math.floor((this.remainingTimeSpan.milliseconds % (1000 * 60)) / 1000)
		const hoursAndMinutesTemplate = join([
			!hours ? undefined : html`<div>${this.padWithZero(hours)}</div>`,
			html`<div>${this.padWithZero(minutes)}</div>`
		].filter(Boolean), html`:`)
		return html`
			in
			${hoursAndMinutesTemplate}
			${hours ? html.nothing : html`<div ${style({ fontSize: '75%' })}>${this.padWithZero(seconds)}</div>`}
		`
	}

	private padWithZero = (value: number) => value < 10 ? `0${value}` : value
}

declare global {
	interface HTMLElementTagNameMap {
		'solid-timer': Timer
	}
}