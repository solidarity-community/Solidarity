import { Component, component, html, query } from '@a11d/lit'
import { SlotController } from '@3mo/slot-controller'
import { Theme, Background } from '@3mo/theme'
import * as QRCode from 'qrcode'

// TODO: Migrate to a11d/lit-qr-code

@component('solid-qr-code')
export class QrCode extends Component {
	protected _ = new SlotController(this, () => this.updateQrCode())

	protected get data() { return this.textContent }

	@query('canvas') private readonly canvasElement?: HTMLCanvasElement

	protected override get template() {
		return html`
			<canvas></canvas>
			<slot hidden></slot>
		`
	}

	private updateQrCode() {
		const backgroundTheme = Theme.background.calculatedValue
		if (this.data) {
			QRCode.toCanvas(this.canvasElement, this.data, {
				margin: 0,
				color: {
					dark: backgroundTheme === Background.Dark ? '#ffffffff' : '#000000ff',
					light: backgroundTheme === Background.Dark ? '#00000000' : '#ffffffff',
				}
			})
		}
	}
}

declare global {
	interface HTMLElementTagNameMap {
		'solid-qr-code': QrCode
	}
}