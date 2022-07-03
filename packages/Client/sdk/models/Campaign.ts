import { Model, Account, Validation, CampaignPaymentMethod, CampaignMedia, CampaignExpenditure, model, CampaignMediaType } from 'sdk'
import { GeometryCollection } from 'geojson'

export enum CampaignStatus { Funding, Allocation, Complete }

@model('Campaign')
export class Campaign extends Model {
	title?: string
	description?: string
	location?: GeometryCollection
	creator?: Account
	targetAllocationDate!: MoDate
	completionDate?: MoDate
	allocationDate?: MoDate
	activatedPaymentMethods = new Array<CampaignPaymentMethod>()
	media = new Array<CampaignMedia>()
	validationId?: number
	validation?: Validation
	expenditures = new Array<CampaignExpenditure>()

	get status() {
		switch (true) {
			case !!this.allocationDate && !this.completionDate:
				return CampaignStatus.Allocation
			case !!this.allocationDate && !!this.completionDate:
				return CampaignStatus.Complete
			default:
				return CampaignStatus.Funding
		}
	}

	get coverImageUri() {
		return this.media.find(m => m.type === CampaignMediaType.File)?.uri
	}

	get totalExpenditure() {
		return this.expenditures.map(e => e.totalPrice).reduce((a, acc) => a + acc, 0)
	}

	get remainingTimePercentage() {
		const now = new MoDate
		const untilTargetDate = now.until(this.targetAllocationDate)
		const sinceCreation = now.since(this.creation)
		return untilTargetDate.milliseconds /
			(sinceCreation.milliseconds + untilTargetDate.milliseconds)
	}
}