import { component, ContextMenuHost, css, DialogAuthenticator, homePage, html, PageComponent, route, state } from '@3mo/modelx'
import { DialogCampaign, PageCampaign } from 'application'
import { Campaign, CampaignService } from 'sdk'

@homePage()
@route('/campaigns')
@component('solid-page-campaigns')
export class PageCampaigns extends PageComponent {
	@state() private campaigns?: Array<Campaign>

	private async fetchCampaigns() { this.campaigns = await CampaignService.getAll() }

	protected override initialized() {
		this.fetchCampaigns()
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
			<mo-fab position='fixed' right='16px' bottom='16px' icon='add'
				?hidden=${!DialogAuthenticator.authenticatedUser.value}
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
							<mo-error icon='youtube_searched_for'>No campaigns found</mo-error>
							${fabTemplate}
						</mo-flex>
					` : html`
						<mo-grid columns='repeat(auto-fill, minmax(300px, 1fr))' gap='var(--mo-thickness-xxl)'>
							${this.campaigns.map(campaign => html`
								<solid-campaign-card
									tabIndex='0'
									.campaign=${campaign}
									@click=${() => new PageCampaign({ id: campaign.id! }).navigate()}
									@contextmenu=${(event: MouseEvent) => campaign.creatorId !== DialogAuthenticator.authenticatedUser.value?.id ? void 0 : ContextMenuHost.openMenu(event, html`
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