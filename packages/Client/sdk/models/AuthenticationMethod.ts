import { Model, Account } from 'sdk'

export interface AuthenticationMethod extends Model {
	accountId: number
	account?: Account
	data: string
}