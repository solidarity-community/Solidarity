import { Application, application, component, html, ThemeHelper, Color } from '@3mo/model'
import { DialogAuthenticator, PageAccount } from 'application'
import * as Solid from '.'

ThemeHelper.accent.value = new Color([105, 69, 130], [92, 119, 185])

@application()
@component('solid-application')
export class Solidarity extends Application {
	protected override get drawerTemplate() {
		return html`
			<mo-navigation-list-item icon='campaign' .component=${new Solid.PageCampaigns}>Campaigns</mo-navigation-list-item>
		`
	}

	protected override get userAvatarMenuItemsTemplate() {
		return !DialogAuthenticator.authenticatedUser.value ? html`
			<mo-navigation-list-item icon='login' .component=${new DialogAuthenticator}>Login or Register</mo-navigation-list-item>
		` : html`
			<mo-navigation-list-item icon='account_circle' .component=${new PageAccount}>Account</mo-navigation-list-item>
		`
	}
}