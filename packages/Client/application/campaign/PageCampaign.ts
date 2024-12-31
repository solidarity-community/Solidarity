import { component, html, state, queryAll, style, ifDefined } from '@a11d/lit'
import { PageComponent, NotificationComponent, route, PageError, HttpErrorCode, } from '@a11d/lit-application'
import { type Button, ButtonType } from '@3mo/button'
import { DialogAcknowledge } from '@3mo/standard-dialogs'
import { IntervalController } from '@3mo/interval-controller'
import { Account, Campaign, CampaignStatus, DialogCampaign, DialogCampaignValidationVote, PageCampaigns, DialogDonate, DialogCampaignAllocations } from 'application'
import { Task } from '@lit/task'

@route('/campaign/:id')
@component('solid-page-campaign')
export class PageCampaign extends PageComponent<{ readonly id: number }> {
	@state() private balance = 0

	private readonly balanceShareFetchTask = new Task(this, () => Campaign.getShare(this.parameters.id), () => [])
	private get balanceShare() { return this.balanceShareFetchTask.value }

	private readonly authenticatedAccountFetchTask = new Task(this, Account.getAuthenticated, () => [])
	private get authenticatedAccount() { return this.authenticatedAccountFetchTask.value }

	private readonly campaignFetchTask = new Task(this, async () => {
		try {
			return await Campaign.get(this.parameters.id)
		} catch {
			new PageError({ error: HttpErrorCode.NotFound, message: 'Campaign not found' }).navigate()
			throw new Error('Campaign not found')
		}
	}, () => [])
	private get campaign() { return this.campaignFetchTask.value }


	@queryAll('.action') private actionButtonElements!: Array<Button>

	protected readonly campaignFetcherTimer = new IntervalController(this, 10_000, () => {
		this.campaignFetchTask.run()
		this.balanceShareFetchTask.run()
	})

	private get isAuthenticatedUserTheCreator() {
		return this.authenticatedAccount?.id === this.campaign?.creatorId
	}

	protected override updated() {
		for (const [index, actionButtonElement] of this.actionButtonElements.entries()) {
			actionButtonElement.type = this.actionButtonElements.length - 1 === index ? ButtonType.Raised : ButtonType.Outlined
		}
	}

	protected override get template() {
		return html`
			<lit-page heading=${`Campaign ${!this.campaign ? '' : `"${this.campaign?.title}"`}`} ?fullHeight=${!this.campaign}>
				${!this.campaign ? html`
					<mo-flex alignItems='center' justifyContent='center'>
						<mo-circular-progress></mo-circular-progress>
					</mo-flex>
				` : html`
					<mo-flex direction='horizontal' alignItems='center' justifyContent='end' gap='25px' style='padding-block: 25px' wrap='wrap'>
						<solid-campaign-progress .campaign=${this.campaign} alwaysShowValidationApprovalThreshold
							${style({ flex: '1 0 300px' })}
							@balanceChange=${(e: CustomEvent<number>) => this.balance = e.detail}
						></solid-campaign-progress>

						${this.actionButtonsTemplate}
					</mo-flex>

					<mo-flex direction='horizontal' gap='25px' wrap='wrap' ${style({ width: '100%' })}>
						<mo-flex gap='25px' ${style({ flex: '2 0 300px', width: '100%' })}>
							<solid-campaign-media-card readOnly hideWhenEmpty .campaign=${this.campaign}></solid-campaign-media-card>

							<mo-card heading=${ifDefined(this.campaign.title)}>
								${this.campaign.description}
							</mo-card>
						</mo-flex>

						<mo-flex gap='25px' ${style({ flex: '1 0 150px' })}>
							<mo-card heading='Location' style='--mo-card-body-padding: 0'>
								<solid-map readOnly .selectedArea=${this.campaign.location}></solid-map>
							</mo-card>

							<solid-campaign-expenditure-card .campaign=${this.campaign}></solid-campaign-expenditure-card>
						</mo-flex>
					</mo-flex>
				`}
			</lit-page>
		`
	}

	private get actionButtonsTemplate() {
		return html`
			<mo-flex direction='horizontal' gap='10px' justifyContent='flex-end'>
				<mo-button class='action' leadingIcon='share' @click=${this.share}>Share</mo-button>

				${this.campaign?.status !== CampaignStatus.Funding ? html.nothing : html`
					<mo-loading-button class='action' leadingIcon='volunteer_activism'
						@click=${() => !this.campaign ? void 0 : new DialogDonate({ campaign: this.campaign }).confirm()}
					>Donate</mo-loading-button>
				`}

				${this.campaign?.status !== CampaignStatus.Allocation ? html.nothing : html`
					<mo-button class='action' leadingIcon='checklist'
						@click=${() => !this.campaign ? void 0 : new DialogCampaignAllocations({ campaign: this.campaign }).confirm()}
					>Allocations</mo-button>
				`}

				${!this.campaign || !this.authenticatedAccount?.id || !this.balanceShare || this.campaign?.status !== CampaignStatus.Validation ? html.nothing : html`
					<mo-button class='action' leadingIcon='how_to_vote'
						@click=${() => new DialogCampaignValidationVote({ campaign: this.campaign! }).confirm()}
					>
						<mo-flex direction='horizontal' gap='5px' alignItems='baseline'>
							<div>Vote</div>
							<div ${style({ fontSize: 'small', color: 'var(--mo-color-foreground)', opacity: '0.75', letterSpacing: '0px', textTransform: 'initial' })}>
								Ends <solid-timer .end=${this.campaign?.validation?.expiration}></solid-timer>
							</div>
						</mo-flex>
					</mo-button>
				`}

				${!this.isAuthenticatedUserTheCreator ? html.nothing : html`
					<mo-split-button>
						${this.campaign?.status === CampaignStatus.Funding && this.balance >= this.campaign?.totalExpenditure ? html`
							<mo-button class='action' leadingIcon='how_to_vote' @click=${this.initiateValidation}>Initiate Validation</mo-button>
							<mo-menu-item slot='more' icon='edit' @click=${this.edit}>Edit</mo-menu-item>
						` : html`
							<mo-button class='action' leadingIcon='edit' @click=${this.edit}>Edit</mo-button>
						`}
						<mo-menu-item slot='more' icon='delete'
							?disabled=${this.campaign?.status === CampaignStatus.Allocation}
							@click=${this.delete}
						>Delete</mo-menu-item>
					</mo-split-button>
				`}
			</mo-flex>
		`
	}

	private share = async () => {
		if (this.campaign && 'share' in navigator) {
			await navigator.share({
				title: this.campaign.title,
				text: 'Check out this campaign on Solidarity',
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
					await Campaign.initiateValidation(this.campaign.id)
					await this.campaignFetchTask.run()
				} catch (error: any) {
					NotificationComponent.notifyError(error.message)
				}
			}
		}
	}

	private async edit() {
		const id = this.campaign?.id
		if (id) {
			await new DialogCampaign({ id }).confirm()
			await this.campaignFetchTask.run()
		}
	}

	private delete = async () => {
		await Campaign.delete(this.parameters.id)
		new PageCampaigns().navigate()
	}
}