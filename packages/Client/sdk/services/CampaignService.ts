import { API, Campaign } from 'sdk'

export class CampaignService {
	static get(id: number) {
		return API.get<Campaign>(`/campaign/${id}`)
	}

	static getAll() {
		return API.get<Array<Campaign>>('/campaign')
	}
}