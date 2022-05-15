import { Campaign, model, Model, Vote } from 'sdk'

@model('Validation')
export class Validation extends Model {
	campaign!: Campaign
	votes = new Array<Vote>()
	expiration!: Date
}