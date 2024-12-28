import { Component, component, css, html } from '@a11d/lit'
import { Application, application, routerLink } from '@a11d/lit-application'
import { Theme } from '@3mo/theme'
import { DataGrid } from '@3mo/data-grid'
import * as Solid from '.'
import { Api } from '@a11d/api'
import { Icon, IconVariant } from '@3mo/icon'

Theme.accent.value = 'rgb(124, 179, 76)'
DataGrid.hasAlternatingBackground.value = false
Icon.defaultVariant = IconVariant.Outlined


@application()
@component('solid-application')
export class Solidarity extends Application {
	static override get styles() {
		return css`
			@import url('https://fonts.googleapis.com/css2?family=Inter:wght@200;300;400;500&display=swap');

			${super.styles}


			:root {
				background-color: var(--mo-color-background);
				font-family: var(--mo-font-family);
				--mo-font-family: 'Inter', sans-serif;
				--mdc-typography-font-family: var(--mo-font-family);
				--mdc-typography-body1-font-family: var(--mo-font-family);
				--md-ref-typeface-plain: var(--mo-font-family);
				font-size: 1rem;
			}
		`
	}

	protected override async initialized() {
		if (await Solid.Account.isAuthenticated() === false) {
			Solid.JwtApiAuthenticator.token = undefined
		}

		return super.initialized()
	}

	protected override get bodyTemplate() {
		return html`
			<solid-header></solid-header>
			${super.bodyTemplate}
		`
	}

	protected get userAvatarMenuItemsTemplate() {
		return !Api.authenticator?.isAuthenticated() ? html`
			<mo-navigation-list-item icon='login' ${routerLink(new Solid.DialogAuthentication)}>Login or Register</mo-navigation-list-item>
		` : html`
			<mo-navigation-list-item icon='account_circle' ${routerLink(new Solid.PageAccount)}>Account</mo-navigation-list-item>
		`
	}
}

@component('solid-header')
export class Header extends Component {
	static override get styles() {
		return css`
			:host {
				display: flex;
				justify-content: space-between;
				align-items: center;
				padding: 0 16px;
				height: 64px;
				border-bottom: 1px solid var(--mo-color-transparent-gray-3);
			}

			mo-flex#logo {
				height: 100%;
				opacity: 0.8;
				color: color-mix(in srgb, var(--mo-color-foreground), var(--mo-color-accent) 32%);

				svg {
					height: 90%;
					fill: var(--mo-color-accent);
				}

				div {
					font-size: min(1.8em, 24px);
				}
			}

			mo-button {
				--mo-button-accent-color: var(--mo-color-foreground);
				&[data-router-selected] {
					background: color-mix(in srgb, var(--mo-color-background), var(--mo-color-accent) 16%);
					--mo-button-accent-color: color-mix(in srgb, var(--mo-color-foreground), var(--mo-color-accent) 16%);
				}
			}
		`
	}

	override get template() {
		return html`
			${this.logoTemplate}
			${this.navigationTemplate}
		`
	}

	protected get logoTemplate() {
		return html`
			<mo-flex id='logo' direction='horizontal' alignItems='center'>
				<svg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 1999.06 1999.06'>
					<defs>
						<style>
							path {
								fill: none;
								stroke: currentColor;
								stroke-miterlimit: 10;
								stroke-width: 100px;
							}
						</style>
					</defs>
					<path d='M848.4,1757.09c251,0,454.51-203.5,454.51-454.52' />
					<path d='M848.4,848.91c251,0,454.51,203.49,454.51,454.51' />
					<path d='M393.88,1302.57c251,0,454.52,203.5,454.52,454.52C597.37,1757.09,393.88,1553.59,393.88,1302.57Z' />
					<path d='M996.53,288.93c-251,0-454.51,203.49-454.51,454.51' />
					<path d='M996.53,1197.1c-251,0-454.51-203.49-454.51-454.51' />
					<path d='M1451,743.44c-251,0-454.51-203.49-454.51-454.51C1247.55,288.93,1451,492.42,1451,743.44Z' />
					<path d='M1605.18,495.6c-225.15,111-497.65,18.47-608.65-206.67C1221.68,177.93,1494.18,270.46,1605.18,495.6Z' />
				</svg>
				<div>Solidarity</div>
			</mo-flex>
		`
	}

	private get navigationTemplate() {
		return html`
			<mo-flex>
				<mo-button ${routerLink(new Solid.PageCampaigns)}>Campaigns</mo-button>
			</mo-flex>
		`
	}
}

declare global {
	interface HTMLElementTagNameMap {
		'solid-application': Solidarity
		'solid-header': Header
	}
}