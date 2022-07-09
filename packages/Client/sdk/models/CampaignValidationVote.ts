import { Model, Account, CampaignValidation, model } from 'sdk'

@model('Vote')
export class CampaignValidationVote extends Model {
	validation!: CampaignValidation
	account!: Account
	value!: boolean
}