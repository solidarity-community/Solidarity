import { type GeometryCollection } from 'geojson'
import { NotificationComponent } from '@a11d/lit-application'
import { Model, model, Api, PaymentMethod, PaymentMethodIdentifier, type Account, type HttpError } from 'application'
import { DialogDeletion } from '@3mo/standard-dialogs'

export enum CampaignStatus { Funding, Validation, Allocation }

@model('Campaign')
export class Campaign extends Model {
	static sample() {
		return new Campaign({
			title: 'Clean Alster Initiative',
			description: 'This initiative focuses on removing litter, promoting environmental awareness, and fostering community involvement.',
			activatedPaymentMethods: [new CampaignPaymentMethod(PaymentMethodIdentifier.BitcoinTestnet)],
			expenditures: [
				new CampaignExpenditure({ quantity: 10, unitPrice: 40, name: 'Litter collection tools' }),
				new CampaignExpenditure({ quantity: 16, unitPrice: 100, name: 'Recycling bins for sorting waste' }),
				new CampaignExpenditure({ quantity: 30, unitPrice: 1, name: 'Refreshments for volunteers' }),
				new CampaignExpenditure({ quantity: 30, unitPrice: 50, name: 'Safety equipment' })
			],
			location: {
				type: 'GeometryCollection',
				geometries: [
					{
						type: 'Polygon',
						coordinates: [[[9.994769, 53.552246], [10.00067, 53.555649], [10.000992, 53.55686], [10.001056, 53.557498], [10.004747, 53.557612], [10.010197, 53.55969], [10.016699, 53.563732], [10.01627, 53.567861], [10.010176, 53.569951], [10.006485, 53.575405], [10.003395, 53.579379], [9.999533, 53.580602], [9.998245, 53.579278], [10.00185, 53.575812], [10.002623, 53.572296], [10.001163, 53.566026], [9.995241, 53.559093], [9.99155, 53.554928], [9.994769, 53.552246]]]
					}
				]
			}
		})
	}

	static get(id: number) {
		return Api.get<Campaign>(`/campaign/${id}`)
	}

	static getAll() {
		return Api.get<Array<Campaign>>('/campaign')
	}

	static getBalance(campaignId: number) {
		return Api.get<number>(`/campaign/${campaignId}/balance`)
	}

	static getShare(campaignId: number) {
		return Api.get<number>(`/campaign/${campaignId}/share`)
	}

	static async getDonationData(campaignId: number) {
		try {
			const response = await Api.get<Record<PaymentMethodIdentifier, string>>(`/campaign/${campaignId}/donation-data`)
			return new Map(Object.entries(response)) as Map<PaymentMethodIdentifier, string>
		} catch (error) {
			NotificationComponent.notifyError((error as HttpError).message)
			throw error
		}
	}

	static async initiateValidation(campaignId: number) {
		await Api.post(`/campaign/${campaignId}/initiate-validation`)
	}

	static async getVote(campaignId: number) {
		const vote = await Api.get<boolean | null>(`/campaign/${campaignId}/vote`)
		return vote ?? undefined
	}

	static getVotes(campaignId: number) {
		return Api.get<{
			readonly balance: number
			readonly endorsedBalance: number
			readonly approvalThreshold: number
		}>(`/campaign/${campaignId}/votes`)
	}

	static async vote(campaignId: number, value: boolean) {
		await Api.post(`/campaign/${campaignId}/vote`, value)
	}

	static save(campaign: Campaign) {
		return campaign.id
			? Api.put<Campaign>('/campaign', campaign)
			: Api.post<Campaign>('/campaign', campaign)
	}

	static delete(id: number) {
		return new DialogDeletion({
			content: 'Are you sure you want to delete this campaign irreversibly? All donations will be refunded.',
			deletionAction: () => Api.delete(`/campaign/${id}`)
		}).confirm()
	}

	constructor(init?: Partial<Campaign>) {
		super()
		Object.assign(this, init)
	}

	readonly creatorId!: number
	title?: string
	description?: string
	location?: GeometryCollection
	creator?: Account
	activatedPaymentMethods = new Array<CampaignPaymentMethod>()
	media = new Array<CampaignMedia>()
	validationId?: number
	validation?: CampaignValidation
	allocation?: CampaignAllocation
	expenditures = new Array<CampaignExpenditure>()

