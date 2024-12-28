import { Component, component, html, event, state, query } from '@a11d/lit'
import { type FileUpload } from '@3mo/file-upload'
import { File } from 'application'

/** @fires change CustomEvent<string> */
@component('solid-button-file-upload')
export class ButtonFileUpload extends Component {
	@event() readonly change!: EventDispatcher<string | undefined>

	@state() private fileName?: string
	@state() private isUploading = false

	@query('mo-file-upload') readonly fileUploadElement!: FileUpload<string>

	get file() {
		return this.fileUploadElement.file
	}

	uploadSelectedFile() {
		return this.fileUploadElement.uploadFile()
	}

	protected override get template() {
		return html`
			<mo-loading-button leadingIcon='upload_file' preventClickEventInference
				?loading=${this.isUploading}
				@click=${() => this.renderRoot.querySelector('mo-file-upload')?.openExplorer()}
			>${this.fileName ?? 'Upload File'}</mo-loading-button>

			<mo-file-upload
				.upload=${(file: globalThis.File) => File.save(file)}
				@fileChange=${(e: CustomEvent<globalThis.File | undefined>) => this.fileName = e.detail?.name}
				@uploadingChange=${() => this.isUploading = true}
				@change=${(e: CustomEvent<string | undefined>) => this.change.dispatch(e.detail)}
			></mo-file-upload>
		`
	}
}

declare global {
	interface HTMLElementTagNameMap {
		'solid-button-file-upload': ButtonFileUpload
	}
}