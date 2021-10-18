import { Application, application, component, html, ThemeHelper, Color } from '@3mo/model'
import * as Pages from './campaign/pages'

ThemeHelper.accent.value = new Color([105, 69, 130], [92, 119, 185])

@application
@component('solid-application')
export class Solidarity extends Application {
	get drawerTemplate() {
		return html`
			<mo-drawer-item icon='campaign' .component=${new Pages.PageCampaigns}>Campaigns</mo-drawer-item>
		`
	}
}