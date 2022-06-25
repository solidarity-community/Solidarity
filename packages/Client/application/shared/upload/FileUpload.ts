import { Component, component, event, html, query, property, NotificationHost } from '@3mo/model'
import { FileService } from 'sdk'

/** 
 * @fires change {CustomEvent<string | undefined>}
 * @fires uploading {CustomEvent}
 * @fires upload {CustomEvent<string>}
 */
@component('solid-file-upload')
export class FileUpload extends Component {
	@event() readonly change!: EventDispatcher<string | undefined>
	@event() readonly uploading!: EventDispatcher
	@event() readonly upload!: EventDispatcher<string>

	@property({ type: Boolean }) uploadOnSelection = false

	@query('input') private readonly inputElement!: HTMLInputElement

	get file() { return this.inputElement.files?.[0] }

	openExplorer() {
		this.inputElement.click()
	}

	async uploadSelectedFile() {
		if (!this.file) {
			return
		}

		try {
			this.uploading.dispatch()
			const path = await FileService.save(this.file)
			this.upload.dispatch(path)
			this.inputElement.value = ''
			this.change.dispatch(undefined)
		} catch (error) {
			NotificationHost.instance.notifyError('Upload has failed. Try again.')
		}
	}

	protected override get template() {
		return html`
			<input type='file' style='display: none' @change=${this.handleChange}>
		`
	}

	protected handleChange = () => {
		this.change.dispatch(this.file?.name)
		if (this.uploadOnSelection) {
			this.uploadSelectedFile()
		}
	}
}

declare global {
	interface HTMLElementTagNameMap {
		'solid-file-upload': FileUpload
	}
}