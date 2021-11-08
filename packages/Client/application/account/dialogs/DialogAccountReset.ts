import { component, DialogComponent, html, ifDefined, state } from '@3mo/model'
import { AccountService } from 'sdk'
import JSEncrypt from 'jsencrypt'

@component('solid-dialog-account-reset')
export class DialogAccountReset extends DialogComponent {
	@state() private idAndPrivateKey?: string

	protected override get template() {
		return html`
			<mo-dialog heading='Reset Account' primaryButtonText='Reset'>
				<mo-flex gap='var(--mo-thickness-m)'>
					<span>To reset your account enter the private key you were provided on account creation</span>
					<mo-text-area label='Private-Key' rows='10'
						value=${ifDefined(this.idAndPrivateKey)}
						@change=${(e: CustomEvent<string>) => this.idAndPrivateKey = e.detail}
					></mo-text-area>
				</mo-flex>
			</mo-dialog>
		`
	}

	protected override async primaryButtonAction() {
		if (!this.idAndPrivateKey) {
			throw new Error('Enter your private-key')
		}

		const parts = this.idAndPrivateKey.split('@')
		const id = Number(parts[0])
		const privateKeyString = parts[1]
		const privateKey = new JSEncrypt
		privateKey.setKey(privateKeyString)
		const encryptedPhrase = await AccountService.reset(id)
		const decryptedPhrase = privateKey.decrypt(encryptedPhrase)
		if (decryptedPhrase === false) {
			throw new Error('Cannot decrypt the phrase')
		}
		await AccountService.recover(decryptedPhrase)
	}
}