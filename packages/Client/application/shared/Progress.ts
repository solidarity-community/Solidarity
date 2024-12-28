import { Component, html, type TemplateResult, style } from '@a11d/lit'

export abstract class Progress extends Component {
	protected abstract get progress(): number
	protected abstract get heading(): string
	protected abstract get value(): string | TemplateResult<1>

	protected override get template() {
		return html`
			<mo-flex gap='2px'>
				<mo-flex direction='horizontal' justifyContent='space-between' alignItems='center'>
					<div ${style({ fontSize: 'small', color: 'var(--mo-color-gray)' })}>${this.heading}</div>
					<div>${this.value}</div>
				</mo-flex>
				${this.progressBarTemplate}
			</mo-flex>
		`
	}

	protected get progressBarTemplate() {
		return html`
			<mo-linear-progress progress=${this.progress}></mo-linear-progress>
		`
	}
}