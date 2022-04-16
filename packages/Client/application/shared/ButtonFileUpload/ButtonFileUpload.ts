import { Component, component, html, event, state, query } from '@3mo/model'
import { FileUpload } from '.'

/** @fires upload CustomEvent<string> */
@component('solid-button-file-upload')
export class ButtonFileUpload extends Component {
	@event() readonly upload!: EventDispatcher<string>

	@state() private fileName?: string
	@state() private isUploading = false

	@query('solid-file-upload') readonly fileUploadElement!: FileUpload

	get file() {
		return this.fileUploadElement.file
	}

	uploadSelectedFile() {
		return this.fileUploadElement.uploadSelectedFile()
	}

	protected override get template() {
		return html`
			<mo-loading-button icon='upload_file' preventClickEventInference
				?loading=${this.isUploading}
				@click=${() => this.shadowRoot.querySelector('solid-file-upload')?.openExplorer()}
			>${this.fileName ?? 'Upload File'}</mo-loading-button>

			<solid-file-upload
				@change=${(e: CustomEvent<string | undefined>) => this.fileName = e.detail}
				@uploading=${(e: CustomEvent) => this.isUploading = true}
				@upload=${(e: CustomEvent<string>) => { this.isUploading = false; this.upload.dispatch(e.detail) }}
			></solid-file-upload>
		`
	}
}

declare global {
	interface HTMLElementTagNameMap {
		'solid-button-file-upload': ButtonFileUpload
	}
}