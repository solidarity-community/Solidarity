import { component, DialogComponent, html, state } from '@3mo/modelx'
import { AccountProfile, AccountProfileService } from 'sdk'

@component('solid-dialog-account-profile')
export class DialogAccountProfile extends DialogComponent<{ readonly accountId: number }> {
	@state() private profile = new AccountProfile(this.parameters.accountId)

	protected override async initialized() {
		try {
			this.profile = await AccountProfileService.getByAccountId(this.parameters.accountId)
		} finally { }
	}

	protected override get template() {
		return html`
			<mo-dialog heading='Profile' primaryButtonText='Save' primaryOnEnter>
				<mo-flex gap='var(--mo-thickness-m)'>
					<mo-field-text label='First name'
						value=${this.profile.firstName ?? ''}
						@change=${(e: CustomEvent<string>) => this.profile.firstName = e.detail}
					></mo-field-text>

					<mo-field-text label='Last name'
						value=${this.profile.lastName ?? ''}
						@change=${(e: CustomEvent<string>) => this.profile.lastName = e.detail}
					></mo-field-text>

					<mo-field-date label='Birth date'
						.value=${this.profile.birthDate}
						@change=${(e: CustomEvent<Date | undefined>) => this.profile.birthDate = e.detail}
					></mo-field-date>
				</mo-flex>
			</mo-dialog>
		`
	}

	protected override async primaryAction() {
		await AccountProfileService.createOrUpdate(this.profile)
	}
}