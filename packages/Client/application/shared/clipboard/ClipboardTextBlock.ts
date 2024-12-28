import { Component, component, css, html } from '@a11d/lit'
import { NotificationComponent } from '@a11d/lit-application'
import { tooltip } from '@3mo/tooltip'

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
				background: color-mix(in srgb, var(--mo-color-on-accent), transparent 25%);
				color: var(--mo-color-accent);
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
			<mo-icon-button dense icon='copy' ${tooltip('Copy to Clipboard')} @click=${this.copyContentToClipboard}></mo-icon-button>
		`
	}

	protected copyContentToClipboard = async () => {
		if (this.value) {
			await navigator.clipboard.writeText(this.value)
			NotificationComponent.notifySuccess('Value copied to clipboard')
		}
	}
}

declare global {
	interface HTMLElementTagNameMap {
		'solid-clipboard-text-block': ClipboardTextBlock
	}
}