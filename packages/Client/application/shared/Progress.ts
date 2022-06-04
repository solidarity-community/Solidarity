import { Component, css, html, TemplateResult } from '@3mo/model'

export abstract class Progress extends Component {
	protected abstract get progress(): number
	protected abstract get progressTemplate(): string | TemplateResult<1>

	static override get styles() {
		return css`
			:host {
				display: block;
				width: 75px;
				height: 75px;
				aspect-ratio: 1 / 1;
				--mdc-circular-progress-track-color: var(--mo-color-transparent-gray-3);
			}

			mo-div {
				position: absolute;
				left: 50%;
				top: 50%;
				transform: translate(-50%, -50%);
				white-space: nowrap;
				text-align: center;
			}

			mo-circular-progress {
				aspect-ratio: 1 / 1;
				width: 100%;
				height: 100%;
			}
		`
	}

	protected override get template() {
		return html`
			<mo-flex alignItems='center' justifyContent='center' position='relative' width='100%' height='100%'>
				<mo-circular-progress progress=${this.progress}></mo-circular-progress>
				<mo-div>${this.progressTemplate}</mo-div>
			</mo-flex>
		`
	}
}