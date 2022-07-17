import { component, PageComponent, html, route, state, PageError, HttpErrorCode, nothing, authentication } from '@3mo/modelx'
import { Account, AccountService, AuthenticationMethodType, AuthenticationService, Identity, IdentityService } from 'sdk'
import { DialogIdentity, DialogAuthenticationMethods, DialogAuthenticator } from 'application'

@authentication()
@route('/account')
@component('solid-page-account')
export class PageAccount extends PageComponent {
	@state() private account: Account = {}
	@state() private identity?: Identity
	@state() private isNewByAuthenticationMethod = new Map<AuthenticationMethodType, boolean>()

	protected override async initialized() {
		await this.redirectToErrorPageIfNotAuthenticated()
		await this.fetchData()
	}

	private async fetchData() {
		await Promise.all([
			this.fetchAccount(),
			this.fetchAuthenticationMethods(),
			this.fetchIdentity()
		])
	}

	private async redirectToErrorPageIfNotAuthenticated() {
		const isAuthenticated = await AuthenticationService.isAuthenticated()
		if (isAuthenticated === false) {
			new PageError({
				error: HttpErrorCode.Unauthorized,
				message: 'You are not authenticated into your account.'
			}).navigate()
		}
	}

	private async fetchAccount() {
		this.account = await AccountService.get()
	}

	private async fetchAuthenticationMethods() {
		this.isNewByAuthenticationMethod = await AuthenticationService.getAll()
	}

	private async fetchIdentity() {
		this.identity = await IdentityService.getByAccountId()
	}

	protected override get template() {
		return html`
			<mo-page heading='Account'>
				<mo-flex height='100%' width='100%' maxWidth='1028px' margin='auto' gap='24px'>
					${this.topBarTemplate}
					<mo-grid height='*' gap='16px'>
						${this.activitiesTemplate}
						${this.securityTemplate}
					</mo-grid>
				</mo-flex>
			</mo-page>
		`
	}

	private get topBarTemplate() {
		const name = `${this.identity?.firstName ?? ''} ${this.identity?.lastName ?? ''}`
		const username = this.account.username
		const hasName = !!name.trim()
		return html`
			<mo-flex direction='horizontal' justifyContent='space-between' alignItems='center'>
				<mo-flex alignItems='center' direction='horizontal' gap='12px'>
					<mo-avatar width='50px' height='50px' fontSize='var(--mo-font-size-xl)' foreground='var(--mo-color-accessible)'>${(this.identity?.firstName || DialogAuthenticator.authenticatedUser.value?.name || '').charAt(0).toUpperCase()}</mo-avatar>
					<mo-flex>
						<mo-heading typography='heading3'>${hasName ? name : username}</mo-heading>
						${!hasName ? nothing : html`<mo-heading typography='heading5' foreground='var(--mo-color-gray)'>${username}</mo-heading>`}
					</mo-flex>
				</mo-flex>

				<mo-button type='raised' icon='person' @click=${this.editProfile}>Edit Profile</mo-button>
			</mo-flex>
		`
	}

	private editProfile = async () => {
		if (!this.account.id) {
			return
		}
		await new DialogIdentity({ accountId: this.account.id }).confirm()
		await this.fetchData()
	}

	private get activitiesTemplate() {
		const getScoreTemplate = (header: string, score: number) => html`
			<mo-flex alignItems='center' justifyContent='center'>
				<mo-div>${header}</mo-div>
				<mo-heading typography='heading3' foreground='var(--mo-accent)'>${score}</mo-heading>
			</mo-flex>
		`

		return html`
			<mo-group-box heading='Activities'>
				<mo-flex direction='horizontal' justifyContent='space-around'>
					${getScoreTemplate('Campaigns', this.account.campaigns?.length ?? 0)}
					${getScoreTemplate('Votes', this.account.votes?.length ?? 0)}
				</mo-flex>
			</mo-group-box>
		`
	}

	private get securityTemplate() {
		const getAuthenticationMethodTemplate = (name: string, activated = false, editAction?: () => Promise<void> | void, deleteAction?: () => Promise<void> | void) => html`
			<mo-card class='authenticationMethod'>
				<mo-flex direction='horizontal'>
					<mo-icon foreground=${activated ? 'var(--mo-accent)' : 'var(--mo-color-gray)'} icon=${activated ? 'verified' : 'gpp_maybe'}></mo-icon>
					<mo-div fontSize='var(--mo-font-size-m)' width='*'>${name}</mo-div>
					${!editAction ? nothing : html`<mo-icon-button icon='edit' @click=${editAction}></mo-icon-button>`}
					${!deleteAction ? nothing : html`<mo-icon-button icon='delete' foreground='var(--mo-color-error)' @click=${deleteAction}></mo-icon-button>`}
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