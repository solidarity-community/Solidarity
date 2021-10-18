import { Model, PasswordAuthentication, Identity, Campaign, Vote } from 'sdk'

export interface Account extends Model {
	username?: string
	publicKey?: string
	passwordAuthentication?: PasswordAuthentication
	identity?: Identity
	campaigns?: Array<Campaign>
	votes?: Array<Vote>
}