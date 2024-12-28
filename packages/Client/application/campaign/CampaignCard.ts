import { component, Component, css, html, property, style } from '@a11d/lit'
import { type Campaign, File } from 'application'

@component('solid-campaign-card')
export class CampaignCard extends Component {
	@property({ type: Object }) campaign!: Campaign

	static override get styles() {
		return css`
			:host {
				aspect-ratio: 16 / 11;
				cursor: pointer;
			}

			[slot=media] {
				position: relative;
				aspect-ratio: 16 / 9;
			}

			#placeholder {
				display: flex !important;
				background: color-mix(in srgb, var(--mo-color-surface), var(--mo-color-accent) 16%);
				border-bottom: 1px solid var(--mo-color-transparent-gray-3);

				mo-icon {
					font-size: 75px;
					color: color-mix(in srgb, var(--mo-color-foreground), var(--mo-color-accent) 50%);
				}
			}

		`
	}

	protected override get template() {
		return html`
			<mo-card>
				${!this.campaign.coverImageUri ? html`
					<mo-flex id='placeholder' slot='media' alignItems='center' justifyContent='center'>
						<mo-icon icon='campaign'></mo-icon>
					</mo-flex>
				` : html`
					<img slot='media' src=${File.getPath(this.campaign.coverImageUri)} />
				`}
				<div slot='heading' ${style({ textAlign: 'center', fontSize: 'large' })}>${this.campaign.title}</div>
				<div ${style({ color: 'var(--mo-color-gray)' })}>${this.campaign.description}</div>
				<solid-campaign-progress slot='footer' .campaign=${this.campaign}></solid-campaign-progress>
			</mo-card>
		`
	}
}

declare global {
	interface HTMLElementTagNameMap {
		'solid-campaign-card': CampaignCard
	}
}