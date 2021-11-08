import { component, DialogComponent, html, ifDefined, nothing, state } from '@3mo/model'
import { AuthenticationMethodType, AuthenticationService } from 'sdk'

@component('solid-dialog-authentication-methods')
export class DialogAuthenticationMethods extends DialogComponent<{ readonly type: AuthenticationMethodType }> {
	@state() private isNewByAuthenticationMethod = new Map<AuthenticationMethodType, boolean>()

	protected override async initialized() {
		this.isNewByAuthenticationMethod = await AuthenticationService.getAll()
	}

	protected override get template() {
		return html`
			<mo-dialog heading=${this.content?.header ?? ''}>
				${this.content?.template ?? nothing}
			</mo-dialog>
		`
	}

	protected override primaryButtonAction() {
		return this.content?.primaryButtonAction()
	}

	private get content() {
		const contentByTab = new Map([
			[AuthenticationMethodType.Password, { header: 'Password Authentication', template: this.passwordTemplate, primaryButtonAction: () => this.savePassword() }]
		])
		return contentByTab.get(this.parameters.type)
	}

	@state() private oldPassword = ''
	@state() private newPassword = ''
	@state() private newPasswordRepeat = ''
	private get passwordTemplate() {
		return html`
			<mo-flex gap='var(--mo-thickness-m)'>
				<mo-field-password label='Old Password'
					?hidden=${this.isNewByAuthenticationMethod.get(AuthenticationMethodType.Password) === false}
					value=${this.oldPassword}
					@change=${(e: CustomEvent<string>) => this.oldPassword = e.detail}
				></mo-field-password>

				<mo-field-password label='New Password'
					value=${this.newPassword}
					@change=${(e: CustomEvent<string>) => this.newPassword = e.detail}
				></mo-field-password>

				<mo-field-password label='Confirm New Password'
					value=${this.newPasswordRepeat}
					@change=${(e: CustomEvent<string>) => this.newPasswordRepeat = e.detail}
				></mo-field-password>
			</mo-flex>
		`
	}

	private savePassword() {
		if (this.newPasswordRepeat !== this.newPassword) {
			throw new Error('Password confirmation failed')
		}
		return AuthenticationService.updatePassword(this.newPassword, this.oldPassword)
	}
}