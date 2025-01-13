import { Api, Model, model, type Campaign, type CampaignValidationVote } from 'application'
import { sha256 } from 'js-sha256'

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
		this.setToken(token)
	}

	static update(account: Account) {
		return Api.put<Account>('/account', account)
	}

	static reset(username: string) {
		return Api.get<string>(`/account/${username}/reset`)
	}

	static async recover(phrase: string) {
		const token = await Api.get<string>(`/account/recover?phrase=${phrase}`)
		this.setToken(token)
	}

	static async authenticate(username: string, password: string) {
		const token = await Api.post<string>('/account/authenticate', { username, password })
		this.setToken(token)
	}

	private static setToken(token: string) {
		Api.authenticator?.authenticate(token)
		window.location.reload()
	}

	static unauthenticate() {
		Api.authenticator?.unauthenticate()
		window.location.reload()
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
	plainPassword = ''
	hashPassword() {
		this.password = !this.plainPassword ? undefined : sha256(this.plainPassword)
	}

	birthDate?: Date
	campaigns?: Array<Campaign>
	votes?: Array<CampaignValidationVote>
}