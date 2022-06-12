import { Component, component, css, html, Snackbar } from '@3mo/model'

@component('solid-clipboard-text-block')
export class ClipboardTextBlock extends Component {
	static override get styles() {
		return css`
			code {
				background: var(--mo-color-transparent-gray);
				font-size: 12px;
				line-height: 12px;
				line-break: anywhere;
				padding: 10px;
				border-radius: 4px;
				display: block;
			}

			code:hover {
				background: var(--mo-accent-transparent);
				color: var(--mo-accent);
				cursor: pointer;
			}
		`
	}

	protected override get template() {
		return html`
			<code @click=${this.copyPrivateKeyToClipboard}>
				<slot></slot>
			</code>
		`
	}

	private copyPrivateKeyToClipboard = async () => {
		const value = this.textContent
		if (value) {
			await navigator.clipboard.writeText(value)
			Snackbar.show('Value copied to clipboard')
		}
	}
}

declare global {
	interface HTMLElementTagNameMap {
		'solid-clipboard-text-block': ClipboardTextBlock
	}
}