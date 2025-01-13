import { html, component, Component, css, property } from '@a11d/lit'
import { routerLink } from '@a11d/lit-application'
import { Account, PageAccount } from 'application'

@component('fm-account-avatar')
export class AccountAvatar extends Component {
	@property({ type: Object }) account!: Account
	@property({ type: Boolean, attribute: true }) withMenu = false

	static override get styles() {
		return css`
			#avatar {
				height: 40px;
				width: 40px;
				aspect-ratio: 1 / 1;
				display: flex;
				user-select: none;
				justify-content: center;
				align-items: center;
				border-radius: 50%;
				font-size: large;
				background: var(--mo-color-surface);
				color: var(--mo-color-foreground);
			}

			:host([withMenu]) #avatar {
				cursor: pointer;
			}

			mo-menu {
				margin-top: 10px;


				div {
					padding: 10px;
				}

				mo-menu-item {
					height: 40px;

					&[data-router-selected] {
						background: color-mix(in srgb, var(--mo-color-surface), var(--mo-color-accent) 16%);
					}
				}
			}
		`
	}

	private get avatarTemplate() {
		return html`
			<div id='avatar'>
				${(this.account.nameOrUsername || '').charAt(0).toUpperCase()}
			</div>
		`
	}

	protected override get template() {
		return !this.account ? html.nothing : !this.withMenu ? this.avatarTemplate : html`
			<mo-popover-container>
				${this.avatarTemplate}
				<mo-menu slot='popover'>
					<div>Hey ${this.account.nameOrUsername}!</div>
					<mo-menu-item icon='account_circle' ${routerLink(new PageAccount())}>Account</mo-menu-item>
					<mo-menu-item icon='logout' @click=${() => Account.unauthenticate()}>Logout</mo-menu-item>
				</mo-menu>
			</mo-popover-container>
		`
	}
}

declare global {
	interface HTMLElementTagNameMap {
		'fm-account-avatar': AccountAvatar
	}
}