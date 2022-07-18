import { component, DataGridColumn, html, nothing } from '@3mo/modelx'
import { PaymentMethod, PaymentMethodIdentifier } from 'sdk'

@component('solid-data-grid-column-payment-method')
export class DataGridColumnPaymentMethod<TData> extends DataGridColumn<TData, PaymentMethodIdentifier> {
	constructor() {
		super()
		this.width = '40px'
	}

	getContentTemplate(value?: PaymentMethodIdentifier) {
		return !value ? nothing : html`<img height='80%' src=${PaymentMethod.getLogoSource(value)} style='margin: 10%' />`
	}

	getEditContentTemplate(value?: PaymentMethodIdentifier) {
		return this.getContentTemplate(value)
	}
}

declare global {
	interface HTMLElementTagNameMap {
		'solid-data-grid-column-payment-method': DataGridColumnPaymentMethod<unknown>
	}
}