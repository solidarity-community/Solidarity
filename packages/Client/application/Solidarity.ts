import { Application, application, component, html, ThemeHelper, Color } from '@3mo/model'
import { PageAccount } from 'application'
import * as Pages from './campaign/pages'

ThemeHelper.accent.value = new Color([105, 69, 130], [92, 119, 185])

@application
@component('solid-application')
export class Solidarity extends Application {
	protected get drawerTemplate() {
		return html`
			<mo-drawer-item icon='campaign' .component=${new Pages.PageCampaigns}>Campaigns</mo-drawer-item>
		`
	}

	protected override get userAvatarMenuItemsTemplate() {
		return html`
			<mo-drawer-item icon='account_circle' ?hidden=${!this.authenticatedUser} .component=${new PageAccount()}>Account</mo-drawer-item>
		`
	}
}