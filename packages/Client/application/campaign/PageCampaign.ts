import { component, PageComponent, html, route, PageError, HttpErrorCode, DialogAuthenticator, nothing, DialogAlert, state, DialogDefault, DialogAcknowledge, Snackbar, NotificationHost } from '@3mo/model'
import { Task, TaskStatus } from '@lit-labs/task'
import { DialogDonate } from 'application'
import { CampaignService, CampaignStatus } from 'sdk'
import { DialogCampaign, DialogVote, PageCampaigns } from '.'

@route('/campaign/:id')
@component('solid-page-campaign')
export class PageCampaign extends PageComponent<{ readonly id: number }> {
	private readonly fetchCampaignTask = new Task(this, async () => {
		try {
			CampaignService.getBalance(this.parameters.id).then(balanceShare => this.balanceShare = balanceShare)
			return await CampaignService.get(this.parameters.id)
		} catch (error) {
			new PageError({ error: HttpErrorCode.NotFound, message: 'Campaign not found' }).navigate()
			throw error
		}
	}, () => [])

	@state() private balance = 0
	@state() private balanceShare = 0

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
								<mo-flex direction='horizontal' gap='var(--mo-thickness-xs)'>
									<solid-amount value=${this.balance}></solid-amount> / <solid-amount value=${campaign.totalExpenditure}></solid-amount>
								</mo-flex>
							</mo-flex>

							<mo-flex width='*' alignItems='center'>
								<solid-campaign-time-progress width='*' .campaign=${campaign}></solid-campaign-time-progress>
								<mo-div foreground='var(--mo-color-gray)'>Target Allocation Date</mo-div>
								<mo-div><solid-timer .end=${campaign.targetAllocationDate}></solid-timer></mo-div>
							</mo-flex>

							<mo-flex width='*' direction='horizontal' gap='10px' justifyContent='flex-end'>
								<mo-button icon='share' @click=${this.share}>Share</mo-button>

								<mo-button icon='volunteer_activism'
									?hidden=${this.campaign?.status !== CampaignStatus.Funding}
									type=${DialogAuthenticator.authenticatedUser.value?.id === this.campaign?.creatorId ? 'normal' : 'raised'}
									@click=${() => !this.fetchCampaignTask.value ? void 0 : new DialogDonate({ campaign: this.fetchCampaignTask.value }).confirm()}
								>Donate</mo-button>

								<mo-button icon='how_to_vote'
									?hidden=${!DialogAuthenticator.authenticatedUser.value?.id || !this.balanceShare || this.campaign?.status !== CampaignStatus.Allocation}
									type=${DialogAuthenticator.authenticatedUser.value?.id === this.campaign?.creatorId ? 'normal' : 'raised'}
									@click=${() => !this.fetchCampaignTask.value || !this.balanceShare ? void 0 : new DialogVote({ campaign: this.fetchCampaignTask.value }).confirm()}
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
		return DialogAuthenticator.authenticatedUser.value?.id !== this.campaign?.creatorId ? nothing : html`
			<mo-split-button>
				<mo-button icon='manage_accounts' @click=${this.edit}>Manage</mo-button>
				<mo-list-item slot='more' ?hidden=${this.campaign?.status !== CampaignStatus.Funding} icon='how_to_vote' @click=${this.declareAllocationPhase}>Declare Allocation Phase</mo-list-item>
				<mo-list-item slot='more' icon='edit' @click=${this.edit}>Edit</mo-list-item>
				<mo-list-item slot='more' icon='delete' @click=${this.delete}>Delete</mo-list-item>
			</mo-split-button>
		`
	}

	private share = async () => {
		if (this.campaign && 'share' in navigator) {
			await navigator.share({
				title: this.campaign.title,
				text: this.campaign.description,
				url: window.location.href
			})
		}
	}

	private declareAllocationPhase = async () => {
		if (this.campaign?.id) {
			const acknowledged = await new DialogAcknowledge({
				heading: 'Declare Allocation Phase',
				content: 'Are you sure you want to declare the allocation phase? This action cannot be undone. All donors will be notified to initiate voting.',
				primaryButtonText: 'Proceed',
				secondaryButtonText: 'Cancel',
			}).confirm()
			if (acknowledged) {
				try {
					await CampaignService.declareAllocationPhase(this.campaign.id)
					await this.fetchCampaignTask.run()
				} catch (error: any) {
					NotificationHost.instance.notifyError(error.message)
				}
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
		await CampaignService.delete(this.parameters.id)
		new PageCampaigns().navigate()
	}
}