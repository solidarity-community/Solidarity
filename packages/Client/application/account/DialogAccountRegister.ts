import { component, DialogComponent, html, Snackbar, state } from '@3mo/model'
import { AccountService } from 'sdk'
import JSEncrypt from 'jsencrypt'

@component('solid-dialog-account-register')
export class DialogAccountRegister extends DialogComponent {
	@state() private idAndPrivateKey?: string
	@state() private securityConfirmation = false

	protected override async initialized() {
		const privateKey = new JSEncrypt
		await AccountService.create({
			// TODO: either random string or prompt user
			username: `arshia11d${String(Math.random()).slice(2, 5)}`,
			publicKey: privateKey.getPublicKeyB64(),
		})
		const account = await AccountService.get()
		this.idAndPrivateKey = `${account.id}@${privateKey.getPrivateKey()}`
	}

	protected override get template() {
		return html`
			<mo-dialog heading='Register Account'>
				<mo-button slot='primaryAction' type='raised' ?disabled=${this.securityConfirmation === false}>Register & Continue</mo-button>

				<mo-flex gap='var(--mo-thickness-m)'>
					<mo-div>Safeguard your private-key as it is the only way you can recover your account. Copy it to clipboard by clicking the block.</mo-div>
					<solid-clipboard-text-block>${this.idAndPrivateKey}</solid-clipboard-text-block>
					<mo-checkbox @change=${(e: CustomEvent<CheckboxValue>) => this.securityConfirmation = e.detail === 'checked'}>I have secured my private-key</mo-checkbox>
				</mo-flex>
			</mo-dialog>
		`
	}

	protected override primaryButtonAction() {
		if (!this.idAndPrivateKey) {
			throw new Error('You are not registered')
		}
		return Promise.resolve()
	}
}