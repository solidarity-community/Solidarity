import { component, homePage, html, PageComponent, route, state } from '@3mo/model'
import { Campaign } from 'sdk'
import { PageCampaign } from 'application'

@homePage()
@route('/campaigns')
@component('solid-page-campaigns')
export class PageCampaigns extends PageComponent {
	@state() private campaigns = new Array<Campaign>(
		{
			id: 1, title: 'Title', location: { x: 10, y: 10, z: 10 }, validationId: 10,
			description: 'Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry\'s standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book.It has survived not only.',
		},
		{
			id: 2, title: 'Title', location: { x: 10, y: 10, z: 10 }, validationId: 10,
			description: 'Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry\'s standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book.It has survived not only.',
		},
	)

	protected override async initialized() {
		// this.campaigns = await CampaignService.getAll()
	}

	protected override get template() {
		return html`
			<mo-page heading='Campaigns'>
				<mo-grid columns='repeat(auto-fill, minmax(250px, 1fr))' columnGap='var(--mo-thickness-xl)' rowGap='var(--mo-thickness-xl)'>
					${this.campaigns.map(campaign => html`
						<solid-campaign-card
							tabIndex='0'
							.campaign=${campaign}
							@click=${() => new PageCampaign({ id: campaign.id! }).navigate()}
						></solid-campaign-card>
					`)}
				</mo-grid>
			</mo-page>
		`
	}
}