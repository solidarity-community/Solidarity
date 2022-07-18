import { component, css, DialogComponent, html, state, nothing } from '@3mo/modelx'
import { Account, AccountService, Campaign, CampaignService, PaymentMethodIdentifier } from 'sdk'

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
		this.donationDataByPaymentMethod = await CampaignService.getDonationData(this.parameters.campaign.id!)
		if (this.donationDataByPaymentMethod.size === 1) {
			this.selectedPaymentMethodIdentifier = [...this.donationDataByPaymentMethod.keys()][0]
		}
		return this.donationDataByPaymentMethod
	}
	
	private async fetchAccount() {
		this.currentAccount = await AccountService.get()
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
					<mo-div>Donate as <span>${this.currentAccount!.username}</span></mo-div>
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
		const walletAddress = this.donationDataByPaymentMethod?.get(this.selectedPaymentMethod!.identifier)
		return html`
			<mo-flex alignItems='center'>
				<solid-wallet-qr-code protocol='bitcoin'>${walletAddress}</solid-wallet-qr-code>
				<solid-wallet-address protocol='bitcoin'>${walletAddress}</solid-wallet-address>
			</mo-flex>
		`
	}
}