import { component, nothing, html, state, style } from '@a11d/lit'
import { PageComponent, PageError, route, HttpErrorCode } from '@a11d/lit-application'
import { requiresAuthentication } from '@a11d/lit-application-authentication'
import { Account, AccountService, AuthenticationMethodType, AccountProfile, AccountProfileService } from 'sdk'
import { DialogAccountProfile, DialogAuthenticationMethods, DialogAuthentication } from 'application'

@requiresAuthentication()
@route('/account')
@component('solid-page-account')
export class PageAccount extends PageComponent {
	@state() private account?: Account
	@state() private profile?: AccountProfile
	@state() private isNewByAuthenticationMethod = new Map<AuthenticationMethodType, boolean>()

	protected override async initialized() {
		await this.redirectToErrorPageIfNotAuthenticated()
		await this.fetchData()
	}

	private async fetchData() {
		await Promise.all([
			this.fetchAccount(),
			this.fetchAuthenticationMethods(),
			this.fetchProfile()
		])
	}

	private async redirectToErrorPageIfNotAuthenticated() {
		const authenticatedAccount = await AccountService.getAuthenticated()
		if (!authenticatedAccount) {
			new PageError({
				error: HttpErrorCode.Unauthorized,
				message: 'You are not authenticated into your account.'
			}).navigate()
		}
	}

	private async fetchAccount() {
		this.account = await AccountService.getAuthenticated()
	}

	private async fetchAuthenticationMethods() {
		this.isNewByAuthenticationMethod = await AccountService.getAllAuthentications()
	}

	private async fetchProfile() {
		this.profile = await AccountProfileService.getByAccountId()
	}

	protected override get template() {
		return html`
			<mo-page heading='Account'>
				<mo-flex gap='24px' ${style({ height: '100%', width: '100%', maxWidth: '1028px', margin: 'auto' })}>
					${this.topBarTemplate}
					<mo-grid gap='16px' ${style({ flex: '1' })}>
						${this.activitiesTemplate}
						${this.securityTemplate}
					</mo-grid>
				</mo-flex>
			</mo-page>
		`
	}

	private get topBarTemplate() {
		const name = `${this.profile?.firstName ?? ''} ${this.profile?.lastName ?? ''}`
		const username = this.account?.username
		const hasName = !!name.trim()
		return html`
			<mo-flex direction='horizontal' justifyContent='space-between' alignItems='center'>
				<mo-flex alignItems='center' direction='horizontal' gap='12px'>
					<mo-avatar ${style({ width: '50px', height: '50px', fontSize: 'x-large', color: 'var(--mo-color-accessible)' })}>
						${(this.profile?.firstName || this.account?.username || '').charAt(0).toUpperCase()}
					</mo-avatar>
					<mo-flex>
						<mo-heading typography='heading3'>${hasName ? name : username}</mo-heading>
						${!hasName ? nothing : html`<mo-heading typography='heading5' ${style({ color: 'var(--mo-color-gray)' })}>${username}</mo-heading>`}
					</mo-flex>
				</mo-flex>

				<mo-button type='raised' icon='person' @click=${this.editProfile}>Edit Profile</mo-button>
			</mo-flex>
		`
	}

	private editProfile = async () => {
		if (this.account?.id) {
			await new DialogAccountProfile({ accountId: this.account.id }).confirm()
			await this.fetchData()
		}
	}

	private get activitiesTemplate() {
		const getScoreTemplate = (header: string, score: number) => html`
			<mo-flex alignItems='center' justifyContent='center'>
				<div>${header}</div>
				<mo-heading typography='heading3' ${style({ color: 'var(--mo-color-accent)' })}>${score}</mo-heading>
			</mo-flex>
		`

		return html`
			<mo-group-box heading='Activities'>
				<mo-flex direction='horizontal' justifyContent='space-around'>
					${getScoreTemplate('Campaigns', this.account?.campaigns?.length ?? 0)}
					${getScoreTemplate('Votes', this.account?.votes?.length ?? 0)}
				</mo-flex>
			</mo-group-box>
		`
	}

	private get securityTemplate() {
		const getAuthenticationMethodTemplate = (name: string, activated = false, editAction?: () => Promise<void> | void, deleteAction?: () => Promise<void> | void) => html`
			<mo-card class='authenticationMethod'>
				<mo-flex direction='horizontal'>
					<mo-icon icon=${activated ? 'verified' : 'gpp_maybe'}
						${style({ color: activated ? 'var(--mo-color-accent)' : 'var(--mo-color-gray)' })}
					></mo-icon>
					<div ${style({ flex: '1' })}>${name}</div>
					${!editAction ? nothing : html`<mo-icon-button icon='edit' @click=${editAction}></mo-icon-button>`}
					${!deleteAction ? nothing : html`<mo-icon-button icon='delete' ${style({ color: 'var(--mo-color-red)' })} @click=${deleteAction}></mo-icon-button>`}
				</mo-flex>
			</mo-card>
		`

		return html`
			<mo-flex gap='4px'>
				<mo-heading typography='heading4'>Security</mo-heading>
				<style>
					.authenticationMethod {
						--mo-card-body-padding: 0px;
					}

					.authenticationMethod mo-flex {
						align-items: center;
						justify-content: center;
						gap: 16px;
						padding: 12px;
					}

					.authenticationMethod mo-icon-button {
						visibility: hidden;
					}

					.authenticationMethod:hover mo-icon-button {
						visibility: visible;
					}
				</style>
				<mo-flex gap='8px'>
					${getAuthenticationMethodTemplate('Password Authentication', this.isNewByAuthenticationMethod.get(AuthenticationMethodType.Password), this.editPasswordAuthentication)}
				</mo-flex>
			</mo-flex>
		`
	}

	private editPasswordAuthentication = async () => {
		await new DialogAuthenticationMethods({ type: AuthenticationMethodType.Password }).confirm()
		await this.fetchData()
	}
}