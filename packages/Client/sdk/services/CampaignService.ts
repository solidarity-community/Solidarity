import { API, Campaign, PaymentMethodService, CampaignDonationChannel } from 'sdk'

export class CampaignService {
	static get(id: number) {
		return API.get<Campaign>(`/campaign/${id}`)
	}

	static getAll() {
		return API.get<Array<Campaign>>('/campaign')
	}

	static async save(campaign: Campaign, includeAllDonationChannels = false) {
		if (includeAllDonationChannels) {
			const paymentMethodIdentifiers = await PaymentMethodService.getAllIdentifiers()
			campaign.donationChannels = paymentMethodIdentifiers.map(identifier => new CampaignDonationChannel(identifier))
		}
		return campaign.id
			? API.put<Campaign>(`/campaign/${campaign.id}`, campaign)
			: API.post<Campaign>('/campaign', campaign)
	}

	static delete(id: number) {
		return API.delete(`/campaign/${id}`)
	}
}