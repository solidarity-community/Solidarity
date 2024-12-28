import { Component, component, css, html, property, style } from '@a11d/lit'
import { type Campaign, CampaignExpenditure, CampaignStatus } from 'application'

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
				color: var(--mo-color-accent);
				font-size: large;
			}
		`
	}

	protected override get template() {
		return html`
			<mo-card heading='Expenditure'>
				${!this.editable ? html.nothing : html`
					<mo-icon-button slot='action' icon='add'
						@click=${() => { this.campaign.expenditures.push(new CampaignExpenditure); this.requestUpdate() }}
					></mo-icon-button>
				`}

				<mo-grid alignItems=${this.editable ? 'center' : 'end'} columns='3* * auto 2* auto 2*' gap='10px'>
					${this.campaign.expenditures.map(expenditure => html`
						${this.getNameTemplate(expenditure)}
						${this.getQuantityTemplate(expenditure)}
						<div class='symbol'> × </div>
						${this.getUnitPriceTemplate(expenditure)}
						<div class='symbol'> = </div>
						<solid-amount class='sum price'
							value=${expenditure.unitPrice * expenditure.quantity}
						></solid-amount>
					`)}
					<div class='total' ${style({ gridColumn: '1 / -2' })}>Total</div>
					<solid-amount class='total sum price' value=${this.campaign.totalExpenditure}></solid-amount>
				</mo-grid>
			</mo-card>
		`
	}

	private getNameTemplate(expenditure: CampaignExpenditure) {
		return this.editable ? html`
			<mo-field-text dense label='Item name'
				?readonly=${this.campaign.status !== CampaignStatus.Funding}
				value=${expenditure.name}
				@change=${(e: CustomEvent<string>) => expenditure.name = e.detail}
			></mo-field-text>
		` : html`
			<div>${expenditure.name}</div>
		`
	}

	private getQuantityTemplate(expenditure: CampaignExpenditure) {
		return this.editable ? html`
			<mo-field-number dense label='Quantity'
				?readonly=${this.campaign.status !== CampaignStatus.Funding}
				value=${expenditure.quantity}
				@change=${(e: CustomEvent<number>) => { expenditure.quantity = e.detail; this.requestUpdate() }}
			></mo-field-number>
		` : html`
			<div class='price'>${expenditure.quantity}</div>
		`
	}

	private getUnitPriceTemplate(expenditure: CampaignExpenditure) {
		return this.editable ? html`
			<solid-field-currency dense label='Unit Price'
				?readonly=${this.campaign.status !== CampaignStatus.Funding}
				value=${expenditure.unitPrice}
				@change=${(e: CustomEvent<number>) => { expenditure.unitPrice = e.detail; this.requestUpdate() }}
			></solid-field-currency>
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