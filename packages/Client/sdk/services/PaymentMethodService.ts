import { API, PaymentMethod, PaymentMethodIdentifier } from 'sdk'

export class PaymentMethodService {
	static async getAll() {
		const paymentMethodIdentifiers = await PaymentMethodService.getAllIdentifiers()
		return paymentMethodIdentifiers.map(identifier => new PaymentMethod(identifier))
	}

	static getAllIdentifiers() {
		return API.get<Array<PaymentMethodIdentifier>>('/paymentMethod')
	}
}