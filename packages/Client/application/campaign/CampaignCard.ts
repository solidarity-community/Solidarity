import { component, Component, css, html, property } from '@3mo/model'
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
					<mo-flex slot='media' alignItems='center' justifyContent='center' background='var(--mo-accent-transparent)'>
						<mo-icon icon='campaign' foreground='var(--mo-color-gray)'></mo-icon>
					</mo-flex>
				` : html`
					<img slot='media' src=${FileService.getPath(this.campaign.coverImageUri)} />
				`}
				<mo-div slot='heading' textAlign='center' fontSize='var(--mo-font-size-l)' fontWeight='bold'>${this.campaign.title}</mo-div>
				<mo-div>${this.campaign.description}</mo-div>
				<mo-flex slot='footer' direction='horizontal' justifyContent='space-around' padding='0 0 8px 0'>
					<mo-div>Graph 1</mo-div>
					<mo-div>Graph 2</mo-div>
				</mo-flex>
			</mo-card>
		`
	}
}

declare global {
	interface HTMLElementTagNameMap {
		'solid-campaign-card': CampaignCard
	}
}