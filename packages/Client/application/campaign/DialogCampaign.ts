import { component, DialogComponent, html, state } from '@3mo/model'
import { Campaign, CampaignService } from 'sdk'
import { GeometryCollection } from 'geojson'

@component('solid-dialog-campaign')
export class DialogCampaign extends DialogComponent<undefined | { readonly id: number }, Campaign> {
	@state() private campaign: Campaign = {
		media: [],
		expenditures: []
	}

	protected override async initialized() {
		if (this.parameters?.id) {
			this.campaign = await CampaignService.get(this.parameters.id)
		}
	}

	protected override get template() {
		return html`
			<mo-dialog size='medium' heading='Campaign' primaryButtonText=${this.parameters?.id ? 'Edit' : 'Create'}>
				<mo-flex gap='var(--mo-thickness-m)'>
					<mo-field-text label='Title'
						value=${this.campaign.title ?? ''}
						@change=${(e: CustomEvent<string>) => this.campaign.title = e.detail}
					></mo-field-text>

					<mo-field-text-area label='Description'
						value=${this.campaign.description ?? ''}
						@change=${(e: CustomEvent<string>) => this.campaign.description = e.detail}
					></mo-field-text-area>

					<solid-campaign-slider .campaign=${this.campaign}></solid-campaign-slider>

					<solid-map height='400px'
						.selectedArea=${this.campaign.location}
						@selectedAreaChange=${(e: CustomEvent<GeometryCollection>) => this.campaign.location = e.detail}
					></solid-map>

					<solid-section-campaign-expenditure editable .expenditures=${this.campaign.expenditures}></solid-section-campaign-expenditure>
				</mo-flex>
			</mo-dialog>
		`
	}

	protected override primaryButtonAction = () => CampaignService.save(this.campaign)
}