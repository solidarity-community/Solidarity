import { Model, PasswordAuthentication, Identity, Campaign, Vote } from 'sdk'

export interface Account extends Model {
	username?: string
	publicKey?: string
	authenticationMethods?: Array<PasswordAuthentication>
	campaigns?: Array<Campaign>
	votes?: Array<Vote>
}