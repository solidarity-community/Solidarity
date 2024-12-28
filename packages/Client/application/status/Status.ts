import { Component, component, css, html, ifDefined, property, unsafeCSS } from '@a11d/lit'
import { type MaterialIcon } from '@3mo/icon'
import { HealthCheckStatus } from './Health'

@component('solid-status')
export class Status extends Component {
	private static readonly iconByStatus = new Map<HealthCheckStatus, MaterialIcon>([
		[HealthCheckStatus.Healthy, 'check_circle_outline'],
		[HealthCheckStatus.Degraded, 'warning_amber'],
		[HealthCheckStatus.Unhealthy, 'error_outline'],
	])

	private static readonly textByStatus = new Map<HealthCheckStatus, string>([
		[HealthCheckStatus.Healthy, 'Operational'],
		[HealthCheckStatus.Degraded, 'Degraded'],
		[HealthCheckStatus.Unhealthy, 'Outage'],
	])

	@property({ type: Number, reflect: true }) status!: HealthCheckStatus

	static override get styles() {
		return css`
			:host([status="${unsafeCSS(HealthCheckStatus.Healthy)}"]) {
				--solid-status-color-base: 40, 167, 69;
			}

			:host([status="${unsafeCSS(HealthCheckStatus.Degraded)}"]) {
				--solid-status-color-base: 217, 183, 37;
			}

			:host([status="${unsafeCSS(HealthCheckStatus.Unhealthy)}"]) {
				--solid-status-color-base: 217, 37, 37;
			}

			:host {
				color: rgb(var(--solid-status-color-base));
				background: rgba(var(--solid-status-color-base), 0.12);

				display: flex;
				align-items: center;
				gap: 6px;
				padding: 8px;
				padding-right: 10px;
				border-radius: 4px;
			}
		`
	}

	protected override get template() {
		return html`
			<mo-icon icon=${ifDefined(Status.iconByStatus.get(this.status))}></mo-icon>
			${Status.textByStatus.get(this.status)}
		`
	}
}

declare global {
	interface HTMLElementTagNameMap {
		'solid-status': Status
	}
}