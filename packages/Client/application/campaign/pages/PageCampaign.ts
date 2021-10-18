import { component, PageComponent, html, route, state, PageHost, PageError, css, HttpErrorCode } from '@3mo/model'
import { Campaign, CampaignService, HttpError } from 'sdk'

@route('/campaign/:id')
@component('solid-page-campaign')
export class PageCampaign extends PageComponent<{ readonly id: number }> {
	@state() private campaign?: Campaign = {
		id: 1, title: 'Title', location: { x: 10, y: 10, z: 10 }, validationId: 10,
		description: 'Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry\'s standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book.It has survived not only.',
	}

	protected override initialized() {
		this.fetchCampaign()
	}

	private async fetchCampaign() {
		try {
			this.campaign = await CampaignService.get(this.parameters.id)
		} catch (error) {
			// new PageError({ error: HttpErrorCode.NotFound, message: 'Campaign not found' }).navigate()
		}
	}

	protected override get template() {
		return html`
			<mo-page header='Campaign'>
				<mo-flex direction='horizontal' alignItems='center' gap='10px'>
					<mo-div width='*' textAlign='center'>
						Funds Raised Graph
					</mo-div>
					<mo-div width='*' textAlign='center'>
						Funds Raised Graph
					</mo-div>
					<mo-flex width='*' direction='horizontal' gap='10px' justifyContent='flex-end'>
						<mo-button icon='share'>Share</mo-button>
						<mo-button raised icon='volunteer_activism'>Donate</mo-button>
					</mo-flex>
				</mo-flex>
				<mo-grid columns='2* *'>
					<mo-section header='Gallery'>Gallery</mo-section>
					<mo-section header='Location'>Location</mo-section>
					<mo-section header='Overview'>
						${this.campaign?.description}
					</mo-section>
					<mo-section header='Expenditure'>Expenditure</mo-section>
				</mo-grid>
				${this.campaign?.title}
			</mo-page>
		`
	}
}