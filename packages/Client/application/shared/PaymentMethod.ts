import { Model, Api } from 'application'

export const enum PaymentMethodIdentifier {
	BitcoinMainnet = 'BTC_MAINNET',
	BitcoinTestnet = 'BTC_TESTNET',
}

export class PaymentMethod extends Model {
	private static readonly nameByIdentifier = new Map<string, string>([
		['BTC_MAINNET', 'Bitcoin'],
		['BTC_TESTNET', 'Bitcoin Testnet'],
	])

	static getLogoSource(identifier: PaymentMethodIdentifier) {
		return `/assets/payment-methods/${identifier.toLowerCase()}.svg`
	}

	static async getAll() {
		const paymentMethodIdentifiers = await PaymentMethod.getAllIdentifiers()
		return paymentMethodIdentifiers.map(identifier => new PaymentMethod(identifier))
	}

	static getAllIdentifiers() {
		return Api.get<Array<PaymentMethodIdentifier>>('/paymentMethod')
	}

	static isAllocationDestinationValid(identifier: PaymentMethodIdentifier, allocationDestination: string) {
		return Api.get<boolean>(`/paymentMethod/${identifier}/is-allocation-destination-valid/${allocationDestination}`)
	}

	constructor(identifier?: PaymentMethodIdentifier) {
		super()
		if (identifier) {
			this.identifier = identifier
		}
	}

	readonly identifier!: PaymentMethodIdentifier

	get name() { return PaymentMethod.nameByIdentifier.get(this.identifier) }
	get logoSource() { return PaymentMethod.getLogoSource(this.identifier) }

	get label() { return this.name || this.identifier }
}