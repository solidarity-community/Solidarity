import { Account, Model, model } from "sdk"

@model('AccountProfile')
export class AccountProfile extends Model {
	constructor(accountId?: number) { 
		super()
		this.accountId = accountId
	}

	accountId?: number
	account?: Account
	firstName?: string
	lastName?: string
	birthDate?: Date
}