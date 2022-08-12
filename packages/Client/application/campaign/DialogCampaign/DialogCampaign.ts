import { component, DialogComponent, html, ifDefined, state } from '@3mo/modelx'
import { Campaign, CampaignPaymentMethod, CampaignService, CampaignStatus, PaymentMethod, PaymentMethodService } from 'sdk'
import { GeometryCollection } from 'geojson'

@component('solid-dialog-campaign')
export class DialogCampaign extends DialogComponent<undefined | { readonly id: number }, Campaign> {
	@state() private campaign = new Campaign
	@state() private paymentMethods?: Array<PaymentMethod>

	private async fetchCampaign() {
		if (this.parameters?.id) {
			this.campaign = await CampaignService.get(this.parameters.id)
		}
	}

	private async fetchPaymentMethods() {
		this.paymentMethods = await PaymentMethodService.getAll()
	}

	protected override initialized() {
		this.fetchCampaign()
		this.fetchPaymentMethods()
	}

	protected override get template() {
		return html`
			<mo-dialog size='medium' heading='Campaign' primaryButtonText=${this.parameters?.id ? 'Edit' : 'Create'}>
				${!this.campaign ? html`
					<mo-flex alignItems='center' justifyContent='center' height='100%'>
						<mo-circular-progress indeterminate></mo-circular-progress>
					</mo-flex>
				` : html`
					<mo-flex gap='var(--mo-thickness-xl)'>
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
							?readOnly=${this.campaign.status !== CampaignStatus.Funding}
							.selectedArea=${this.campaign.location}
							@selectedAreaChange=${(e: CustomEvent<GeometryCollection>) => this.campaign.location = e.detail}
						></solid-map>

						<mo-section heading='Donation methods'>
							${this.paymentMethods?.map(pm => html`
								<mo-flex direction='horizontal' gap='20px'>
									<mo-checkbox
										?disabled=${this.campaign?.status === CampaignStatus.Allocation}
										label=${pm.name || pm.identifier}
										?checked=${this.campaign.activatedPaymentMethods.some(dc => dc.identifier === pm.identifier)}
										@change=${(e: CustomEvent<CheckboxValue>) => { this.campaign.activatedPaymentMethods = e.detail === 'checked' ? [...this.campaign.activatedPaymentMethods, new CampaignPaymentMethod(pm.identifier)] : this.campaign.activatedPaymentMethods.filter(dc => dc.identifier !== pm.identifier); this.requestUpdate() }}
									></mo-checkbox>

									<solid-field-allocation-destination width='*'
										paymentMethodIdentifier=${pm.identifier}
										?readonly=${this.campaign?.status === CampaignStatus.Allocation}
										?disabled=${!this.campaign.activatedPaymentMethods.some(dc => dc.identifier === pm.identifier)}
										value=${ifDefined(this.campaign.activatedPaymentMethods.find(p => p.identifier === pm.identifier)?.allocationDestination)}
										@change=${(e: CustomEvent<string>) => this.campaign.activatedPaymentMethods.find(p => p.identifier === pm.identifier)!.allocationDestination = e.detail}
									></solid-field-allocation-destination>
								</mo-flex>
							`)}
						</mo-section>

						<solid-section-campaign-expenditure editable .campaign=${this.campaign}></solid-section-campaign-expenditure>
					</mo-flex>
				`}
			</mo-dialog>
		`
	}

	protected override primaryAction = () => CampaignService.save(this.campaign)
}