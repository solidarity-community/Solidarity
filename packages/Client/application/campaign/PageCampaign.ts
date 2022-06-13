import { component, PageComponent, html, route, PageError, HttpErrorCode, DialogAuthenticator, nothing, DialogAlert, state, DialogDefault, DialogAcknowledge } from '@3mo/model'
import { Task, TaskStatus } from '@lit-labs/task'
import { DialogDonate } from 'application'
import { CampaignService, CampaignStatus } from 'sdk'
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

	@state() private balance = 0

	private get campaign() {
		return this.fetchCampaignTask.value
	}

	protected override get template() {
		return html`
			<mo-page heading=${`Campaign ${!this.fetchCampaignTask.value ? '' : `"${this.fetchCampaignTask.value?.title}"`}`} ?fullHeight=${this.fetchCampaignTask.status !== TaskStatus.COMPLETE}>
				${this.fetchCampaignTask.render({
					pending: () => html`
						<mo-flex alignItems='center' justifyContent='center'>
							<mo-circular-progress indeterminate></mo-circular-progress>
						</mo-flex>
					`,
					complete: campaign => html`
						<mo-flex direction='horizontal' alignItems='center' gap='10px'>
							<mo-flex width='*' alignItems='center'>
								<solid-donation-progress width='*'
									.campaign=${campaign}
									@balanceChange=${(e: CustomEvent<number>) => this.balance = e.detail}
								></solid-donation-progress>
								<mo-div foreground='var(--mo-color-gray)'>Fund Raised</mo-div>
								<mo-div>${this.balance} / ${campaign.totalExpenditure}</mo-div>
							</mo-flex>

							<mo-flex width='*' alignItems='center'>
								<solid-campaign-time-progress width='*' .campaign=${campaign}></solid-campaign-time-progress>
								<mo-div foreground='var(--mo-color-gray)'>Target Allocation Date</mo-div>
								<mo-div><solid-timer .end=${campaign.targetAllocationDate}></solid-timer></mo-div>
							</mo-flex>

							<mo-flex width='*' direction='horizontal' gap='10px' justifyContent='flex-end'>
								<mo-button icon='share' @click=${() => this.share()}>Share</mo-button>

								<mo-button icon='volunteer_activism'
									?hidden=${this.campaign?.status !== CampaignStatus.Funding}
									type=${DialogAuthenticator.authenticatedUser.value ? 'normal' : 'raised'}
									@click=${() => !this.fetchCampaignTask.value ? void 0 : new DialogDonate({ campaign: this.fetchCampaignTask.value }).confirm()}
								>Donate</mo-button>

								<mo-button icon='how_to_vote'
									?hidden=${this.campaign?.status !== CampaignStatus.Allocation}
									type=${DialogAuthenticator.authenticatedUser.value ? 'normal' : 'raised'}
								>Vote</mo-button>

								${this.manageButtonTemplate}
							</mo-flex>
						</mo-flex>

						<mo-grid columns='2* *' rows='435px *' gap='25px'>
							<mo-section heading='Gallery'>
								<solid-campaign-slider readOnly .campaign=${campaign}></solid-campaign-slider>
							</mo-section>

							<mo-section heading='Location'>
								<solid-map readOnly .selectedArea=${campaign.location}></solid-map>
							</mo-section>

							<mo-section heading='Overview'>
								${campaign.description}
							</mo-section>

							<solid-section-campaign-expenditure .campaign=${campaign}></solid-section-campaign-expenditure>
						</mo-grid>
					`
				})}
			</mo-page>
		`
	}

	private get manageButtonTemplate() {
		return !DialogAuthenticator.authenticatedUser.value ? nothing : html`
			<mo-split-button>
				<mo-button icon='manage_accounts' @click=${this.edit}>Manage</mo-button>
				<mo-list-item slot='more' ?disabled=${this.campaign?.status !== CampaignStatus.Funding} icon='how_to_vote' @click=${this.declareAllocationPhase}>Declare Allocation Phase</mo-list-item>
				<mo-list-item slot='more' icon='edit' @click=${this.edit}>Edit</mo-list-item>
				<mo-list-item slot='more' icon='delete' @click=${this.delete}>Delete</mo-list-item>
			</mo-split-button>
		`
	}

	private async share() {
		if (this.campaign && 'share' in navigator) {
			await navigator.share({
				title: this.campaign.title,
				text: this.campaign.description,
				url: window.location.href
			})
		}
	}

	private async declareAllocationPhase() {
		if (this.campaign?.id) {
			const acknowledged = await new DialogAcknowledge({
				heading: 'Declare Allocation Phase',
				content: 'Are you sure you want to declare the allocation phase? This action cannot be undone. All donors will be notified of the change to initiate the voting.',
				primaryButtonText: 'Proceed',
				secondaryButtonText: 'Cancel',
			}).confirm()
			if (acknowledged) {
				await CampaignService.declareAllocationPhase(this.campaign.id)
				await this.fetchCampaignTask.run()
			}
		}
	}

	private async edit() {
		const id = this.fetchCampaignTask.value?.id
		if (id) {
			await new DialogCampaign({ id }).confirm()
			await this.fetchCampaignTask.run()
		}
	}

	private delete = async () => {
		await new DialogAlert({
			heading: 'Delete Campaign',
			content: 'Are you sure you want to delete this campaign irreversibly? All donations will be refunded.',
			primaryButtonText: 'Delete'
		}).confirm()
		await CampaignService.delete(this.parameters.id)
		new PageCampaigns().navigate()
	}
}