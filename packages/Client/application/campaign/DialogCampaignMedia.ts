import { cache, component, DialogComponent, html, ifDefined, query, state } from '@3mo/modelx'
import { CampaignMedia, CampaignMediaType, CampaignMediaService, Campaign } from 'sdk'
import { ButtonFileUpload } from 'application'

@component('solid-dialog-campaign-media')
export class DialogCampaignMedia extends DialogComponent<{ readonly campaign?: Campaign }, CampaignMedia> {
	@state() private mediaType?: CampaignMediaType
	@state() private content?: string

	@query('solid-button-file-upload') private readonly fileUploadButtonElement?: ButtonFileUpload

	protected override get template() {
		return html`
			<mo-dialog heading='Add Media' primaryButtonText=${this.mediaType === undefined ? 'Continue' : 'Save'}>
				${cache(this.contentTemplate)}
			</mo-dialog>
		`
	}

	private get contentTemplate() {
		switch (this.mediaType) {
			case undefined:
				return this.selectionTemplate
			case CampaignMediaType.File:
				return this.fileMediaTemplate
			case CampaignMediaType.YouTube:
				return this.youTubeMediaTemplate
			case CampaignMediaType.Twitch:
				return this.twitchMediaTemplate
		}
	}

	private get selectionTemplate() {
		return html`
			<mo-flex direction='horizontal' gap='10px'>
				<mo-button type='outlined' height='50px' @click=${() => this.mediaType = CampaignMediaType.File}>Image file</mo-button>
				<mo-button type='outlined' height='50px' @click=${() => this.mediaType = CampaignMediaType.YouTube}>YouTube video</mo-button>
				<mo-button type='outlined' height='50px' @click=${() => this.mediaType = CampaignMediaType.Twitch}>Twitch video</mo-button>
			</mo-flex>
		`
	}

	private get fileMediaTemplate() {
		return html`
			<solid-button-file-upload @upload=${(e: CustomEvent<string>) => this.content = e.detail}></solid-button-file-upload>
		`
	}

	private get youTubeMediaTemplate() {
		return html`
			<mo-field-text label='YouTube video link'
				value=${ifDefined(this.content)}
				@change=${(e: CustomEvent<string>) => this.content = e.detail}
			></mo-field-text>
		`
	}

	private get twitchMediaTemplate() {
		return html`
			<mo-field-text label='Twitch video link'
				value=${ifDefined(this.content)}
				@change=${(e: CustomEvent<string>) => this.content = e.detail}
			></mo-field-text>
		`
	}

	protected override async primaryAction() {
		if (this.mediaType === undefined) {
			throw new Error('Media type not selected')
		}

		if (this.mediaType === CampaignMediaType.File) {
			if (!this.fileUploadButtonElement?.file) {
				throw new Error('File not selected')
			}
			await this.fileUploadButtonElement?.uploadSelectedFile()
		}

		if (!this.content) {
			throw new Error('Content not specified')
		}

		return {
			type: this.mediaType,
			uri: CampaignMediaService.extractUriByType(this.content, this.mediaType),
		}
	}
}