import { component, html, property, style } from '@a11d/lit'
import { FieldText } from '@3mo/text-fields'
import { Account } from 'application'
import { tooltip } from '@3mo/tooltip'

@component('solid-field-username')
export class FieldUsername extends FieldText {
	@property() override label = 'Username'

	protected override get endSlotTemplate() {
		return html`
			${super.endSlotTemplate}
			${this.validationTemplate}
		`
	}

	private get validationTemplate() {
		return html`
			<mo-icon slot='end' icon=${this.invalid ? 'error' : 'done'}
				${style({ color: this.invalid ? 'var(--mo-color-red)' : 'var(--mo-color-accent)' })}
				${tooltip(this.invalid ? 'Username unavailable' : 'Username available')}
			></mo-icon>
		`
	}

	override checkValidity() {
		return !this.inputStringValue
			? Promise.resolve(false)
			: Account.isUsernameAvailable(this.inputStringValue)
	}
}

declare global {
	interface HTMLElementTagNameMap {
		'solid-field-username': FieldUsername
	}
}