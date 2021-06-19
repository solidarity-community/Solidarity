import { Application, application, component, html } from '@3mo/model/library'
import { ThemeHelper } from '@3mo/model/helpers'
import { Color } from '@3mo/model/types'
import * as Pages from './pages'

ThemeHelper.accent.value = new Color([105, 69, 130], [92, 119, 185])

@application
@component('solid-application')
export class Solidarity extends Application {
	get drawerTemplate() {
		return html`
			<mo-drawer-list>
				<mo-drawer-item icon='home' .component=${new Pages.PageHome}>Home</mo-drawer-item>
			</mo-drawer-list>
		`
	}
}