import { component, property, html, style } from '@a11d/lit'
import { DataGridColumnComponent } from '@3mo/data-grid'
import { PaymentMethod, type PaymentMethodIdentifier } from 'application'

@component('solid-data-grid-column-payment-method')
export class DataGridColumnPaymentMethod<TData> extends DataGridColumnComponent<TData, PaymentMethodIdentifier> {
	@property() override width = '40px'

	override getContentTemplate(value?: PaymentMethodIdentifier) {
		return !value ? html.nothing : html`<img src=${PaymentMethod.getLogoSource(value)} ${style({ margin: '10%', height: '80%' })} />`
	}

	override getEditContentTemplate(value?: PaymentMethodIdentifier) {
		return this.getContentTemplate(value)
	}
}

declare global {
	interface HTMLElementTagNameMap {
		'solid-data-grid-column-payment-method': DataGridColumnPaymentMethod<unknown>
	}
}