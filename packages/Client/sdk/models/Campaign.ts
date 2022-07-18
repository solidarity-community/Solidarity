import { Model, Account, CampaignValidation, CampaignPaymentMethod, CampaignMedia, CampaignExpenditure, model, CampaignMediaType, CampaignAllocation } from 'sdk'
import { GeometryCollection } from 'geojson'

export enum CampaignStatus { Funding, Validation, Allocation }

@model('Campaign')
export class Campaign extends Model {
	title?: string
	description?: string
	location?: GeometryCollection
	creator?: Account
	activatedPaymentMethods = new Array<CampaignPaymentMethod>()
	media = new Array<CampaignMedia>()
	validationId?: number
	validation?: CampaignValidation
	allocation?: CampaignAllocation
	expenditures = new Array<CampaignExpenditure>()

	get status() {
		switch (true) {
			case !this.validation && !this.allocation:
				return CampaignStatus.Funding
			case !!this.validation && !this.allocation:
				return CampaignStatus.Validation
			case !!this.validation && !!this.allocation:
				return CampaignStatus.Allocation
			default:
				throw new Error('Unknown campaign status')
		}
	}

	get coverImageUri() {
		return this.media.find(m => m.type === CampaignMediaType.File)?.uri
	}

	get totalExpenditure() {
		return this.expenditures.map(e => e.totalPrice).reduce((a, acc) => a + acc, 0)
	}
}