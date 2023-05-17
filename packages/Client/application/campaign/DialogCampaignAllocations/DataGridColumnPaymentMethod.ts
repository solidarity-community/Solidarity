import { component, property, html, nothing, style } from '@a11d/lit'
import { DataGridColumn } from '@3mo/data-grid'
import { PaymentMethod, PaymentMethodIdentifier } from 'sdk'

@component('solid-data-grid-column-payment-method')
export class DataGridColumnPaymentMethod<TData> extends DataGridColumn<TData, PaymentMethodIdentifier> {
	@property() override width = '40px'

	getContentTemplate(value?: PaymentMethodIdentifier) {
		return !value ? nothing : html`<img src=${PaymentMethod.getLogoSource(value)} ${style({ margin: '10%', height: '80%' })} />`
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