import { Model, Account, CampaignValidation, model } from 'sdk'

@model('CampaignValidationVote')
export class CampaignValidationVote extends Model {
	validation!: CampaignValidation
	account!: Account
	value!: boolean
}