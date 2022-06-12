import { component, css, DialogComponent, html, state, Task } from '@3mo/model'
import { Campaign, CampaignPaymentMethod, CampaignService, PaymentMethodIdentifier } from 'sdk'

@component('solid-dialog-donate')
export class DialogDonate extends DialogComponent<{ readonly campaign: Campaign }> {
	@state() private selectedPaymentMethodIdentifier?: PaymentMethodIdentifier

	private readonly donationDataByPaymentMethodTask = new Task(this,
		() => CampaignService.getDonationData(this.parameters.campaign.id!),
		() => []
	)

	static override get styles() {
		return css`
			img {
				height: 40px;
			}
		`
	}

	protected override get template() {
		return html`
			<mo-dialog heading='Donate' primaryButtonText=''>
				<mo-flex>
					${this.contentTemplate}
				</mo-flex>
			</mo-dialog>
		`
	}

	private get contentTemplate() {
		const donationChannel = this.parameters.campaign.activatedPaymentMethods
			.find(dc => dc.identifier === this.selectedPaymentMethodIdentifier)
		switch (this.selectedPaymentMethodIdentifier) {
			case PaymentMethodIdentifier.BitcoinMainnet:
				return this.getBitcoinTemplate(donationChannel!)
			case PaymentMethodIdentifier.BitcoinTestnet:
				return this.getBitcoinTemplate(donationChannel!)
			default:
				return this.selectionTemplate
		}
	}

	private get selectionTemplate() {
		return html`
			<mo-list>
				${this.parameters.campaign.activatedPaymentMethods.map(donationChannel => html`
					<mo-list-item @click=${() => this.selectedPaymentMethodIdentifier = donationChannel.identifier}>
						<img slot='graphic' src=${donationChannel.logoSource} />
						${donationChannel.identifier}
					</mo-list-item>
				`)}
			</mo-list>
		`
	}

	private getBitcoinTemplate(paymentMethod: CampaignPaymentMethod) {
		return html`
			<mo-div>Address</mo-div>
			<solid-clipboard-text-block>${this.donationDataByPaymentMethodTask.value?.get(paymentMethod.identifier)}</solid-clipboard-text-block>
		`
	}
}