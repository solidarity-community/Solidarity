import { Campaign, model } from 'sdk'
import { DonationChannel } from './DonationChannel'

@model('CampaignDonationChannel')
export class CampaignDonationChannel extends DonationChannel {
	readonly campaign!: Campaign
}