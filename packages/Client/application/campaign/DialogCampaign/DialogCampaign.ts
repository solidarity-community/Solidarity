import { EntityDialogComponent } from '@3mo/entity-dialog'
import { component, css, html, ifDefined, style } from '@a11d/lit'
import { Task } from '@lit/task'
import { CampaignPaymentMethod, Campaign, CampaignStatus, PaymentMethod } from 'application'

@component('solid-dialog-campaign')
export class DialogCampaign extends EntityDialogComponent<Campaign> {
	protected override entity = location.hostname === 'localhost' ? Campaign.sample() : new Campaign()
	protected override fetch = Campaign.get
	protected override save = Campaign.save
	protected override delete = undefined

	get campaign() { return this.entity }

	readonly paymentMethodsFetchTask = new Task(this, PaymentMethod.getAll, () => [])
	get paymentMethods() { return this.paymentMethodsFetchTask.value }

	static override get styles() {
		return css`
			mo-entity-dialog {
				background: var(--mo-color-background);
			}
		`
	}

	protected override get template() {
		const { bind } = this.entityBinder
		return html`
			<mo-entity-dialog size='large' heading='Campaign' primaryButtonText=${this.parameters?.id ? 'Edit' : 'Create'}>
				<mo-flex gap='14px'>
					<mo-field-text label='Title' ${bind('title')}></mo-field-text>
					<mo-field-text-area label='Description' ${bind('description')}></mo-field-text-area>

					<mo-card heading='Location' style='--mo-card-body-padding: 0'>
						<solid-map ${style({ height: '400px' })} ?readOnly=${this.campaign.status !== CampaignStatus.Funding} ${bind('location')}></solid-map>
					</mo-card>

					<mo-collapsible-card heading='Media' collapsed>
						<solid-campaign-slider .campaign=${this.campaign}></solid-campaign-slider>
					</mo-collapsible-card>

					<mo-card heading='Donation methods'>
						${this.paymentMethods?.map(pm => html`
							<mo-flex direction='horizontal' gap='20px' alignItems='center'>
								<mo-checkbox
									?disabled=${this.campaign?.status === CampaignStatus.Allocation}
									label=${pm.label}
									?selected=${this.campaign.activatedPaymentMethods.some(dc => dc.identifier === pm.identifier)}
									@change=${(e: CustomEvent<CheckboxSelection>) => { this.campaign.activatedPaymentMethods = e.detail ? [...this.campaign.activatedPaymentMethods, new CampaignPaymentMethod(pm.identifier)] : this.campaign.activatedPaymentMethods.filter(dc => dc.identifier !== pm.identifier); this.requestUpdate() }}
								></mo-checkbox>

								<solid-field-allocation-destination ${style({ flex: 1 })}
									paymentMethodIdentifier=${pm.identifier}
									?readonly=${this.campaign?.status === CampaignStatus.Allocation}
									?disabled=${!this.campaign.activatedPaymentMethods.some(dc => dc.identifier === pm.identifier)}
									value=${ifDefined(this.campaign.activatedPaymentMethods.find(p => p.identifier === pm.identifier)?.allocationDestination)}
									@change=${(e: CustomEvent<string>) => this.campaign.activatedPaymentMethods.find(p => p.identifier === pm.identifier)!.allocationDestination = e.detail}
								></solid-field-allocation-destination>
							</mo-flex>
						`)}
					</mo-card>

					<solid-section-campaign-expenditure editable .campaign=${this.campaign}></solid-section-campaign-expenditure>
				</mo-flex>
			</mo-entity-dialog>
		`
	}

	protected override primaryAction = () => Campaign.save(this.campaign)
}