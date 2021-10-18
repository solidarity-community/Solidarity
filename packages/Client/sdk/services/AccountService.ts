import { API, Account } from 'sdk'

export class AccountService {
	static get() {
		return API.get<Account>(`/account`)
	}

	static async create(account: Account) {
		API.token = await API.post<string>(`/account`, account)
	}

	static update(account: Account) {
		return API.put<Account>(`/account`, account)
	}

	static reset(id: number) {
		return API.get<string>(`/account/${id}/reset`)
	}

	static async recover(phrase: string) {
		API.token = await API.get<string>(`/recover?phrase=${phrase}`)
	}
}