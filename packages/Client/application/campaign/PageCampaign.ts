import { component, PageComponent, html, route, PageError, HttpErrorCode, nothing, state, DialogAcknowledge, NotificationHost, Task, TaskStatus, queryAll, Button, ButtonType } from '@3mo/model'
import { CampaignService, CampaignStatus } from 'sdk'
import { DialogCampaign, DialogVote, PageCampaigns, DialogDonate, DialogAuthenticator } from 'application'

@route('/campaign/:id')
@component('solid-page-campaign')
export class PageCampaign extends PageComponent<{ readonly id: number }> {
	private readonly fetchCampaignTask = new Task(this, async () => {
		try {
			CampaignService.getShare(this.parameters.id).then(balanceShare => this.balanceShare = balanceShare)
			return await CampaignService.get(this.parameters.id)
		} catch (error) {
			new PageError({ error: HttpErrorCode.NotFound, message: 'Campaign not found' }).navigate()
			throw error
		}
	}, () => [])

	@state() private balanceShare = 0
	@queryAll('mo-button.action') private actionButtonElements!: Array<Button>

	private get campaign() {
		return this.fetchCampaignTask.value
	}

	private get authenticatedUserId() {
		return DialogAuthenticator.authenticatedUser.value?.id
	}

	private get isAuthenticatedUserTheCreator() {
		return this.authenticatedUserId === this.campaign?.creatorId
	}

	protected override updated() {
		for (const [index, actionButtonElement] of this.actionButtonElements.entries()) {
			actionButtonElement.type = this.actionButtonElements.length - 1 === index ? ButtonType.Raised : ButtonType.Outlined
		}
	}

	protected override get template() {
		return html`
			<mo-page style='--mo-page-margin: 0px' heading=${`Campaign ${!this.fetchCampaignTask.value ? '' : `"${this.fetchCampaignTask.value?.title}"`}`} ?fullHeight=${this.fetchCampaignTask.status !== TaskStatus.COMPLETE}>
				${this.fetchCampaignTask.render({
					pending: () => html`
						<mo-flex alignItems='center' justifyContent='center'>
							<mo-circular-progress indeterminate></mo-circular-progress>
						</mo-flex>
					`,
					complete: campaign => html`
						<mo-flex direction='horizontal' alignItems='center' padding='20px' background='var(--mo-color-transparent-gray-2)' gap='50px'>
							<solid-campaign-progress width='*' .campaign=${campaign} alwaysShowValidationApprovalThreshold></solid-campaign-progress>

							<mo-flex direction='horizontal' gap='10px' justifyContent='flex-end'>
								<mo-button class='action' icon='share' @click=${this.share}>Share</mo-button>

								${this.campaign?.status !== CampaignStatus.Funding ? nothing : html`
									<mo-button class='action' icon='volunteer_activism'
										@click=${() => !this.fetchCampaignTask.value ? void 0 : new DialogDonate({ campaign: this.fetchCampaignTask.value }).confirm()}
									>Donate</mo-button>
								`}

								${!this.authenticatedUserId || !this.balanceShare || this.campaign?.status !== CampaignStatus.Validation ? nothing : html`
									<mo-button class='action' icon='how_to_vote'
										@click=${() => !this.fetchCampaignTask.value || !this.balanceShare ? void 0 : new DialogVote({ campaign: this.fetchCampaignTask.value }).confirm()}
									>
										<mo-flex direction='horizontal' gap='5px' alignItems='baseline'>
											<mo-div>Vote</mo-div>
											<mo-div fontSize='var(--mo-font-size-s)' foreground='var(--mo-color-gray)' style='letter-spacing: 0px; text-transform: initial;'>
												Ends <solid-timer .end=${this.campaign.validation?.expiration}></solid-timer>
											</mo-div>
										</mo-flex>
									</mo-button>
								`}

								${!this.isAuthenticatedUserTheCreator ? nothing : html`
									<mo-split-button>
										<mo-button class='action' icon='manage_accounts' @click=${this.edit}>Manage</mo-button>
										<mo-list-item slot='more' ?hidden=${this.campaign?.status !== CampaignStatus.Funding} icon='how_to_vote' @click=${this.declareAllocationPhase}>Declare Allocation Phase</mo-list-item>
										<mo-list-item slot='more' icon='edit' @click=${this.edit}>Edit</mo-list-item>
										<mo-list-item slot='more' icon='delete' @click=${this.delete}>Delete</mo-list-item>
									</mo-split-button>
								`}
							</mo-flex>
						</mo-flex>

						<mo-grid columns='2* *' rows='435px *' gap='25px' padding='20px'>
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