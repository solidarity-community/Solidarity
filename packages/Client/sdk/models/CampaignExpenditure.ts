import { Model, model } from 'sdk'

@model('CampaignExpenditure')
export class CampaignExpenditure extends Model {
	quantity = 1
	name = ''
	unitPrice = 0

	get totalPrice() { return this.quantity * this.unitPrice }
}