import { component, html, property } from '@a11d/lit'
import { ClipboardTextBlock } from './ClipboardTextBlock'
import { tooltip } from '@3mo/tooltip'

@component('solid-wallet-address')
export class WalletAddress extends ClipboardTextBlock {
	@property() protocol?: string

	protected override get actionsTemplate() {
		return html`
			${super.actionsTemplate}
			${!this.protocol ? html.nothing : html`
				<mo-icon-button dense icon='launch' ${tooltip('Open in a client')} @click=${this.openClient}></mo-icon-button>
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