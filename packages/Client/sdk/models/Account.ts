import { Model, model, PasswordAuthentication, Campaign, Vote } from 'sdk'

@model('Account')
export class Account extends Model {
	username?: string
	publicKey?: string
	authenticationMethods?: Array<PasswordAuthentication>
	campaigns?: Array<Campaign>
	votes?: Array<Vote>
}