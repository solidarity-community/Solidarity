import { Model, Account, Validation, CampaignPaymentMethod, CampaignMedia, CampaignExpenditure, model, CampaignMediaType } from 'sdk'
import { GeometryCollection } from 'geojson'

@model('Campaign')
export class Campaign extends Model {
	title?: string
	description?: string
	location?: GeometryCollection
	creator?: Account
	targetDate!: MoDate
	completion?: MoDate
	activatedPaymentMethods = new Array<CampaignPaymentMethod>()
	media = new Array<CampaignMedia>()
	validationId?: number
	validation?: Validation
	expenditures = new Array<CampaignExpenditure>()

	get coverImageUri() {
		return this.media.find(m => m.type === CampaignMediaType.File)?.uri
	}

	get totalExpenditure() {
		return this.expenditures.map(e => e.totalPrice).reduce((a, acc) => a + acc, 0)
	}

	get remainingTimePercentage() {
		const now = new MoDate
		const untilTargetDate = now.until(this.targetDate)
		const sinceCreation = now.since(this.creation)
		return untilTargetDate.milliseconds /
			(sinceCreation.milliseconds + untilTargetDate.milliseconds)
	}
}