import { component, property } from '@a11d/lit'
import { QrCode } from '@a11d/lit-qr-code'

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