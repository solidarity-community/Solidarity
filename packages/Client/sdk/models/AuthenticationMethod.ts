import { Model, Account, AuthenticationMethodType } from 'sdk'

export abstract class AuthenticationMethod extends Model {
	type?: AuthenticationMethodType
	accountId!: number
	account?: Account
	data!: string
}