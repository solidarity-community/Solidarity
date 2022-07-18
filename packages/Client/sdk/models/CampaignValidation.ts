import { Campaign, model, Model, CampaignValidationVote } from 'sdk'

@model('CampaignValidation')
export class CampaignValidation extends Model {
	campaign!: Campaign
	votes = new Array<CampaignValidationVote>()
	expiration!: Date
}