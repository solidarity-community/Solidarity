import { CampaignAllocation, Model, model } from 'sdk'

export const enum CampaignAllocationEntryType { Fund, Refund }

@model('CampaignAllocationEntry')
export class CampaignAllocationEntry extends Model {
	allocationId!: number
	allocation!: CampaignAllocation
	type!: CampaignAllocationEntryType
	paymentMethodIdentifier!: string
	data!: string
	amount!: number
}