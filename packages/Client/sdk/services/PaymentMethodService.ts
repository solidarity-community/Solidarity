import { Api, PaymentMethod, PaymentMethodIdentifier } from 'sdk'

export class PaymentMethodService {
	static async getAll() {
		const paymentMethodIdentifiers = await PaymentMethodService.getAllIdentifiers()
		return paymentMethodIdentifiers.map(identifier => new PaymentMethod(identifier))
	}

	static getAllIdentifiers() {
		return Api.get<Array<PaymentMethodIdentifier>>('/paymentMethod')
	}

	static isAllocationDestinationValid(identifier: PaymentMethodIdentifier, allocationDestination: string) {
		return Api.get<boolean>(`/paymentMethod/${identifier}/is-allocation-destination-valid/${allocationDestination}`)
	}
}