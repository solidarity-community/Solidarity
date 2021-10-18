import { Account, Model } from "sdk"

export const enum IdentityRoles {
	Admin = 'admin',
	Member = 'member',
}

export interface Identity extends Model {
	accountId: number
	account?: Account
	firstName: string
	lastName: string
	birthDate?: string
}