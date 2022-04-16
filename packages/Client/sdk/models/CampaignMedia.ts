import { Model } from 'sdk'

export const enum CampaignMediaType { File, YouTube, Twitch, }

export interface CampaignMedia extends Model {
	type: CampaignMediaType
	uri?: string
}