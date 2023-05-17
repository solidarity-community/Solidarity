import { contextMenu } from '@3mo/context-menu'
import { component, style, css, html, state } from '@a11d/lit'
import { PageComponent, route } from '@a11d/lit-application'
import { DialogCampaign, PageCampaign } from 'application'
import { Account, AccountService, Campaign, CampaignService } from 'sdk'

@route('/', '/campaigns')
@component('solid-page-campaigns')
export class PageCampaigns extends PageComponent {
	@state() private campaigns?: Array<Campaign>
	@state() private authenticatedAccount?: Account

	private async fetchCampaigns() { this.campaigns = await CampaignService.getAll() }
	private async fetchAuthenticatedUser() { this.authenticatedAccount = await AccountService.getAuthenticated() }

	protected override initialized() {
		this.fetchCampaigns()
		this.fetchAuthenticatedUser()
	}

	static override get styles() {
		return css`
			solid-campaign-card:hover {
				transition: var(--mo-duration-quick);
				transform: scale(1.05);
			}
		`
	}

	protected override get template() {
		const fabTemplate = html`
			<mo-fab icon='add' ${style({ position: 'fixed', right: '16px', bottom: '16px' })}
				?hidden=${!this.authenticatedAccount}
				@click=${() => this.open()}
			>New Campaign</mo-fab>
		`
		return html`
			<mo-page heading='Campaigns' fullHeight>
				${!this.campaigns ? html`
					<mo-flex alignItems='center' justifyContent='center'>
						<mo-circular-progress indeterminate></mo-circular-progress>
						${fabTemplate}
					</mo-flex>
				` : html`
					${this.campaigns.length === 0 ? html`
						<mo-flex justifyContent='center'>
							<mo-empty-state icon='youtube_searched_for'>No campaigns found</mo-empty-state>
							${fabTemplate}
						</mo-flex>
					` : html`
						<mo-grid columns='repeat(auto-fill, minmax(300px, 1fr))' gap='20px'>
							${this.campaigns.map(campaign => html`
								<solid-campaign-card
									tabIndex='0'
									.campaign=${campaign}
									@click=${() => new PageCampaign({ id: campaign.id! }).navigate()}
									${contextMenu(html`
										<mo-context-menu-item icon='edit' @click=${() => this.open(campaign.id)}>Edit</mo-context-menu-item>
										<mo-context-menu-item icon='delete' @click=${() => this.delete(campaign.id!)}>Delete</mo-context-menu-item>
									`)}
								></solid-campaign-card>
							`)}
							${fabTemplate}
						</mo-grid>
					`}
				`}
			</mo-page>
		`
	}

	private async open(id?: number) {
		await new DialogCampaign(!id ? undefined : { id }).confirm()
		await this.fetchCampaigns()
	}

	private delete = async (campaignId: number) => {
		await CampaignService.delete(campaignId)
		await this.fetchCampaigns()
	}
}