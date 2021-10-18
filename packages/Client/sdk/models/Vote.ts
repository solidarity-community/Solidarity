import { Model, Account, Validation } from 'sdk'

export interface Vote extends Model {
	validation: Validation
	account: Account
	value: boolean
}