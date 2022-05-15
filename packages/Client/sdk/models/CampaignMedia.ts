import { model, Model } from 'sdk'

export const enum CampaignMediaType { File, YouTube, Twitch, }

@model('CampaignMedia')
export class CampaignMedia extends Model {
	type!: CampaignMediaType
	uri?: string
}