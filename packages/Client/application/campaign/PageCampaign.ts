import { component, PageComponent, html, route, PageError, HttpErrorCode, nothing, state, DialogAcknowledge, NotificationHost, Task, TaskStatus, queryAll, Button, ButtonType, choose } from '@3mo/modelx'
import { CampaignService, CampaignStatus } from 'sdk'
import { DialogCampaign, DialogVote, PageCampaigns, DialogDonate, DialogAuthenticator, DialogCampaignAllocations } from 'application'

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

	@state() private balance = 0
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
			<mo-page style='--mo-page-margin: 0px' heading=${`Campaign ${!this.campaign ? '' : `"${this.campaign?.title}"`}`} ?fullHeight=${this.fetchCampaignTask.status !== TaskStatus.COMPLETE}>
				${this.fetchCampaignTask.render({
					pending: () => html`
						<mo-flex alignItems='center' justifyContent='center'>
							<mo-circular-progress indeterminate></mo-circular-progress>
						</mo-flex>
					`,
					complete: campaign => html`
						<mo-flex direction='horizontal' alignItems='center' padding='20px' background='var(--mo-color-transparent-gray-2)' gap='50px'>
							<solid-campaign-progress width='*' .campaign=${campaign} alwaysShowValidationApprovalThreshold
								@balanceChange=${(e: CustomEvent<number>) => this.balance = e.detail}
							></solid-campaign-progress>
						
							${this.actionButtonsTemplate}
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

	private get actionButtonsTemplate() {
		return html`
			<mo-flex direction='horizontal' gap='10px' justifyContent='flex-end'>
				<mo-button class='action' icon='share' @click=${this.share}>Share</mo-button>

				${this.campaign?.status !== CampaignStatus.Funding ? nothing : html`
					<mo-button class='action' icon='volunteer_activism'
						@click=${() => !this.campaign ? void 0 : new DialogDonate({ campaign: this.campaign }).confirm()}
					>Donate</mo-button>
				`}

				${this.campaign?.status !== CampaignStatus.Allocation ? nothing : html`
					<mo-button class='action' icon='checklist'
						@click=${() => !this.campaign ? void 0 : new DialogCampaignAllocations({ campaign: this.campaign }).confirm()}
					>Allocations</mo-button>
				`}

				${!this.authenticatedUserId || !this.balanceShare || this.campaign?.status !== CampaignStatus.Validation ? nothing : html`
					<mo-button class='action' icon='how_to_vote'
						@click=${() => !this.campaign || !this.balanceShare ? void 0 : new DialogVote({ campaign: this.campaign }).confirm()}
					>
						<mo-flex direction='horizontal' gap='5px' alignItems='baseline'>
							<mo-div>Vote</mo-div>
							<mo-div fontSize='var(--mo-font-size-s)' foreground='var(--mo-color-gray)' style='letter-spacing: 0px; text-transform: initial;'>
								Ends <solid-timer .end=${this.campaign?.validation?.expiration}></solid-timer>
							</mo-div>
						</mo-flex>
					</mo-button>
				`}

				${!this.isAuthenticatedUserTheCreator ? nothing : html`
					<mo-split-button>
						${this.campaign?.status === CampaignStatus.Funding && this.balance >= this.campaign?.totalExpenditure ? html`
							<mo-button class='action' icon='how_to_vote' @click=${this.initiateValidation}>Initiate Validation</mo-button>
							<mo-list-item slot='more' icon='edit' @click=${this.edit}>Edit</mo-list-item>
						` : html`
							<mo-button class='action' icon='edit' @click=${this.edit}>Edit</mo-button>
						`}
						<mo-list-item slot='more' icon='delete'
							?disabled=${this.campaign?.status === CampaignStatus.Allocation}
							@click=${this.delete}
						>Delete</mo-list-item>
					</mo-split-button>
				`}
			</mo-flex>
		`
	}

	private share = async () => {
		if (this.campaign && 'share' in navigator) {
			await navigator.share({
				title: this.campaign.title,
				text: `Check out this campaign on Solidarity`,
				url: window.location.href,
			})
		}
	}

	private initiateValidation = async () => {
		if (this.campaign?.id) {
			const acknowledged = await new DialogAcknowledge({
				heading: 'Initiate Validation',
				content: html`
					Once the campaign is in the validation phase:
					<ul>
						<li>You won't be able to edit campaign's location and expenditures.</li>
						<li>All donors will be notified to initiate the validation voting procedures.</li>
						<li>The campaign cannot transition back to the funding status.</li>
					</ul> 
					Are you sure you want to initiate the validation?
				`,
				primaryButtonText: 'Proceed',
				secondaryButtonText: 'Cancel',
			}).confirm()
			if (acknowledged) {
				try {
					await CampaignService.initiateValidation(this.campaign.id)
					await this.fetchCampaignTask.run()
				} catch (error: any) {
					NotificationHost.instance.notifyError(error.message)
				}
			}
		}
	}

	private async edit() {
		const id = this.campaign?.id
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