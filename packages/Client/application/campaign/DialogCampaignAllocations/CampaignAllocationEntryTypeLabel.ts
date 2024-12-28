import { Component, component, html, property, css, style } from '@a11d/lit'
import { CampaignAllocationEntryType } from 'application'

@component('solid-campaign-allocation-entry-type-label')
export class CampaignAllocationEntryTypeLabel extends Component {
	@property({ type: Number, reflect: true }) type!: CampaignAllocationEntryType

	static override get styles() {
		return css`
			:host {
				display: inline-block;
				max-width: 60px;
				height: 25px;
			}

			:host([type="${CampaignAllocationEntryType.Fund}"]) mo-flex {
				background: var(--mo-color-accent-transparent);
				color: var(--mo-color-accent);
			}

			:host([type="${CampaignAllocationEntryType.Refund}"]) mo-flex {
				background: rgba(var(--mo-color-red-base), 0.12);
				color: rgba(var(--mo-color-red-base), 1);
			}
		`
	}

	protected override get template() {
		return this.type === undefined ? html.nothing : html`
			<mo-flex alignItems='center' justifyContent='center' ${style({ height: '100%', padding: '0px 10px', borderRadius: 'var(--mo-border-radius)' })}>
				${this.type === CampaignAllocationEntryType.Fund ? 'Fund' : 'Refund'}
			</mo-flex>
		`
	}
}

declare global {
	interface HTMLElementTagNameMap {
		'solid-campaign-allocation-entry-type-label': CampaignAllocationEntryTypeLabel
	}
}