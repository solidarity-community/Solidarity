import { component, css, DialogComponent, html, state, Task, nothing } from '@3mo/model'
import { AccountService, Campaign, CampaignService, PaymentMethodIdentifier } from 'sdk'

@component('solid-dialog-donate')
export class DialogDonate extends DialogComponent<{ readonly campaign: Campaign }> {
	private readonly fetchDonationDataByPaymentMethodTask = new Task(this, this.fetchDonationDataByPaymentMethod.bind(this), () => [])
	private readonly fetchCurrentAccountTask = new Task(this, AccountService.get, () => [])

	@state() private selectedPaymentMethodIdentifier?: PaymentMethodIdentifier

	private get selectedPaymentMethod() {
		return this.parameters.campaign.activatedPaymentMethods
			.find(dc => dc.identifier === this.selectedPaymentMethodIdentifier)
	}

	private get isPublicDonation() {
		return !this.fetchCurrentAccountTask.value
	}

	private async fetchDonationDataByPaymentMethod() {
		const donationData = await CampaignService.getDonationData(this.parameters.campaign.id!)
		if (donationData.size === 1) {
			this.selectedPaymentMethodIdentifier = [...donationData.keys()][0]
		}
		return donationData
	}

	static override get styles() {
		return css`
			img {
				height: 40px;
			}

			mo-flex[slot=header] {
				font-size: small;
			}

			ol, ul {
				line-height: 1.75;
			}

			ul {
				list-style-type: none;
				padding: 0px;
			}

			li {
				margin-top: 20px;
			}

			li::marker {
				color: var(--mo-accent);
				font-weight: bold;
				font-size: larger;
			}

			li span {
				font-size: larger;
			}

			li span, mo-flex[slot=header] span {
				color: var(--mo-accent);
				font-weight: bold;
			}

			mo-flex[slot=header] mo-div {
				background: var(--mo-accent-gradient-transparent);
				padding: 3px 6px;
				border-radius: 0 6px 6px 0;
			}

			mo-flex[slot=header] img {
				height: 30px;
				margin-right: -4px;
				z-index: 1;
			}
		`
	}

	protected override get template() {
		return html`
			<mo-dialog heading='Donate' primaryButtonText=''>
				${!this.selectedPaymentMethodIdentifier ? this.selectionTemplate : this.donationTemplate }
			</mo-dialog>
		`
	}

	private get selectionTemplate() {
		return html`
			<mo-list>
				${this.parameters.campaign.activatedPaymentMethods.map(paymentMethod => html`
					<mo-list-item @click=${() => this.selectedPaymentMethodIdentifier = paymentMethod.identifier}>
						<img slot='graphic' src=${paymentMethod.logoSource} />
						${paymentMethod.name}
					</mo-list-item>
				`)}
			</mo-list>
		`
	}

	private get donationTemplate() {
		const listItemsTemplate = html`
			<li>
				<mo-flex gap='10px'>
					<mo-div><span>Donate.</span> Send <b>${this.selectedPaymentMethod?.name}</b> funds to this campaign via the following information:</mo-div>
					${this.paymentMethodTemplate}
				</mo-flex>
			</li>
			${this.isPublicDonation ? nothing : html`
				<li>
					<span>Validate.</span> Use provided media, description, location, etc. to validate this campaign's activities.
				</li>
				<li>
					<span>Vote.</span> Endorse or Oppose the the integrity of this campaign to decide on allocating or refunding donations.
				</li>
			`}
		`
		return html`
			<mo-flex slot='header' alignItems='center' direction='horizontal'>
				<img src=${this.selectedPaymentMethod!.logoSource} />
				${this.isPublicDonation ? html`
					<mo-div><span>Public</span> donation</mo-div>
				` : html`
					<mo-div>Donate as <span>${this.fetchCurrentAccountTask.value!.username}</span></mo-div>
				`}
			</mo-flex>

			${this.isPublicDonation ? html`<ul>${listItemsTemplate}</ul>` : html`<ol>${listItemsTemplate}</ol>`}
		`
	}

	private get paymentMethodTemplate() {
		switch (this.selectedPaymentMethodIdentifier) {
			case PaymentMethodIdentifier.BitcoinMainnet:
			case PaymentMethodIdentifier.BitcoinTestnet:
				return this.bitcoinTemplate
			default:
				return nothing
		}
	}

	private get bitcoinTemplate() {
		const walletAddress = this.fetchDonationDataByPaymentMethodTask.value?.get(this.selectedPaymentMethod!.identifier)
		return html`
			<mo-flex alignItems='center'>
				<solid-wallet-qr-code protocol='bitcoin'>${walletAddress}</solid-wallet-qr-code>
				<solid-wallet-address protocol='bitcoin'>${walletAddress}</solid-wallet-address>
			</mo-flex>
		`
	}
}