import { Api, AccountProfile } from 'sdk'

export class AccountProfileService {
	static get(id: number) {
		return Api.get<AccountProfile>(`/account/profile/${id}`)
	}

	static getByAccountId(accountId?: number) {
		return Api.get<AccountProfile>(accountId ? `/account/${accountId}/profile` : '/account/profile')
	}

	static createOrUpdate(profile: AccountProfile) {
		return Api.post<AccountProfile>(`/account/profile`, profile)
	}
}