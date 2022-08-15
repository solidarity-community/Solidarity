import { component, DialogComponent, html, state } from '@3mo/modelx'
import { AccountService } from 'sdk'
import JSEncrypt from 'jsencrypt'

@component('solid-dialog-account-recover')
export class DialogAccountRecover extends DialogComponent {
	@state() private username?: string
	@state() private privateKey?: string

	protected override get template() {
		return html`
			<mo-dialog heading='Reset Account'>
				<mo-loading-button slot='primaryAction' type='raised' ?disabled=${!this.username || !this.privateKey}>
					Reset
				</mo-loading-button>

				<mo-flex gap='var(--mo-thickness-m)'>
					<span>To recover your account enter your username and the private key you were provided during account registration.</span>
					<mo-field-text label='Username' @input=${(e: CustomEvent<string>) => this.username = e.detail}></mo-field-text>
					<mo-text-area label='Private-Key' rows='10' @input=${(e: CustomEvent<string>) => this.privateKey = e.detail}></mo-text-area>
				</mo-flex>
			</mo-dialog>
		`
	}

	protected override async primaryAction() {
		if (!this.privateKey) {
			throw new Error('Enter your private-key')
		}

		if (!this.username) {
			throw new Error('Enter your username')
		}

		const encryptedPhrase = await AccountService.reset(this.username)

		const privateKey = new JSEncrypt
		privateKey.setKey(this.privateKey)
		const decryptedPhrase = privateKey.decrypt(encryptedPhrase)

		if (decryptedPhrase === false) {
			throw new Error('Cannot decrypt the phrase')
		}

		await AccountService.recover(decryptedPhrase)
	}
}