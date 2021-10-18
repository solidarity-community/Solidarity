import { Campaign, Model, Vote } from 'sdk'

export interface Validation extends Model {
	campaign: Campaign
	votes: Array<Vote>
	expiration: string
}