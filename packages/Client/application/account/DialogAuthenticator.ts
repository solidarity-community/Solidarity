import { component, DialogAuthenticator as DialogAuthenticatorBase, User, authenticator, html, css } from '@3mo/modelx'
import { AccountService, AuthenticationService } from 'sdk'
import { DialogAccountRegister, DialogAccountRecover } from 'application'

@authenticator()
@component('solid-dialog-authenticator')
export class DialogAuthenticator extends DialogAuthenticatorBase {
	static override get styles() {
		return css`
			${super.styles}
			a { display: none; }
		`
	}

	protected async authenticateProcess() {
		await AuthenticationService.authenticateWithPassword(this.username, this.password)
		const user = await this.fetchUser()
		this.requestApplicationUpdate()
		return user
	}

	private async fetchUser() {
		const account = await AccountService.get()
		const user: User = {
			id: account?.id ?? 0,
			name: account?.username ?? '',
			email: '',
		}
		DialogAuthenticator.authenticatedUser.value = user
		return user
	}

	protected async unauthenticateProcess() {
		AuthenticationService.unauthenticate()
		this.requestApplicationUpdate()
	}

	protected checkAuthenticationProcess() {
		return AuthenticationService.isAuthenticated()
	}

	protected async resetPasswordProcess() {
		throw new NotImplementedError()
	}

	protected override get additionalTemplate() {
		return html`
			<mo-flex slot='footer' direction='horizontal' width='*' gap='4px'>
				<mo-button type='outlined' @click=${() => this.register()}>Register</mo-button>
				<mo-button type='outlined' @click=${() => this.recover()}>Recover</mo-button>
			</mo-flex>
		`
	}

	private async register() {
		await new DialogAccountRegister().confirm()
		await this.fetchUser()
		this.requestApplicationUpdate()
		this.close()
	}

	private async recover() {
		await new DialogAccountRecover().confirm()
		await this.fetchUser()
		this.requestApplicationUpdate()
		this.close()	
	}
}