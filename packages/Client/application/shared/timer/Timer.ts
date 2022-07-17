import { Component, component, css, html, join, nothing, property } from '@3mo/modelx'
import { TimerController } from '../utilities'

@component('solid-timer')
export class Timer extends Component {
	@property({ type: Object }) end = new MoDate

	protected _ = new TimerController(this, 1000, this.requestUpdate.bind(this))

	static override get styles() {
		return css`
			:host {
				white-space: normal;
			}
		`
	}

	protected get remainingTimeSpan() {
		return new MoDate().until(this.end)
	}

	protected override get template() {
		return this.remainingTimeSpan.days >= 1
			? this.overOneDayTemplate
			: this.belowOneDayTemplate
	}

	protected get overOneDayTemplate() {
		return html`${this.remainingTimeSpan.toString({ style: 'short' })}`
	}

	protected get belowOneDayTemplate() {
		const hours = this.remainingTimeSpan.milliseconds < 0 ? 0 : Math.floor((this.remainingTimeSpan.milliseconds % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60))
		const minutes = this.remainingTimeSpan.milliseconds < 0 ? 0 : Math.floor((this.remainingTimeSpan.milliseconds % (1000 * 60 * 60)) / (1000 * 60))
		const seconds = this.remainingTimeSpan.milliseconds < 0 ? 0 : Math.floor((this.remainingTimeSpan.milliseconds % (1000 * 60)) / 1000)
		const hoursAndMinutesTemplate = join([
			!hours ? undefined : html`<mo-div>${this.padWithZero(hours)}</mo-div>`,
			html`<mo-div>${this.padWithZero(minutes)}</mo-div>`
		].filter(Boolean), html`:`)
		return html`
			in
			${hoursAndMinutesTemplate}
			${hours ? nothing : html`<mo-div fontSize='75%'>${this.padWithZero(seconds)}</mo-div>`}
		`
	}

	private padWithZero = (value: number) => value < 10 ? `0${value}` : value
}

declare global {
	interface HTMLElementTagNameMap {
		'solid-timer': Timer
	}
}