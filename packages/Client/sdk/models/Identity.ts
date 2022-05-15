import { Account, Model, model } from "sdk"

export const enum IdentityRoles {
	Admin = 'admin',
	Member = 'member',
}

@model('Identity')
export class Identity extends Model {
	accountId?: number
	account?: Account
	firstName?: string
	lastName?: string
	birthDate?: Date
}