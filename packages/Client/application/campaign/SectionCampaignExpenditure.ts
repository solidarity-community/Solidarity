import { Component, component, css, html, nothing, property } from '@3mo/model'
import { Campaign, CampaignExpenditure } from 'sdk'

@component('solid-section-campaign-expenditure')
export class SectionCampaignExpenditure extends Component {
	@property({ type: Object }) campaign!: Campaign
	@property({ type: Boolean }) editable = false

	static override get styles() {
		return css`
			.symbol {
				color: var(--mo-color-gray);
			}

			.price {
				place-self: end;
			}

			.sum {
				font-weight: bold;
			}

			.total {
				color: var(--mo-accent);
				font-size: var(--mo-font-size-l);
			}
		`
	}

	protected override get template() {
		return html`
			<mo-section heading='Expenditure'>
				${!this.editable ? nothing : html`
					<mo-icon-button slot='action' icon='add'
						@click=${() => { this.campaign.expenditures.push(new CampaignExpenditure); this.requestUpdate() }}
					></mo-icon-button>
				`}

				<mo-grid alignItems='end' columns='3* * auto 2* auto 2*' gap='10px'>
					${this.campaign.expenditures.map(expenditure => html`
						${this.getNameTemplate(expenditure)}
						${this.getQuantityTemplate(expenditure)}
						<mo-div class='symbol'> × </mo-div>
						${this.getUnitPriceTemplate(expenditure)}
						<mo-div class='symbol'> = </mo-div>
						<solid-amount class='sum price'
							value=${expenditure.unitPrice * expenditure.quantity}
						></solid-amount>
					`)}
					<mo-div class='total' gridColumn='1 / -2'>Sum</mo-div>
					<solid-amount class='total sum price'
						value=${this.campaign.totalExpenditure}
					></solid-amount>
				</mo-grid>
			</mo-section>
		`
	}

	private getNameTemplate(expenditure: CampaignExpenditure) {
		return this.editable ? html`
			<mo-field-text dense label='Item name'
				value=${expenditure.name}
				@change=${(e: CustomEvent<string>) => expenditure.name = e.detail}
			></mo-field-text>
		` : html`
			<mo-div>${expenditure.name}</mo-div>
		`
	}

	private getQuantityTemplate(expenditure: CampaignExpenditure) {
		return this.editable ? html`
			<mo-field-number dense label='Quantity'
				value=${expenditure.quantity}
				@change=${(e: CustomEvent<number>) => { expenditure.quantity = e.detail; this.requestUpdate() }}
			></mo-field-number>
		` : html`
			<mo-div class='price'>${expenditure.quantity}</mo-div>
		`
	}

	private getUnitPriceTemplate(expenditure: CampaignExpenditure) {
		return this.editable ? html`
			<solid-field-amount dense label='Unit Price'
				value=${expenditure.unitPrice}
				@change=${(e: CustomEvent<number>) => { expenditure.unitPrice = e.detail; this.requestUpdate() }}
			></solid-field-amount>
		` : html`
			<solid-amount class='price' value=${expenditure.unitPrice}></solid-amount>
		`
	}
}

declare global {
	interface HTMLElementTagNameMap {
		'solid-section-campaign-expenditure': SectionCampaignExpenditure
	}
}