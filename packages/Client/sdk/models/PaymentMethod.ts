import { Model } from 'sdk'

export const enum PaymentMethodIdentifier {
	BitcoinMainnet = 'BTC_MAINNET',
	BitcoinTestnet = 'BTC_TESTNET',
}

export class PaymentMethod extends Model {
	private static readonly nameByIdentifier = new Map<string, string>([
		['BTC_MAINNET', 'Bitcoin'],
		['BTC_TESTNET', 'Bitcoin Testnet'],
	])

	constructor(identifier?: PaymentMethodIdentifier) {
		super()
		if (identifier) {
			this.identifier = identifier
		}
	}

	readonly identifier!: PaymentMethodIdentifier

	get name() { return PaymentMethod.nameByIdentifier.get(this.identifier) }

	get logoSource() { return `/assets/payment-methods/${this.identifier.toLowerCase()}.svg` }
}