import { Component, component, css, html, join, nothing, property } from '@3mo/model'

@component('solid-timer')
export class Timer extends Component {
	@property({ type: Object }) end = new MoDate

	static override get styles() {
		return css`
			:host {
				white-space: normal;
			}
		`
	}

	protected override initialized() {
		setInterval(() => this.requestUpdate(), 1000)
	}

	protected get remainingTimeSpan() {
		return new MoDate().until(this.end)
	}

	protected override get template() {
		return new MoDate().until(this.end).days >= 1
			? this.belowOneDayTemplate
			: this.overOneDayTemplate
	}

	protected get belowOneDayTemplate() {
		return html`${this.remainingTimeSpan.toString({ style: 'short' })}`
	}

	protected get overOneDayTemplate() {
		const hours = Math.floor((this.remainingTimeSpan.milliseconds % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60))
		const minutes = Math.floor((this.remainingTimeSpan.milliseconds % (1000 * 60 * 60)) / (1000 * 60))
		const seconds = Math.floor((this.remainingTimeSpan.milliseconds % (1000 * 60)) / 1000)
		const hoursAndMinutesTemplate = join([
			!hours ? undefined : html`<mo-div>${this.padWithZero(hours)}</mo-div>`,
			html`<mo-div>${this.padWithZero(minutes)}</mo-div>`
		].filter(Boolean), html`:`)
		return html`
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