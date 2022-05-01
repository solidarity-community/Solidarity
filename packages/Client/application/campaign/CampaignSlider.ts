import { Component, component, html, css, property, ifDefined, styleMap, when, query, nothing } from '@3mo/model'
import { Campaign, CampaignMedia, CampaignMediaType, FileService } from 'sdk'
import { DialogCampaignMedia } from 'application'
import { Slider } from '@a11d/lit-slider'

@component('solid-campaign-slider')
export class CampaignSlider extends Component {
	@property({ type: Object }) campaign!: Campaign
	@property({ type: Boolean }) readOnly = false

	@query('lit-slider') readonly sliderElement!: Slider

	static override get styles() {
		return css`
			:host {
				display: block;
				position: relative;
				width: 100%;
				height: 100%;
				min-height: 400px;
			}

			mo-error {
				min-height: 400px;
			}

			lit-slider {
				position: absolute;
				width: 100%;
				height: 100%;
			}

			mo-fab {
				z-index: 1;
				bottom: 58px;
				position: absolute;
			}

			lit-slider::part(previousButton), lit-slider::part(nextButton) {
				background-color: var(--mo-accent);
				border-radius: 100px;
				width: 50px;
				height: 50px;
				transform: translateY(-25px);
				color: white;
				opacity: 1;
				font-weight: 900;
			}
		`
	}

	protected override get template() {
		return html`
			${!this.campaign.media?.length ? html`
				<mo-error icon='collections' height='100%'>
					No media found
				</mo-error>
			` : html`
				<lit-slider hasPagination hasNavigation>
					${this.campaign.media?.map(media => this.getSlideTemplate(media))}
				</lit-slider>
			`}
			${when(!this.readOnly, () => html`
				<mo-fab icon='add' right='8px' @click=${() => this.create()}>Add</mo-fab>
				<mo-fab icon='delete' left='8px' @click=${() => this.deleteSelectedMedia()}>Delete</mo-fab>
			`)}
		`
	}

	private async create() {
		const media = await new DialogCampaignMedia({ campaign: this.campaign }).confirm()
		this.campaign.media.push(media)
		this.requestUpdate()
	}

	private deleteSelectedMedia() {
		const index = this.sliderElement['slider']?.activeIndex
		if (index === undefined) {
			return
		}
		const media = this.campaign.media?.at(index)
		if (media === undefined) {
			return
		}
		this.campaign.media = this.campaign.media?.filter(m => m.id !== media.id) ?? []
		this.requestUpdate()
	}

	private getSlideTemplate(media: CampaignMedia) {
		switch (media.type) {
			case CampaignMediaType.File:
				return this.getFileSlideTemplate(media)
			case CampaignMediaType.YouTube:
				return this.getYouTubeSlideTemplate(media)
			case CampaignMediaType.Twitch:
				return this.getTwitchSlideTemplate(media)
		}
	}

	private getFileSlideTemplate(media: CampaignMedia) {
		return !media.uri ? nothing : html`
			<lit-slide style=${styleMap({ background: `url(${FileService.getPath(media.uri)})` })}></lit-slide>
		`
	}

	private getYouTubeSlideTemplate(media: CampaignMedia) {
		return html`
			<lit-slide>
				<iframe type='text/html' style=${styleMap({ width: '100%', height: '100%' })}
					src=${ifDefined(!media.uri ? undefined : `http://www.youtube.com/embed/${media.uri}`)}
				></iframe>
			</lit-slide>
		`

	}

	private getTwitchSlideTemplate(media: CampaignMedia) {
		return html`
			<lit-slide>
				<iframe type='text/html' style=${styleMap({ width: '100%', height: '100%' })}
					src=${ifDefined(!media.uri ? undefined : `https://player.twitch.tv/?channel=${media.uri}&html5&parent=${window.location.hostname}`)}
				></iframe>
			</lit-slide>
		`
	}
}

declare global {
	interface HTMLElementTagNameMap {
		'solid-campaign-slider': CampaignSlider
	}
}