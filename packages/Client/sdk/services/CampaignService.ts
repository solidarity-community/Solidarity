import { API, Campaign, PaymentMethodService, CampaignPaymentMethod } from 'sdk'

export class CampaignService {
	static get(id: number) {
		return API.get<Campaign>(`/campaign/${id}`)
	}

	static getAll() {
		return API.get<Array<Campaign>>('/campaign')
	}

	static getBalance(campaignId: number) {
		return API.get<number>(`/campaign/${campaignId}/balance`)
	}

	static async getDonationData(campaignId: number) {
		const response = await API.get<Record<string, string>>(`/campaign/${campaignId}/donation-data`)
		return new Map(Object.entries(response))
	}

	static async save(campaign: Campaign, includeAllDonationChannels = false) {
		if (includeAllDonationChannels) {
			const paymentMethodIdentifiers = await PaymentMethodService.getAllIdentifiers()
			campaign.activatedPaymentMethods = paymentMethodIdentifiers.map(identifier => new CampaignPaymentMethod(identifier))
		}
		return campaign.id
			? API.put<Campaign>(`/campaign/${campaign.id}`, campaign)
			: API.post<Campaign>('/campaign', campaign)
	}

	static delete(id: number) {
		return API.delete(`/campaign/${id}`)
	}
}