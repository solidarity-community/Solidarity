import { Api, Model, model, type Campaign, type CampaignValidationVote } from 'application'

@model('Account')
export class Account extends Model {
	static getAuthenticated() {
		return (Api.authenticator?.isAuthenticated() === false)
			? Promise.resolve(undefined)
			: Api.get<Account | undefined>('/account/authenticated')
	}

	static async isAuthenticated() {
		// eslint-disable-next-line no-return-await
		return Account.isAuthenticatedClientSide() && await Api.get<boolean>('/account/is-authenticated')
	}

	static isAuthenticatedClientSide() {
		return !!Api.authenticator?.isAuthenticated()
	}

	static isUsernameAvailable(username: string) {
		return Api.get<boolean>(`/account/is-username-available/${username}`)
	}

	static async create(account: Account) {
		const token = await Api.post<string>('/account', account)
		Api.authenticator?.authenticate(token)
	}

	static update(account: Account) {
		return Api.put<Account>('/account', account)
	}

	static reset(username: string) {
		return Api.get<string>(`/account/${username}/reset`)
	}

	static async recover(phrase: string) {
		const token = await Api.get<string>(`/account/recover?phrase=${phrase}`)
		Api.authenticator?.authenticate(token)
	}

	static async authenticate(username: string, password: string) {
		const token = await Api.get<string>(`/authentication?username=${username}&password=${password}`)
		Api.authenticator?.authenticate(token)
	}

	static updatePassword(newPassword: string, oldPassword: string) {
		return Api.put(`/authentication/password?newPassword=${newPassword}${oldPassword ? `&oldPassword=${oldPassword}` : ''}`)
	}

	static unauthenticate() {
		Api.authenticator?.unauthenticate()
	}

	constructor(init?: Partial<Account>) {
		super()
		Object.assign(this, init)
	}

	publicKey?: string

	name?: string
	username?: string

	get nameOrUsername() {
		return this.name?.trim() || this.username
	}

	password?: string
	birthDate?: Date
	campaigns?: Array<Campaign>
	votes?: Array<CampaignValidationVote>
}