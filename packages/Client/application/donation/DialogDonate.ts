import { component, css, html, state } from '@a11d/lit'
import { DialogComponent } from '@a11d/lit-application'
import { Account, Campaign, PaymentMethodIdentifier } from 'application'

@component('solid-dialog-donate')
export class DialogDonate extends DialogComponent<{ readonly campaign: Campaign }> {
	override async confirm(...parameters: Parameters<DialogComponent<{ readonly campaign: Campaign }>['confirm']>) {
		await this.fetchDonationDataByPaymentMethod()
		await this.fetchAccount()
		return super.confirm(...parameters)
	}

	@state() private donationDataByPaymentMethod?: Map<PaymentMethodIdentifier, string>
	@state() private selectedPaymentMethodIdentifier?: PaymentMethodIdentifier
	@state() private currentAccount?: Account

	private async fetchDonationDataByPaymentMethod() {
		this.donationDataByPaymentMethod = await Campaign.getDonationData(this.parameters.campaign.id!)
		if (this.donationDataByPaymentMethod.size === 1) {
			this.selectedPaymentMethodIdentifier = [...this.donationDataByPaymentMethod.keys()][0]
		}
		return this.donationDataByPaymentMethod
	}

	private async fetchAccount() {
		this.currentAccount = await Account.getAuthenticated()
	}

	private get selectedPaymentMethod() {
		return this.parameters.campaign.activatedPaymentMethods
			.find(dc => dc.identifier === this.selectedPaymentMethodIdentifier)
	}

	private get isPublicDonation() {
		return !this.currentAccount
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
				color: var(--mo-color-accent);
				font-weight: bold;
				font-size: larger;
			}

			li span {
				font-size: larger;
			}

			li span, mo-flex[slot=header] span {
				color: var(--mo-color-accent);
				font-weight: bold;
			}

			mo-flex[slot=header] div {
				background: var(--mo-color-accent-transparent);
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
			<mo-dialog heading='Donate'>
				${!this.selectedPaymentMethodIdentifier ? this.selectionTemplate : this.donationTemplate}
			</mo-dialog>
		`
	}

	private get selectionTemplate() {
		return html`
			<mo-list>
				${this.parameters.campaign.activatedPaymentMethods.map(paymentMethod => html`
					<mo-list-item @click=${() => this.selectedPaymentMethodIdentifier = paymentMethod.identifier}>
						<img width='30px' src=${paymentMethod.logoSource} />
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
					<div><span>Donate.</span> Send <b>${this.selectedPaymentMethod?.name}</b> funds to this campaign via the following information:</div>
					${this.paymentMethodTemplate}
				</mo-flex>
			</li>
			${this.isPublicDonation ? html.nothing : html`
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
					<div><span>Public</span> donation</div>
				` : html`
					<div>Donate as <span>${this.currentAccount!.username}</span></div>
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
				return html.nothing
		}
	}

	private get bitcoinTemplate() {
		const walletAddress = this.donationDataByPaymentMethod?.get(this.selectedPaymentMethod!.identifier)
		return html`
			<mo-flex alignItems='center'>
				<solid-wallet-qr-code protocol='bitcoin'>${walletAddress}</solid-wallet-qr-code>
				<solid-wallet-address protocol='bitcoin'>${walletAddress}</solid-wallet-address>
			</mo-flex>
		`
	}
}