import { component, FieldTextBase, html, property } from '@3mo/modelx'
import { PaymentMethodIdentifier, PaymentMethodService } from 'sdk'

@component('solid-field-allocation-destination')
export class FieldAllocationDestination extends FieldTextBase {
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
			<mo-icon
				foreground=${this.invalid ? 'var(--mo-color-error)' : 'var(--mo-accent)'}
				title=${this.invalid ? 'This value is invalid' : 'This value is valid'}
				icon=${this.invalid ? 'error' : 'done'}
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