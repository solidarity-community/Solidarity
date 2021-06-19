import { component, homePage, html, PageComponent, route } from '@3mo/model/library'

@homePage
@route('/home')
@component('solid-page-home')
export class PageHome extends PageComponent {
	protected override render() {
		return html`
			<mo-page header='Home'>
				Home Page
			</mo-page>
		`
	}
}