	get status() {
		switch (true) {
			case !this.validation && !this.allocation:
				return CampaignStatus.Funding
			case !!this.validation && !this.allocation:
				return CampaignStatus.Validation
			case !!this.validation && !!this.allocation:
				return CampaignStatus.Allocation
			default:
				throw new Error('Unknown campaign status')
		}
	}

	get coverImageUri() {
		return this.media.find(m => m.type === CampaignMediaType.File)?.uri
	}

	get totalExpenditure() {
		return this.expenditures.map(e => e.totalPrice).reduce((a, acc) => a + acc, 0)
	}
}

@model('CampaignAllocation')
export class CampaignAllocation extends Model {
	entries = new Array<CampaignAllocationEntry>()

	get totalFundAmount() {
		return this.entries
			.filter(entry => entry.type === CampaignAllocationEntryType.Fund)
			.reduce((acc, entry) => acc + entry.amount, 0)
	}

	get totalRefundAmount() {
		return this.entries
			.filter(entry => entry.type === CampaignAllocationEntryType.Refund)
			.reduce((acc, entry) => acc + entry.amount, 0)
	}
}

export const enum CampaignAllocationEntryType { Fund, Refund }

@model('CampaignAllocationEntry')
export class CampaignAllocationEntry extends Model {
	allocationId!: number
	allocation!: CampaignAllocation
	type!: CampaignAllocationEntryType
	paymentMethodIdentifier!: string
	data!: string
	amount!: number
}

@model('CampaignExpenditure')
export class CampaignExpenditure extends Model {
	quantity = 1
	name = ''
	unitPrice = 0

	get totalPrice() { return this.quantity * this.unitPrice }

	constructor(init?: Partial<CampaignExpenditure>) {
		super()
		Object.assign(this, init)
	}
}

export const enum CampaignMediaType { File, YouTube, Twitch, }

@model('CampaignMedia')
export class CampaignMedia extends Model {
	static get(id: number) {
		return Api.get<CampaignMedia>(`/media/${id}`)
	}

	static getAll() {
		return Api.get<Array<CampaignMedia>>('/media')
	}

	static extractUriByType(content: string, type: CampaignMediaType) {
		switch (type) {
			case CampaignMediaType.File:
				return content
			case CampaignMediaType.YouTube:
				return CampaignMedia.extractYouTubeVideoId(content)
			case CampaignMediaType.Twitch:
				return CampaignMedia.extractTwitchVideoId(content)
		}
	}

	private static extractYouTubeVideoId(content: string) {
		const regex = /^.*(youtu\.be\/|v\/|u\/\w\/|embed\/|watch\?v=|\&v=)([^#\&\?]*).*/
		const videoId = content.match(regex)?.[2] as string | undefined
		if (videoId?.length !== 11) {
			throw new Error('Invalid YouTube URI')
		}
		return videoId
	}

	private static extractTwitchVideoId(content: string) {
		const regex = /^.*(twitch\.tv\/videos\/)([^#\&\?]*).*/
		const videoId = content.match(regex)?.[2] as string | undefined
		if (videoId?.length !== 10) {
			throw new Error('Invalid Twitch URI')
		}
		return videoId
	}

	static save(media: CampaignMedia) {
		return Api.post<CampaignMedia>('/media', media)
	}

	static delete(id: number) {
		return Api.delete(`/media/${id}`)
	}

	type!: CampaignMediaType
	uri?: string
}

@model('CampaignPaymentMethod')
export class CampaignPaymentMethod extends PaymentMethod {
	override readonly identifier!: PaymentMethodIdentifier
	readonly campaign!: Campaign
	allocationDestination?: string
}

@model('CampaignValidation')
export class CampaignValidation extends Model {
	campaign!: Campaign
	votes = new Array<CampaignValidationVote>()
	expiration!: Date
}

@model('CampaignValidationVote')
export class CampaignValidationVote extends Model {
	validation!: CampaignValidation
	account!: Account
	value!: boolean
}