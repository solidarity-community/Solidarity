import { Api, Account, AuthenticationMethodType } from 'sdk'

export class AccountService {
	static getAuthenticated() {
		return (Api.authenticator?.isAuthenticated() === false)
			? Promise.resolve(undefined)
			: Api.get<Account | undefined>('/account/authenticated')
	}

	static isUsernameAvailable(username: string) {
		return Api.get<boolean>(`/account/is-username-available/${username}`)
	}

	static async create(account: Account) {
		const token = await Api.post<string>(`/account`, account)
		Api.authenticator?.authenticate(token)
	}

	static update(account: Account) {
		return Api.put<Account>(`/account`, account)
	}

	static reset(username: string) {
		return Api.get<string>(`/account/${username}/reset`)
	}

	static async recover(phrase: string) {
		const token = await Api.get<string>(`/account/recover?phrase=${phrase}`)
		Api.authenticator?.authenticate(token)
	}

	static async getAllAuthentications() {
		const isNewByAuthenticationObject = await Api.get<Record<AuthenticationMethodType, boolean>>('/authentication')
		return new Map(Object.entries(isNewByAuthenticationObject)) as Map<AuthenticationMethodType, boolean>
	}

	static async authenticateWithPassword(username: string, password: string) {
		const token = await Api.get<string>(`/authentication/password?username=${username}&password=${password}`)
		Api.authenticator?.authenticate(token)
	}

	static updatePassword(newPassword: string, oldPassword: string) {
		return Api.put(`/authentication/password?newPassword=${newPassword}${oldPassword ? `&oldPassword=${oldPassword}` : ''}`)
	}

	static unauthenticate() {
		Api.authenticator?.unauthenticate()
	}
}