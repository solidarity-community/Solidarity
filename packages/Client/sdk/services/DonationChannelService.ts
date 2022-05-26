import { DonationChannel, PaymentMethodService } from 'sdk'

export class DonationChannelService {
	static async getAll() {
		const paymentMethodIdentifiers = await PaymentMethodService.getAllIdentifiers()
		return paymentMethodIdentifiers.map(identifier => new DonationChannel(identifier))
	}
}