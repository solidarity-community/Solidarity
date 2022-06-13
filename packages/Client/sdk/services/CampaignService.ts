import { API, Campaign, PaymentMethodService, CampaignPaymentMethod, PaymentMethodIdentifier } from 'sdk'

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

	static getShare(campaignId: number) {
		return API.get<number>(`/campaign/${campaignId}/share`)
	}

	static async getDonationData(campaignId: number) {
		const response = await API.get<Record<PaymentMethodIdentifier, string>>(`/campaign/${campaignId}/donation-data`)
		return new Map(Object.entries(response)) as Map<PaymentMethodIdentifier, string>
	}

	static async declareAllocationPhase(campaignId: number) {
		await API.post(`/campaign/${campaignId}/declare-allocation-phase`)
	}

	static async allocate(campaignId: number, destinationByPaymentMethodIdentifier: Map<string, string>) {
		await API.post(`/campaign/${campaignId}/allocate`, destinationByPaymentMethodIdentifier)
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