import { Model, Account, AuthenticationMethodType } from 'sdk'

export interface AuthenticationMethod extends Model {
	type?: AuthenticationMethodType
	accountId: number
	account?: Account
	data: string
}