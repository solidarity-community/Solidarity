import { API } from 'sdk'

export class PaymentMethodService {
	static getAllIdentifiers() {
		return API.get<Array<string>>('/paymentMethod')
	}
}