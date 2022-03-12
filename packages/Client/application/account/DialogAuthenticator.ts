import { component, DialogAuthenticator as DialogAuthenticatorBase, User, LocalStorageEntry, authenticator, DialogPrompt, html, state, Snackbar } from '@3mo/model'
import { AccountService, AuthenticationService } from 'sdk'
import { DialogAccountRegister, DialogAccountReset } from 'application'

@authenticator()
@component('solid-dialog-authenticator')
export class DialogAuthenticator extends DialogAuthenticatorBase {
	protected async authenticateProcess() {
		await AuthenticationService.authenticateWithPassword(this.username, this.password)
		const user = await this.fetchUser()
		MoDeL.application.requestUpdate()
		return user
	}

	private async fetchUser() {
		const account = await AccountService.get()
		const user: User = {
			id: account.id ?? 0,
			name: account.username ?? '',
			email: account.username ?? '',
		}
		DialogAuthenticator.authenticatedUser.value = user
		return user
	}

	protected async unauthenticateProcess() {
		AuthenticationService.unauthenticate()
	}

	protected checkAuthenticationProcess() {
		return AuthenticationService.isAuthenticated()
	}

	protected async resetPasswordProcess() {
		await new DialogAccountReset().confirm()
		await this.fetchUser()
		this.close()
	}

	protected override get contentTemplate() {
		return html`
			${super.contentTemplate}
			<mo-button @click=${() => this.register()}>Or Create an Account</mo-button>
		`
	}

	private async register() {
		await new DialogAccountRegister().confirm()
		await this.fetchUser()
		this.close()
		MoDeL.application.requestUpdate()
	}
}