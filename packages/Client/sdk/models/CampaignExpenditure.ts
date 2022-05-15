import { Model, model } from 'sdk'

@model('CampaignExpenditure')
export class CampaignExpenditure extends Model {
	quantity!: number
	name!: string
	unitPrice!: number

	get totalPrice() { return this.quantity * this.unitPrice }
}