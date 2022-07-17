import { component, DialogComponent, html, ifDefined, state, Task } from '@3mo/modelx'
import { Campaign, CampaignPaymentMethod, CampaignService, CampaignStatus, PaymentMethodService } from 'sdk'
import { GeometryCollection } from 'geojson'

@component('solid-dialog-campaign')
export class DialogCampaign extends DialogComponent<undefined | { readonly id: number }, Campaign> {
	private campaignTask = new Task(this, async () => !this.parameters?.id ? new Campaign : await CampaignService.get(this.parameters.id), () => [])
	private get campaign() { return this.campaignTask.value }

	private paymentMethodsTask = new Task(this, PaymentMethodService.getAll, () => [])
	private get paymentMethods() { return this.paymentMethodsTask.value }

	protected override get template() {
		return html`
			<mo-dialog size='medium' heading='Campaign' primaryButtonText=${this.parameters?.id ? 'Edit' : 'Create'}>
				${this.campaignTask.render({
					pending: () => html`
						<mo-flex alignItems='center' justifyContent='center' height='100%'>
							<mo-circular-progress indeterminate></mo-circular-progress>
						</mo-flex>
					`,
					complete: campaign => html`
						<mo-flex gap='var(--mo-thickness-xl)'>
							<mo-field-text label='Title'
								value=${campaign.title ?? ''}
								@change=${(e: CustomEvent<string>) => campaign.title = e.detail}
							></mo-field-text>

							<mo-field-text-area label='Description'
								value=${campaign.description ?? ''}
								@change=${(e: CustomEvent<string>) => campaign.description = e.detail}
							></mo-field-text-area>

							<solid-campaign-slider .campaign=${campaign}></solid-campaign-slider>

							<solid-map height='400px'
								?readOnly=${campaign.status !== CampaignStatus.Funding}
								.selectedArea=${campaign.location}
								@selectedAreaChange=${(e: CustomEvent<GeometryCollection>) => campaign.location = e.detail}
							></solid-map>

							<mo-section heading='Donation methods'>
								${this.paymentMethods?.map(pm => html`
									<mo-flex direction='horizontal' gap='20px'>
										<mo-checkbox
											?disabled=${this.campaign?.status === CampaignStatus.Allocation}
											label=${pm.name || pm.identifier}
											?checked=${campaign.activatedPaymentMethods.some(dc => dc.identifier === pm.identifier)}
											@change=${(e: CustomEvent<CheckboxValue>) => { campaign.activatedPaymentMethods = e.detail === 'checked' ? [...campaign.activatedPaymentMethods, new CampaignPaymentMethod(pm.identifier)] : campaign.activatedPaymentMethods.filter(dc => dc.identifier !== pm.identifier); this.requestUpdate() }}
										></mo-checkbox>

										<solid-field-allocation-destination width='*'
											paymentMethodIdentifier=${pm.identifier}
											?readonly=${this.campaign?.status === CampaignStatus.Allocation}
											?disabled=${!campaign.activatedPaymentMethods.some(dc => dc.identifier === pm.identifier)}
											value=${ifDefined(campaign.activatedPaymentMethods.find(p => p.identifier === pm.identifier)?.allocationDestination)}
											@change=${(e: CustomEvent<string>) => campaign.activatedPaymentMethods.find(p => p.identifier === pm.identifier)!.allocationDestination = e.detail}
										></solid-field-allocation-destination>
									</mo-flex>
								`)}
							</mo-section>

							<solid-section-campaign-expenditure editable .campaign=${campaign}></solid-section-campaign-expenditure>
						</mo-flex>
					`
				})}
			</mo-dialog>
		`
	}

	protected override primaryButtonAction = () => CampaignService.save(this.campaignTask.value!)
}