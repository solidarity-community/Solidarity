import { component, DialogComponent, html, state } from '@3mo/modelx'
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
					Register & Continue
				</mo-loading-button>

				<mo-flex gap='var(--mo-thickness-m)'>
					<solid-field-username label='Username'
						@change=${(e: CustomEvent<string>) => this.username = e.detail}
						@validityChange=${(e: CustomEvent<boolean>) => this.isUsernameValid = e.detail}
					></solid-field-username>

					<mo-div>Safeguard your private-key as it is the only way you can recover your account in combination with your username.</mo-div>
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
			// TODO: either random string or prompt user
			username: this.username,
			publicKey: this.privateKey!.getPublicKeyB64(),
		})
		const account = await AccountService.get()

		if (!this.privateKey) {
			throw new Error('You are not registered')
		}
		return Promise.resolve()
	}
}