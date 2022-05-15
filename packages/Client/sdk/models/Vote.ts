import { Model, Account, Validation, model } from 'sdk'

@model('Vote')
export class Vote extends Model {
	validation!: Validation
	account!: Account
	value!: boolean
}