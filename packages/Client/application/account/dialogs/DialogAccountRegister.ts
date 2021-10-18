import { component, DialogComponent, html, Snackbar, state } from '@3mo/model'
import { AccountService } from 'sdk'
import JSEncrypt from 'jsencrypt'

@component('solid-dialog-account-register')
export class DialogAccountRegister extends DialogComponent {
	@state() idAndPrivateKey?: string
	@state() securityConfirmation = false

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
			<mo-dialog header='Register Account'>
				<mo-button slot='primaryAction' ?disabled=${this.securityConfirmation === false}>Register & Continue</mo-button>

				<style>
					code {
						background: var(--mo-color-transparent-gray);
						font-size: 12px;
						line-height: 12px;
						line-break: anywhere;
						padding: 10px;
						border-radius: 4px;
					}

					code:hover {
						background: var(--mo-accent-transparent);
						color: var(--mo-accent);
						cursor: pointer;
					}
				</style>

				<mo-flex gap='var(--mo-thickness-m)'>
					<mo-div>Safeguard your private-key as it is the only way you can recover your account. Copy it to clipboard by clicking the block.</mo-div>
					<code @click=${this.copyPrivateKeyToClipboard}>${this.idAndPrivateKey}</code>
					<mo-checkbox @change=${(e: CustomEvent<CheckboxValue>) => this.securityConfirmation = e.detail === 'checked'}>I have secured my private-key</mo-checkbox>
				</mo-flex>

			</mo-dialog>
		`
	}

	private copyPrivateKeyToClipboard = async () => {
		if (!this.idAndPrivateKey) {
			return
		}
		await navigator.clipboard.writeText(this.idAndPrivateKey)
		Snackbar.show('Private-key copied to clipboard')
	}

	protected override primaryButtonAction() {
		if (!this.idAndPrivateKey) {
			throw new Error('You are not registered')
		}
		return Promise.resolve()
	}
}