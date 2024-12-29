import { component, html, type TemplateResult, css, style, state } from '@a11d/lit'
import { PageComponent, route } from '@a11d/lit-application'
import { IntervalController } from '@3mo/interval-controller'
import { Health, type HealthCheck } from 'application'

@route('/status')
@component('solid-page-status')
export class PageStatus extends PageComponent {
	protected readonly _ = new IntervalController(this, 30_000, () => this.fetchHealth())

	@state() private health?: Health

	private async fetchHealth() {
		this.health = await Health.get()
	}

	static override get styles() {
		return css`
			.checks {
				position: relative;
				gap: 20px;
			}

			.check {
				position: relative;
				gap: 20px;
				padding-left: 20px;
			}

			.checks::before {
				content: ' ';
				position: absolute;
				height: 100%;
				width: 2px;
				top: 0;
				left: 0;
				background: var(--mo-color-gray-transparent);
			}

			.check::before {
				content: ' ';
				position: absolute;
				height: 2px;
				width: 10px;
				top: 19px;
				left: 0;
				background: var(--mo-color-gray-transparent);
			}
		`
	}

	protected override get template() {
		return html`
			<lit-page heading='Status' fullHeight>
				${!this.health ? html`
					<mo-flex alignItems='center' justifyContent='center'>
						<mo-circular-progress></mo-circular-progress>
					</mo-flex>
				` : html`
					<mo-flex ${style({ height: '100%' })}>
						<mo-flex direction='horizontal' gap='6px' alignItems='center' justifyContent='center' ${style({ padding: '20px', background: 'var(--mo-color-transparent-gray-2)', color: 'var(--mo-color-accent)', fontSize: 'x-large' })}>
							<div ${style({ flex: 1 })}>Solidarity</div>
							<solid-status status=${this.health.status}></solid-status>
						</mo-flex>

						<mo-flex ${style({ padding: '20px' })}>
							${this.getHealthChecksTemplate(this.health.checks)}
						</mo-flex>
					</mo-flex>
				`}
			</lit-page>
		`
	}

	private getHealthChecksTemplate(checks: Array<HealthCheck>, index = 0): TemplateResult<1> {
		return html`
			<mo-flex class='checks' ${style({ marginLeft: `${index * 20}px` })}>
				${checks.map(check => this.getHealthCheckTemplate(check, index + 1))}
			</mo-flex>
		`
	}

	private getHealthCheckTemplate(check: HealthCheck, index: number): TemplateResult<1> {
		return html`
			<mo-flex class='check'>
				<mo-flex direction='horizontal' alignItems='center'>
					<div ${style({ flex: 1, fontSize: 'large' })}>${check.key}</div>
					<solid-status status=${check.status}></solid-status>
				</mo-flex>
				${!check.checks.length ? html.nothing : this.getHealthChecksTemplate(check.checks, index)}
			</mo-flex>
		`
	}
}