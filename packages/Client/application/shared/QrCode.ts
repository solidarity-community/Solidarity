import { Background, Component, component, html, query, SlotController, ThemeHelper } from '@3mo/model'
import * as QRCode from 'qrcode'

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
		const backgroundTheme = ThemeHelper.background.calculatedValue
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