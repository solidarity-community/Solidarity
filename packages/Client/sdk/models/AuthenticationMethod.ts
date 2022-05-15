import { Model, Account, AuthenticationMethodType, model } from 'sdk'

export abstract class AuthenticationMethod extends Model {
	type?: AuthenticationMethodType
	accountId!: number
	account?: Account
	data!: string
}