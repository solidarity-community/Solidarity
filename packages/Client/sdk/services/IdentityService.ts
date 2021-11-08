import { API, Identity } from 'sdk'

export class IdentityService {
	static get(id: number) {
		return API.get<Identity>(`/identity/${id}`)
	}

	static getByAccountId(accountId?: number) {
		return API.get<Identity>(accountId ? `/identity?accountId=${accountId}` : '/identity')
	}

	static createOrUpdate(identity: Identity) {
		return API.post<Identity>(`/identity`, identity)
	}
}