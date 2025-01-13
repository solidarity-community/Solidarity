import { component, style, html } from '@a11d/lit'
import { DialogAuthenticator, authenticator } from '@a11d/lit-application-authentication'
import { Account, DialogAccountCreation, DialogAccountRecover } from 'application'

@authenticator()
@component('solid-dialog-authentication')
export class DialogAuthentication extends DialogAuthenticator<Account> {
	protected override get template() {
		return html`
			<mo-dialog blocking primaryOnEnter ${style({ '--mo-dialog-scrim-color': 'var(--mo-color-background)' })}>
				<mo-loading-button slot='primaryAction' type='raised'>${t('Login')}</mo-loading-button>
				<mo-loading-button slot='footer' type='outlined' @click=${() => this.register()}>${t('Register')}</mo-loading-button>
				<mo-flex alignItems='center' gap='40px'>
					${this.logoTemplate}
					${this.contentTemplate}
				</mo-flex>
				<mo-flex slot='footer' direction='horizontal' gap='4px' ${style({ flex: '1' })}>
					<mo-button type='outlined' @click=${() => this.recover()}>Recover</mo-button>
				</mo-flex>
			</mo-dialog>
		`
	}

	protected get logoTemplate() {
		return html`
			<mo-flex alignItems='center'>
				<img src='/assets/solidarity.svg' style='height: 200px' />
				<mo-heading style='font-size: 24px'>Solidarity</mo-heading>
			</mo-flex>
		`
	}

	protected get contentTemplate() {
		return html`
			<mo-flex gap='8px' ${style({ height: '*', width: '100%', paddingBottom: '25px' })}>
				<mo-field-text autofocus
					label=${t('Username')}
					.value=${this.username}
					@input=${(e: CustomEvent<string>) => this.username = e.detail}
				></mo-field-text>

				<mo-field-password
					label=${t('Password')}
					.value=${this.password}
					@input=${(e: CustomEvent<string>) => this.password = e.detail}
				></mo-field-password>

				<mo-flex direction='horizontal' justifyContent='space-between' alignItems='center' wrap='wrap-reverse'>
					<mo-checkbox
						label=${t('Remember Password')}
						?selected=${this.shallRememberPassword}
						@change=${(e: CustomEvent<CheckboxSelection>) => this.shallRememberPassword = e.detail === true}
					></mo-checkbox>
				</mo-flex>
			</mo-flex>
		`
	}

	protected async authenticateAccount() {
		await Account.authenticate(this.username, this.password)
		const account = await Account.getAuthenticated()
		return account as Account
	}

	protected unauthenticateAccount() {
		return Promise.resolve(Account.unauthenticate())
	}

	protected getAuthenticatedAccount() {
		return Account.getAuthenticated()
	}

	private async register() {
		await new DialogAccountCreation().confirm()
		return Account.getAuthenticated()
	}

	private async recover() {
		await new DialogAccountRecover().confirm()
		return Account.getAuthenticated()
	}
}