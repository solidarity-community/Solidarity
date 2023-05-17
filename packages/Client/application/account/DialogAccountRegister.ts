import { component, html, state } from '@a11d/lit'
import { DialogComponent } from '@a11d/lit-application'
import { AccountService } from 'sdk'
import JSEncrypt from 'jsencrypt'

@component('solid-dialog-account-register')
export class DialogAccountRegister extends DialogComponent {
	@state() private username = ''
	@state() private privateKey = new JSEncrypt
	@state() private isUsernameValid = false
	@state() private securityConfirmation = false

	protected override get template() {
		return html`
			<mo-dialog heading='Register Account'>
				<mo-loading-button slot='primaryAction' type='raised' ?disabled=${!this.securityConfirmation || !this.isUsernameValid}>
					Register
				</mo-loading-button>

				<mo-flex gap='8px'>
					<solid-field-username label='Username'
						@change=${(e: CustomEvent<string>) => this.username = e.detail}
						@validityChange=${(e: CustomEvent<boolean>) => this.isUsernameValid = e.detail}
					></solid-field-username>

					<div>Safeguard your private-key as it is the only way you can recover your account in combination with your username.</div>
					<solid-clipboard-text-block>${this.privateKey.getPrivateKey()}</solid-clipboard-text-block>

					<mo-checkbox @change=${(e: CustomEvent<CheckboxValue>) => this.securityConfirmation = e.detail === 'checked'}>
						I have secured my private-key
					</mo-checkbox>
				</mo-flex>
			</mo-dialog>
		`
	}

	protected override async primaryAction() {
		await AccountService.create({
			username: this.username,
			publicKey: this.privateKey.getPublicKey(),
		})
	}
}