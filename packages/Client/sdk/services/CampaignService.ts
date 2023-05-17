import { Api, Service, Campaign, PaymentMethodIdentifier, HttpError } from 'sdk'

export class CampaignService extends Service {
	static get(id: number) {
		return Api.get<Campaign>(`/campaign/${id}`)
	}

	static getAll() {
		return Api.get<Array<Campaign>>('/campaign')
	}

	static getBalance(campaignId: number) {
		return Api.get<number>(`/campaign/${campaignId}/balance`)
	}

	static getShare(campaignId: number) {
		return Api.get<number>(`/campaign/${campaignId}/share`)
	}

	static async getDonationData(campaignId: number) {
		try {
			const response = await Api.get<Record<PaymentMethodIdentifier, string>>(`/campaign/${campaignId}/donation-data`)
			return new Map(Object.entries(response)) as Map<PaymentMethodIdentifier, string>
		} catch (error) {
			Service.notifyError((error as HttpError).message)
			throw error
		}
	}

	static async initiateValidation(campaignId: number) {
		await Api.post(`/campaign/${campaignId}/initiate-validation`)
	}

	static async getVote(campaignId: number) {
		const vote = await Api.get<boolean | null>(`/campaign/${campaignId}/vote`)
		return vote ?? undefined
	}

	static getVotes(campaignId: number) {
		return Api.get<{
			readonly balance: number,
			readonly endorsedBalance: number,
			readonly approvalThreshold: number,
		}>(`/campaign/${campaignId}/votes`)
	}

	static async vote(campaignId: number, value: boolean) {
		await Api.post(`/campaign/${campaignId}/vote`, value)
	}

	static save(campaign: Campaign) {
		return campaign.id
			? Api.put<Campaign>(`/campaign/${campaign.id}`, campaign)
			: Api.post<Campaign>('/campaign', campaign)
	}

	static delete(id: number) {
		return this.confirmDeletion(() => Api.delete(`/campaign/${id}`), {
			heading: 'Delete Campaign',
			content: 'Are you sure you want to delete this campaign irreversibly? All donations will be refunded.',
		})
	}
}