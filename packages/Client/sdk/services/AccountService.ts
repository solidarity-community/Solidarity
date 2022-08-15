import { API, Account } from 'sdk'

export class AccountService {
	static get() {
		return !API.authenticator?.isAuthenticated() ? Promise.resolve(undefined) : API.get<Account>(`/account`)
	}

	static isUsernameAvailable(username: string) {
		return API.get<boolean>(`/account/is-username-available/${username}`)
	}

	static async create(account: Account) {
		const token = await API.post<string>(`/account`, account)
		API.authenticator?.authenticate(token)
	}

	static update(account: Account) {
		return API.put<Account>(`/account`, account)
	}

	static reset(id: number) {
		return API.get<string>(`/account/${id}/reset`)
	}

	static async recover(phrase: string) {
		const token = await API.get<string>(`/recover?phrase=${phrase}`)
		API.authenticator?.authenticate(token)
	}
}