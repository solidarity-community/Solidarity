import { component, FieldTextBase, html, property } from '@3mo/modelx'
import { AccountService } from 'sdk'

@component('solid-field-username')
export class FieldUsername extends FieldTextBase {
	@property() override label = 'Username'

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
				title=${this.invalid ? 'Username unavailable' : 'Username available'}
				icon=${this.invalid ? 'error' : 'done'}
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