import { Model } from './Model'

export interface CampaignExpenditure extends Model {
	quantity: number
	name: string
	unitPrice: number
}