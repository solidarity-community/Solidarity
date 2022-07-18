import { API, AccountProfile } from 'sdk'

export class AccountProfileService {
	static get(id: number) {
		return API.get<AccountProfile>(`/account/profile/${id}`)
	}

	static getByAccountId(accountId?: number) {
		return API.get<AccountProfile>(accountId ? `/account/${accountId}/profile` : '/account/profile')
	}

	static createOrUpdate(profile: AccountProfile) {
		return API.post<AccountProfile>(`/account/profile`, profile)
	}
}