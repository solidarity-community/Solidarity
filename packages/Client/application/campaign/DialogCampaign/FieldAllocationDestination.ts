import { component, style, html, property } from '@a11d/lit'
import { FieldText } from '@3mo/text-fields'
import { tooltip } from '@3mo/tooltip'
import { type PaymentMethodIdentifier, PaymentMethod } from 'application'

@component('solid-field-allocation-destination')
export class FieldAllocationDestination extends FieldText {
	@property() paymentMethodIdentifier!: PaymentMethodIdentifier
	@property() override label = 'Allocation Destination'

	protected override get endSlotTemplate() {
		return html`
			${super.endSlotTemplate}
			<mo-icon slot='end' icon=${this.invalid ? 'error' : 'done'}
				${tooltip(this.invalid ? 'This value is invalid' : 'This value is valid')}
				${style({ color: this.invalid ? 'var(--mo-color-red)' : 'var(--mo-color-accent)' })}
			></mo-icon>
		`
	}

	override checkValidity() {
		return !this.inputStringValue
			? Promise.resolve(false)
			: PaymentMethod.isAllocationDestinationValid(this.paymentMethodIdentifier, this.inputStringValue)
	}
}

declare global {
	interface HTMLElementTagNameMap {
		'solid-field-allocation-destination': FieldAllocationDestination
	}
}