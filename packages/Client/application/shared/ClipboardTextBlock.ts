import { Component, component, css, eventListener, html, NotificationHost } from '@3mo/model'

@component('solid-clipboard-text-block')
export class ClipboardTextBlock extends Component {
	protected get value() { return this.textContent }

	static override get styles() {
		return css`
			:host {
				position: relative;
			}

			code {
				background: var(--mo-color-transparent-gray);
				font-size: 12px;
				line-height: 12px;
				line-break: anywhere;
				padding: 10px;
				border-radius: 4px;
				display: block;
			}

			mo-flex {
				visibility: hidden;
				position: absolute;
				top: 0px;
				right: 0px;
				cursor: pointer;
			}

			mo-flex mo-icon-button {
				border-radius: 50%;
				background: rgba(var(--mo-color-accessible-base), 0.75);
				color: var(--mo-accent);
			}

			:host(:hover) mo-flex {
				visibility: visible;
			}
		`
	}

	protected override get template() {
		return html`
			<code>
				<slot></slot>
			</code>
			<mo-flex direction='horizontal-reversed' gap='2px'>
				${this.actionsTemplate}
			</mo-flex>
		`
	}

	protected get actionsTemplate() {
		return html`
			<mo-icon-button small icon='copy' title='Copy to clipboard' @click=${this.copyContentToClipboard}></mo-icon-button>
		`
	}

	protected copyContentToClipboard = async () => {
		if (this.value) {
			await navigator.clipboard.writeText(this.value)
			NotificationHost.instance.notifySuccess('Value copied to clipboard')
		}
	}
}

declare global {
	interface HTMLElementTagNameMap {
		'solid-clipboard-text-block': ClipboardTextBlock
	}
}