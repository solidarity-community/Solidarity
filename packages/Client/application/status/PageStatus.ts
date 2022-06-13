import { component, PageComponent, html, route, Task } from '@3mo/model'
import { HealthService } from 'sdk'

@route('/status')
@component('solid-page-status')
export class PageStatus extends PageComponent {
	private readonly fetchHealthTask = new Task(this, HealthService.get, () => [])

	protected override get template() {
		return html`
			<mo-page heading='Status'>
				${this.fetchHealthTask.render({
					pending: () => html`
						<mo-flex alignItems='center' justifyContent='center'>
							<mo-circular-progress indeterminate></mo-circular-progress>
						</mo-flex>
					`, complete: health => html`
						Overall status: ${health.status}

						<ul>
							${health.checks.map(healthCheck => html`
								<li>
									${healthCheck.key}: ${healthCheck.status} ${healthCheck.description}
								</li>
							`)}
						</ul>
					`
				})}
			</mo-page>
		`
	}
}