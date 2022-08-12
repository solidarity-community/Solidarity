import { component, PageComponent, html, route, state } from '@3mo/modelx'
import { Campaign, CampaignService } from 'sdk'

@route('/map')
@component('solid-page-campaigns-map')
export class PageCampaignsMap extends PageComponent {
	@state() private campaigns = new Array<Campaign>()

	override async initialized() {
		this.campaigns = await CampaignService.getAll()
	}

	protected override get template() {
		return html`
			<mo-page heading='Map' fullHeight style='--mo-page-margin: 0px'>
				<solid-map .selectedAreas=${this.campaigns.map(c => c.location)}></solid-map>
			</mo-page>
		`
	}
}