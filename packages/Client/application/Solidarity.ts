import { component, html } from '@a11d/lit'
import { Application, application, routerLink } from '@a11d/lit-application'
import { ColorSet, Theme } from '@3mo/theme'
import { DataGrid } from '@3mo/data-grid'
import * as Solid from '.'
import { Api } from '@a11d/api'

Theme.accent.value = new ColorSet('rgb(104, 159, 56)', 'rgb(124, 179, 76)', 'rgb(144, 199, 96)')
DataGrid.hasAlternatingBackground.value = false

// TODO: Re-design the application shell

@application()
@component('solid-application')
export class Solidarity extends Application {
	protected get drawerTemplate() {
		return html`
			<mo-navigation-list-item icon='campaign' ${routerLink(new Solid.PageCampaigns)}>Campaigns</mo-navigation-list-item>
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