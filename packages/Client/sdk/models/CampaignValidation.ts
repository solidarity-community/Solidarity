import { Campaign, model, Model, CampaignValidationVote } from 'sdk'

@model('Validation')
export class CampaignValidation extends Model {
	campaign!: Campaign
	votes = new Array<CampaignValidationVote>()
	expiration!: Date
}