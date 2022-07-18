import { CampaignAllocationEntry, CampaignAllocationEntryType, Model, model } from 'sdk'

@model('CampaignAllocation')
export class CampaignAllocation extends Model {
	entries = new Array<CampaignAllocationEntry>()

	get totalFundAmount() {
		return this.entries
			.filter(entry => entry.type === CampaignAllocationEntryType.Fund)
			.reduce((acc, entry) => acc + entry.amount, 0)
	}

	get totalRefundAmount() {
		return this.entries
			.filter(entry => entry.type === CampaignAllocationEntryType.Refund)
			.reduce((acc, entry) => acc + entry.amount, 0)
	}
}