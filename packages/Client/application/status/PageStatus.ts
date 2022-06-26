import { component, PageComponent, html, route, Task, TemplateResult, nothing, css, styleMap, state } from '@3mo/model'
import { Health, HealthCheck, HealthService } from 'sdk'
import { IntervalController } from 'application'

@route('/status')
@component('solid-page-status')
export class PageStatus extends PageComponent {
	protected readonly _ = new IntervalController(this, TimeSpan.fromMilliseconds(30_000), () => this.fetchHealth())

	@state() private health?: Health

	private async fetchHealth() {
		this.health = await HealthService.get()
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
			<mo-page heading='Status' fullHeight style='--mo-page-margin: 0px'>
				${!this.health ? html`
					<mo-flex alignItems='center' justifyContent='center'>
						<mo-circular-progress indeterminate></mo-circular-progress>
					</mo-flex>
				` : html`
					<mo-flex height='100%'>
						<mo-flex direction='horizontal' background='var(--mo-color-transparent-gray-2)' foreground='var(--mo-accent)' gap='6px' padding='20px' alignItems='center' justifyContent='center' fontSize='x-large'>
							<mo-div width='*'>Solidarity</mo-div>
							<solid-status status=${this.health.status}></solid-status>
						</mo-flex>

						<mo-flex padding='20px'>
							${this.getHealthChecksTemplate(this.health.checks)}
						</mo-flex>
					</mo-flex>
				`}
			</mo-page>
		`
	}

	private getHealthChecksTemplate(checks: Array<HealthCheck>, index = 0): TemplateResult<1> {
		return html`
			<mo-flex class='checks' style=${styleMap({ marginLeft: `${index * 20}px` })}>
				${checks.map(check => this.getHealthCheckTemplate(check, index + 1))}
			</mo-flex>
		`
	}

	private getHealthCheckTemplate(check: HealthCheck, index: number): TemplateResult<1> {
		return html`
			<mo-flex class='check'>
				<mo-flex direction='horizontal' alignItems='center'>
					<mo-div width='*' fontSize='large'>${check.key}</mo-div>
					<solid-status status=${check.status}></solid-status>
				</mo-flex>
				${!check.checks?.length ? nothing : this.getHealthChecksTemplate(check.checks, index)}
			</mo-flex>
		`
	}
}