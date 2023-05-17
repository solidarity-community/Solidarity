import { component, style, html, property } from '@a11d/lit'
import { FieldText } from '@3mo/text-fields'
import { tooltip } from '@3mo/tooltip'
import { PaymentMethodIdentifier, PaymentMethodService } from 'sdk'

@component('solid-field-allocation-destination')
export class FieldAllocationDestination extends FieldText {
	@property() paymentMethodIdentifier!: PaymentMethodIdentifier
	@property() override label = 'Allocation Destination'

	protected override get template() {
		return html`
			${super.template}
			${this.validationTemplate}
		`
	}

	private get validationTemplate() {
		return html`
			<mo-icon icon=${this.invalid ? 'error' : 'done'}
				${tooltip(this.invalid ? 'This value is invalid' : 'This value is valid')}
				${style({ color: this.invalid ? 'var(--mo-color-red)' : 'var(--mo-color-accent)' })}
			></mo-icon>
		`
	}

	override checkValidity() {
		return !this.value
			? Promise.resolve(false)
			: PaymentMethodService.isAllocationDestinationValid(this.paymentMethodIdentifier, this.value)
	}
}

declare global {
	interface HTMLElementTagNameMap {
		'solid-field-allocation-destination': FieldAllocationDestination
	}
}