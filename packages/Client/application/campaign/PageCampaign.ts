import { component, PageComponent, html, route, state, PageError, HttpErrorCode, DialogAuthenticator, nothing } from '@3mo/model'
import { Task, TaskStatus } from '@lit-labs/task'
import { CampaignService } from 'sdk'
import { DialogCampaign, PageCampaigns } from '.'

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
			<mo-page heading=${`Campaign "${this.fetchCampaignTask.value?.title}"`} ?fullHeight=${this.fetchCampaignTask.status !== TaskStatus.COMPLETE}>
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
								<mo-button type=${DialogAuthenticator.authenticatedUser.value ? 'normal' : 'raised'} icon='volunteer_activism'>Donate</mo-button>
								${this.manageButtonTemplate}
							</mo-flex>
						</mo-flex>

						<mo-grid columns='2* *' rows='500px *'>
							<mo-section heading='Gallery'>Gallery</mo-section>
							<mo-section heading='Location'>
								<solid-map readOnly .selectedArea=${campaign.location}></solid-map>
							</mo-section>
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

	private get manageButtonTemplate() {
		return !DialogAuthenticator.authenticatedUser.value ? nothing : html`
			<mo-split-button>
				<mo-button icon='manage_accounts'>Manage</mo-button>
				<mo-list-item slot='more' icon='edit' @click=${this.edit}>Edit</mo-list-item>
				<mo-list-item slot='more' icon='delete' @click=${this.delete}>Delete</mo-list-item>
			</mo-split-button>
		`
	}

	private async edit() {
		const id = this.fetchCampaignTask.value?.id
		if (id) {
			await new DialogCampaign({ id }).confirm()
			await this.fetchCampaignTask.run()
		}
	}

	private delete = async () => {
		await CampaignService.delete(this.parameters.id)
		new PageCampaigns().navigate()
	}
}