import { component, html, style } from '@a11d/lit'
import { PageComponent, PageError, route, HttpErrorCode } from '@a11d/lit-application'
import { requiresAuthentication } from '@a11d/lit-application-authentication'
import { Task } from '@lit/task'
import { Account, DialogAccountCreation } from 'application'

@requiresAuthentication()
@route('/account')
@component('solid-page-account')
export class PageAccount extends PageComponent {
	readonly accountFetchTask = new Task(this, Account.getAuthenticated, () => [])
	get account() { return this.accountFetchTask.value }

	protected override async initialized() {
		await this.accountFetchTask.taskComplete
		if (!this.accountFetchTask.value) {
			new PageError({ error: HttpErrorCode.Unauthorized, message: 'You are not authenticated into your account.' }).navigate()
		}
	}

	protected override get template() {
		return html`
			<mo-page heading='Account'>
				<mo-flex gap='24px' ${style({ height: '100%', width: '100%', maxWidth: '1028px', margin: 'auto' })}>
					${this.accountTemplate}
					${this.activitiesTemplate}
					${this.securityTemplate}
					<mo-grid gap='16px' ${style({ flex: '1' })}>
					</mo-grid>
				</mo-flex>
			</mo-page>
		`
	}

	private get accountTemplate() {
		return html`
			<mo-flex direction='horizontal' justifyContent='space-between' alignItems='center'>
				<mo-flex alignItems='center' direction='horizontal' gap='12px'>
					<fm-avatar ${style({ width: '50px', height: '50px', fontSize: 'x-large', color: 'var(--mo-color-on-accent)' })}>
						${(this.account?.name || this.account?.username || '').charAt(0).toUpperCase()}
					</fm-avatar>
					<mo-flex>
						<mo-heading typography='heading3'>${this.account?.nameOrUsername}</mo-heading>
						${!this.account?.username ? html.nothing : html`<mo-heading typography='heading5' ${style({ color: 'var(--mo-color-gray)' })}>${this.account.username}</mo-heading>`}
					</mo-flex>
				</mo-flex>
			</mo-flex>
		`
	}

	private get activitiesTemplate() {
		const getScoreTemplate = (header: string, score: number) => html`
			<mo-flex alignItems='center' justifyContent='center'>
				<div>${header}</div>
				<mo-heading typography='heading3' ${style({ color: 'var(--mo-color-accent)' })}>${score}</mo-heading>
			</mo-flex>
		`

		return html`
			<mo-group-box heading='Activities'>
				<mo-flex direction='horizontal' justifyContent='space-around'>
					${getScoreTemplate('Campaigns', this.account?.campaigns?.length ?? 0)}
					${getScoreTemplate('Votes', this.account?.votes?.length ?? 0)}
				</mo-flex>
			</mo-group-box>
		`
	}

	private get securityTemplate() {
		const getAuthenticationMethodTemplate = (name: string, activated = false, editAction?: () => Promise<void> | void, deleteAction?: () => Promise<void> | void) => html`
			<mo-card class='authenticationMethod'>
				<mo-flex direction='horizontal'>
					<mo-icon icon=${activated ? 'verified' : 'gpp_maybe'}
						${style({ color: activated ? 'var(--mo-color-accent)' : 'var(--mo-color-gray)' })}
					></mo-icon>
					<div ${style({ flex: '1' })}>${name}</div>
					${!editAction ? html.nothing : html`<mo-icon-button icon='edit' @click=${editAction}></mo-icon-button>`}
					${!deleteAction ? html.nothing : html`<mo-icon-button icon='delete' ${style({ color: 'var(--mo-color-red)' })} @click=${deleteAction}></mo-icon-button>`}
				</mo-flex>
			</mo-card>
		`

		getAuthenticationMethodTemplate

		return html`
			<mo-group-box id='security' heading='Security'>
				<style>
					mo-group-box#security {
						--mo-card-body-padding: 0px;

						&::part(card) {
							background: transparent;
						}

						.authenticationMethod {
							--mo-card-body-padding: 0px;

							mo-flex {
								align-items: center;
								justify-content: center;
								gap: 16px;
								padding: 12px;
							}

							mo-icon-button {
								visibility: hidden;
							}

							mo-icon-button {
								visibility: visible;
							}
						}
					}
				</style>
				<mo-flex gap='8px'>
					${getAuthenticationMethodTemplate('Password', !!this.account?.password)}
				</mo-flex>
			</mo-group-box>
		`
	}
}