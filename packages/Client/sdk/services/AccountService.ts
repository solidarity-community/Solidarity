import { API, Account } from 'sdk'

export class AccountService {
	static get() {
		return API.get<Account>(`/account`)
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