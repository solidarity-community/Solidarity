import { component, html } from '@a11d/lit'
import { Application, application } from '@a11d/lit-application'
import { ColorSet, Theme } from '@3mo/theme'
import { DataGrid } from '@3mo/data-grid'
import { DialogAuthentication, PageAccount } from 'application'
import * as Solid from '.'
import './Logo.svg'

Theme.accent.value = new ColorSet('rgb(104, 159, 56)', 'rgb(124, 179, 76)', 'rgb(144, 199, 96)')
DataGrid.hasAlternatingBackground.value = false

// TODO: Migrate

@application()
@component('solid-application')
export class Solidarity extends Application {
	protected get drawerTemplate() {
		return html`
			<mo-navigation-list-item icon='campaign' .component=${new Solid.PageCampaigns}>Campaigns</mo-navigation-list-item>
		`
	}

	protected get userAvatarMenuItemsTemplate() {
		return !DialogAuthentication.authenticatedUser.value ? html`
			<mo-navigation-list-item icon='login' .component=${new DialogAuthentication}>Login or Register</mo-navigation-list-item>
		` : html`
			<mo-navigation-list-item icon='account_circle' .component=${new PageAccount}>Account</mo-navigation-list-item>
		`
	}
}