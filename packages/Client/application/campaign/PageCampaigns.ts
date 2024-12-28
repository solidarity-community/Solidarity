import { component, style, css, html, state } from '@a11d/lit'
import { PageComponent, route } from '@a11d/lit-application'
import { contextMenu } from '@3mo/context-menu'
import { DialogCampaign, PageCampaign, Account, Campaign } from 'application'

@route('/', '/campaigns')
@component('solid-page-campaigns')
export class PageCampaigns extends PageComponent {
	@state() private campaigns?: Array<Campaign>
	@state() private authenticatedAccount?: Account

	private async fetchCampaigns() { this.campaigns = await Campaign.getAll() }
	private async fetchAuthenticatedUser() { this.authenticatedAccount = await Account.getAuthenticated() }

	protected override initialized() {
		this.fetchCampaigns()
		this.fetchAuthenticatedUser()
	}

	static override get styles() {
		return css`
			solid-campaign-card:hover {
				transition: 250ms;
				transform: scale(1.02);
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
			<lit-page heading='Campaigns' fullHeight>
				${!this.campaigns ? html`
					<mo-flex alignItems='center' justifyContent='center'>
						<mo-circular-progress></mo-circular-progress>
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
									${contextMenu(() => html`
										<mo-context-menu-item icon='edit' @click=${() => this.open(campaign.id)}>Edit</mo-context-menu-item>
										<mo-context-menu-item icon='delete' @click=${() => this.delete(campaign.id!)}>Delete</mo-context-menu-item>
									`)}
								></solid-campaign-card>
							`)}
							${fabTemplate}
						</mo-grid>
					`}
				`}
			</lit-page>
		`
	}

	private async open(id?: number) {
		await new DialogCampaign({ id }).confirm()
		await this.fetchCampaigns()
	}

	private delete = async (campaignId: number) => {
		await Campaign.delete(campaignId)
		await this.fetchCampaigns()
	}
}