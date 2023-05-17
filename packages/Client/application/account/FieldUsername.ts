import { component, html, property, style } from '@a11d/lit'
import { FieldText } from '@3mo/text-fields'
import { AccountService } from 'sdk'
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
		return !this.value
			? Promise.resolve(false)
			: AccountService.isUsernameAvailable(this.value)
	}
}

declare global {
	interface HTMLElementTagNameMap {
		'solid-field-username': FieldUsername
	}
}