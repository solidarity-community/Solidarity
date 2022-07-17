import { Component, html, TemplateResult } from '@3mo/modelx'

export abstract class Progress extends Component {
	protected abstract get progress(): number
	protected abstract get heading(): string
	protected abstract get value(): string | TemplateResult<1>

	protected override get template() {
		return html`
			<mo-flex gap='2px'>
				<mo-flex direction='horizontal' justifyContent='space-between' alignItems='center'>
					<mo-div fontSize='var(--mo-font-size-s)' foreground='var(--mo-color-gray)'>${this.heading}</mo-div>
					<mo-div>${this.value}</mo-div>
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