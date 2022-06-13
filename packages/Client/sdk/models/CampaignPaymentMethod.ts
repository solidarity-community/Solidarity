import { Campaign, model } from 'sdk'
import { PaymentMethod, PaymentMethodIdentifier } from './PaymentMethod'

@model('CampaignPaymentMethod')
export class CampaignPaymentMethod extends PaymentMethod {
	override readonly identifier!: PaymentMethodIdentifier
	readonly campaign!: Campaign
}