import { component, Component, css, html, property, style } from '@a11d/lit'
import { Campaign, FileService } from 'sdk'

@component('solid-campaign-card')
export class CampaignCard extends Component {
	@property({ type: Object }) campaign!: Campaign

	static override get styles() {
		return css`
			:host {
				aspect-ratio: 16 / 11;
			}

			[slot=media] {
				position: relative;
				aspect-ratio: 16 / 9;
			}

			mo-icon {
				position: absolute;
				left: 50%;
				top: 50%;
				transform: translate(-50%, -50%);
				font-size: 75px;
			}
		`
	}

	protected override get template() {
		return html`
			<mo-card cursor='pointer'>
				${!this.campaign.coverImageUri ? html`
					<mo-flex slot='media' alignItems='center' justifyContent='center' ${style({ background: 'var(--mo-color-accent-gradient-transparent)' })}>
						<mo-icon icon='campaign' ${style({ color: 'var(--mo-color-gray)' })}></mo-icon>
					</mo-flex>
				` : html`
					<img slot='media' src=${FileService.getPath(this.campaign.coverImageUri)} />
				`}
				<div slot='heading' ${style({ textAlign: 'center', fontSize: 'large', fontWeight: 'bold' })}>${this.campaign.title}</div>
				<div>${this.campaign.description}</div>
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