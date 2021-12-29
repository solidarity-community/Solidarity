import { component, DialogComponent, html, state } from '@3mo/model'
import { Identity, IdentityService } from 'sdk'

@component('solid-dialog-identity')
export class DialogIdentity extends DialogComponent<{ readonly accountId: number }> {
	@state() private identity: Identity = { accountId: this.parameters.accountId }

	protected override async initialized() {
		try {
			this.identity = await IdentityService.getByAccountId(this.parameters.accountId)
		} finally { }
	}

	protected override get template() {
		return html`
			<mo-dialog heading='Profile' primaryButtonText='Save'>
				<mo-flex gap='var(--mo-thickness-m)'>
					<mo-field-text label='First name'
						value=${this.identity.firstName ?? ''}
						@change=${(e: CustomEvent<string>) => this.identity.firstName = e.detail}
					></mo-field-text>

					<mo-field-text label='Last name'
						value=${this.identity.lastName ?? ''}
						@change=${(e: CustomEvent<string>) => this.identity.lastName = e.detail}
					></mo-field-text>

					<mo-field-date label='Birth date'
						.value=${this.identity.birthDate ? new Date(this.identity.birthDate) : undefined}
						@change=${(e: CustomEvent<Date | undefined>) => this.identity.birthDate = e.detail?.toJSON()}
					></mo-field-date>
				</mo-flex>
			</mo-dialog>
		`
	}

	protected override async primaryButtonAction() {
		await IdentityService.createOrUpdate(this.identity)
	}
}