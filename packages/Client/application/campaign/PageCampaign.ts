import { component, PageComponent, html, route, state, PageError, HttpErrorCode } from '@3mo/model'
import { Task } from '@lit-labs/task'
import { Campaign, CampaignService } from 'sdk'

@route('/campaign/:id')
@component('solid-page-campaign')
export class PageCampaign extends PageComponent<{ readonly id: number }> {
	private readonly fetchCampaignTask = new Task(this, async () => {
		try {
			return await CampaignService.get(this.parameters.id)
		} catch (error) {
			new PageError({ error: HttpErrorCode.NotFound, message: 'Campaign not found' }).navigate()
			throw error
		}
	}, () => [])

	protected override get template() {
		return html`
			<mo-page heading=${`Campaign "${this.fetchCampaignTask.value?.title}"`}>
				${this.fetchCampaignTask.render({
					pending: () => html`
						<mo-flex alignItems='center' justifyContent='center'>
							<mo-circular-progress indeterminate></mo-circular-progress>
						</mo-flex>
					`,
					complete: campaign => html`
						<mo-flex direction='horizontal' alignItems='center' gap='10px'>
							<mo-div width='*' textAlign='center'>
								Funds Raised Graph
							</mo-div>
							<mo-div width='*' textAlign='center'>
								Funds Raised Graph
							</mo-div>
							<mo-flex width='*' direction='horizontal' gap='10px' justifyContent='flex-end'>
								<mo-button icon='share'>Share</mo-button>
								<mo-button type='raised' icon='volunteer_activism'>Donate</mo-button>
							</mo-flex>
						</mo-flex>
						<mo-grid columns='2* *'>
							<mo-section heading='Gallery'>Gallery</mo-section>
							<mo-section heading='Location'>Location</mo-section>
							<mo-section heading='Overview'>
								${campaign.description}
							</mo-section>
							<mo-section heading='Expenditure'>Expenditure</mo-section>
						</mo-grid>
					`
				})}
			</mo-page>
		`
	}
}