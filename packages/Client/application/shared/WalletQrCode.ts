import { component, property } from '@3mo/modelx'
import { QrCode } from './QrCode'

@component('solid-wallet-qr-code')
export class WalletQrCode extends QrCode {
	@property() protocol?: string

	protected override get data() {
		return `${this.protocol}:${super.data}`
	}
}

declare global {
	interface HTMLElementTagNameMap {
		'solid-wallet-qr-code': WalletQrCode
	}
}