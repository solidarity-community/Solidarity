import { Binder, component, html, state } from '@a11d/lit'
import { DialogComponent } from '@a11d/lit-application'
import { Account, PageAccount } from 'application'
import JSEncrypt from 'jsencrypt'

@component('solid-dialog-account-creation')
export class DialogAccountCreation extends DialogComponent {
	@state() privateKey = new JSEncrypt
	@state() account = new Account({ publicKey: this.privateKey.getPublicKey() })
	@state() isUsernameValid = false
	@state() securityConfirmation = false

	private readonly accountBinder = new Binder(this, 'account')

	protected override get template() {
		const { bind } = this.accountBinder
		return html`
			<mo-dialog heading='Register Account'>
				<mo-loading-button slot='primaryAction' type='raised' ?disabled=${!this.securityConfirmation || !this.isUsernameValid}>
					Register
				</mo-loading-button>

				<mo-flex gap='8px'>
					<solid-field-username label='Username'
						${bind('username')}
						@validityChange=${(e: CustomEvent<boolean>) => this.isUsernameValid = e.detail}
					></solid-field-username>

					<mo-field-password label='Password' ${bind({ keyPath: 'plainPassword', sourceUpdated: () => this.account.hashPassword() })}></mo-field-password>

					<div>Safeguard your private-key as it is the only way you can recover your account in combination with your username.</div>
					<solid-clipboard-text-block>${this.privateKey.getPrivateKey()}</solid-clipboard-text-block>

					<mo-checkbox label='I have secured my private-key'
						@change=${(e: CustomEvent<CheckboxSelection>) => this.securityConfirmation = e.detail === true}>
					</mo-checkbox>
				</mo-flex>
			</mo-dialog>
		`
	}

	protected override async primaryAction() {
		await Account.create(this.account)
		await new PageAccount().navigate()
	}
}