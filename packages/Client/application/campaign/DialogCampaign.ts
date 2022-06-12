import { component, DialogComponent, html, state, Task } from '@3mo/model'
import { Campaign, CampaignPaymentMethod, CampaignService, PaymentMethodService } from 'sdk'
import { GeometryCollection } from 'geojson'

@component('solid-dialog-campaign')
export class DialogCampaign extends DialogComponent<undefined | { readonly id: number }, Campaign> {
	private campaignTask = new Task(this, async () => !this.parameters?.id ? new Campaign : await CampaignService.get(this.parameters.id), () => [])

	private paymentMethodsTask = new Task(this, PaymentMethodService.getAll, () => [])
	private get paymentMethods() { return this.paymentMethodsTask.value }

	@state() private includeAllDonationChannels = !this.parameters?.id

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

							<mo-field-date label='Target Date'
								.value=${campaign.targetDate}
								@change=${(e: CustomEvent<MoDate>) => campaign.targetDate = e.detail}
							></mo-field-date>

							<mo-field-text-area label='Description'
								value=${campaign.description ?? ''}
								@change=${(e: CustomEvent<string>) => campaign.description = e.detail}
							></mo-field-text-area>

							<solid-campaign-slider .campaign=${campaign}></solid-campaign-slider>

							<solid-map height='400px'
								.selectedArea=${campaign.location}
								@selectedAreaChange=${(e: CustomEvent<GeometryCollection>) => campaign.location = e.detail}
							></solid-map>

							<mo-section heading='Donation channels'>
								<mo-checkbox slot='action' label='Include all'
									?checked=${this.includeAllDonationChannels}
									@change=${(e: CustomEvent<CheckboxValue>) => this.includeAllDonationChannels = e.detail === 'checked'}
								></mo-checkbox>

								${this.paymentMethods?.map(pm => html`
									<mo-checkbox
										?disabled=${this.includeAllDonationChannels}
										label=${pm.name || pm.identifier}
										?checked=${campaign.activatedPaymentMethods.some(dc => dc.identifier === pm.identifier) || this.includeAllDonationChannels}
										@change=${(e: CustomEvent<CheckboxValue>) => campaign.activatedPaymentMethods = e.detail === 'checked' ? [...campaign.activatedPaymentMethods, new CampaignPaymentMethod(pm.identifier)] : campaign.activatedPaymentMethods.filter(dc => dc.identifier !== pm.identifier)}
									></mo-checkbox>
								`)}
							</mo-section>

							<solid-section-campaign-expenditure editable .campaign=${campaign}></solid-section-campaign-expenditure>
						</mo-flex>
					`
				})}
			</mo-dialog>
		`
	}

	protected override primaryButtonAction = () => CampaignService.save(this.campaignTask.value!, this.includeAllDonationChannels)
}