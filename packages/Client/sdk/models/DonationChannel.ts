import { Model } from 'sdk'

export class DonationChannel extends Model {
	private static readonly nameByIdentifier = new Map<string, string>([
		['BTC_MAINNET', 'Bitcoin Mainnet'],
		['BTC_TESTNET', 'Bitcoin Testnet'],
	])

	constructor(paymentMethodIdentifier?: string) {
		super()
		if (paymentMethodIdentifier) {
			this.paymentMethodIdentifier = paymentMethodIdentifier
		}
	}

	readonly paymentMethodIdentifier!: string

	get name() { return DonationChannel.nameByIdentifier.get(this.paymentMethodIdentifier) }

	get logoSource() { return `/assets/payment-methods/${this.paymentMethodIdentifier.toLowerCase()}.svg` }
}