import { component, html, ifDefined, style } from '@a11d/lit'
import { DialogComponent } from '@a11d/lit-application'
import { type Campaign, type CampaignAllocationEntry } from 'application'

@component('solid-dialog-campaign-allocations')
export class DialogCampaignAllocations extends DialogComponent<{ readonly campaign: Campaign }> {
	protected override get template() {
		return html`
			<mo-dialog size='medium' heading='Allocations'>
				<style>
					mo-dialog::part(content) {
						padding: 0;
					}
				</style>
				<mo-data-grid .data=${this.parameters.campaign.allocation?.entries ?? []}>
					<solid-data-grid-column-campaign-allocation-entry-type heading='Type' dataSelector=${getKeyPath<CampaignAllocationEntry>('type')}></solid-data-grid-column-campaign-allocation-entry-type>
					<solid-data-grid-column-payment-method heading='Payment Method' dataSelector=${getKeyPath<CampaignAllocationEntry>('paymentMethodIdentifier')}></solid-data-grid-column-payment-method>
					<mo-data-grid-column-text width='*' heading='Transaction ID' dataSelector=${getKeyPath<CampaignAllocationEntry>('data')}></mo-data-grid-column-text>
					<solid-data-grid-column-amount width='100px' heading='Amount' dataSelector=${getKeyPath<CampaignAllocationEntry>('amount')}></solid-data-grid-column-amount>

					<mo-data-grid-footer-sum slot='sum' heading='Allocated Refunds' ${style({ color: 'var(--mo-color-accent)' })}>
						<solid-amount value=${ifDefined(this.parameters.campaign.allocation?.totalRefundAmount)} ${style({ fontWeight: 'bold' })}></solid-amount>
					</mo-data-grid-footer-sum>

					<mo-data-grid-footer-sum slot='sum' heading='Allocated Funds' ${style({ color: 'var(--mo-color-accent)' })}>
						<solid-amount value=${ifDefined(this.parameters.campaign.allocation?.totalFundAmount)} ${style({ fontWeight: 'bold' })}></solid-amount>
					</mo-data-grid-footer-sum>
				</mo-data-grid>
			</mo-dialog>
		`
	}
}