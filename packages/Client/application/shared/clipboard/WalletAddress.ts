import { component, html, property, nothing } from '@3mo/modelx'
import { ClipboardTextBlock } from './ClipboardTextBlock'

@component('solid-wallet-address')
export class WalletAddress extends ClipboardTextBlock {
	@property() protocol?: string

	protected override get actionsTemplate() {
		return html`
			${super.actionsTemplate}
			${!this.protocol ? nothing : html`
				<mo-icon-button small icon='launch' title='Open in a client' @click=${this.openClient}></mo-icon-button>
			`}
		`
	}

	private openClient = () => {
		if (this.value && this.protocol) {
			window.open(`${this.protocol}:${this.value}`, '_blank')
		}
	}
}

declare global {
	interface HTMLElementTagNameMap {
		'solid-wallet-address': WalletAddress
	}
}