import { Component, component, html, css, property, when, query, style, choose } from '@a11d/lit'
import { type Campaign, type CampaignMedia, CampaignMediaType, File, DialogCampaignMedia } from 'application'
import { type SwiperContainer } from 'swiper/element'
import { register } from 'swiper/element/bundle'

register()

@component('solid-campaign-media-card')
export class CampaignMediaCard extends Component {
	@property({ type: Object }) campaign!: Campaign
	@property({ type: Boolean }) readOnly = false
	@property({ type: Boolean }) hideWhenEmpty = false

	@query('swiper-container') readonly sliderElement!: SwiperContainer

	static override get styles() {
		return css`
			:host {
				display: block;
				position: relative;
				height: 400px;
			}

			:host([hidden][hideWhenEmpty]) {
				display: none;
			}

			mo-card {
				position: relative;
				height: 100%;
			}

			mo-empty-state {
				height: 100%;
			}

			swiper-container {
				position: absolute;
				width: 100%;
				height: calc(100% - 54px);
				margin: auto 0;

				&::part(button-prev), &::part(button-next) {
					height: 32px;
					width: 32px;
					padding: 8px;
					color: white;
					border-radius: 100px;
					opacity: 1;
					z-index: 3;
				}

				&::part(bullet-active) {
					background-color: var(--mo-color-accent);
				}

				swiper-slide {
					* {
						width: 100%;
						height: 100%;
						object-fit: contain;
						&.cover {
							filter: blur(10px) brightness(0.7);
							position: absolute;
							top: 0;
							left: 0;
							width: 100%;
							height: 100%;
							object-fit: cover;
						}
					}
				}
			}

			mo-fab {
				position: absolute;
				z-index: 1;
				inset-block-end: 16px;
			}

			iframe {
				border: 0;
			}
		`
	}

	protected override get template() {
		this.hidden = !this.campaign.media?.length
		return html`
			<mo-card heading='Media' style='--mo-card-body-padding: 0'>
				${!this.campaign.media?.length ? html`
					<mo-empty-state icon='collections'>
						No media
					</mo-empty-state>
				` : html`
					<swiper-container pagination='true' navigation='true'>
						${this.campaign.media?.map(media => this.getSlideTemplate(media))}
					</swiper-container>
				`}
				${when(!this.readOnly, () => html`
					<mo-fab icon='add' ${style({ insetInlineEnd: '16px' })} @click=${() => this.create()}>Add</mo-fab>
					${when(!!this.campaign.media.length, () => html`
						<mo-fab icon='delete' ${style({ insetInlineStart: '16px' })} @click=${() => this.deleteSelectedMedia()}>Delete</mo-fab>
					`)}
				`)}
			</mo-card>
		`
	}

	private async create() {
		const media = await new DialogCampaignMedia({ campaign: this.campaign }).confirm()
		this.campaign.media.push(media)
		this.requestUpdate()
	}

	private deleteSelectedMedia() {
		const index = this.sliderElement.swiper?.activeIndex
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
		return !media.uri ? html.nothing : html`
			<swiper-slide> ${choose(media.type, [
			[CampaignMediaType.File, () => html`<img src=${File.getPath(media.uri!)}>`],
			[CampaignMediaType.YouTube, () => html`<iframe type='text/html' src=${`http://www.youtube.com/embed/${media.uri}`}></iframe>`],
			[CampaignMediaType.Twitch, () => html`<iframe type='text/html' src=${`https://player.twitch.tv/?channel=${media.uri}&html5&parent=${window.location.hostname}`}></iframe>`]
		])}</swiper-slide>
		`
	}
}

declare global {
	interface HTMLElementTagNameMap {
		'solid-campaign-media-card': CampaignMediaCard
	}
}