import { component, DialogComponent, html, ifDefined } from '@3mo/modelx'
import { Campaign, CampaignAllocationEntry } from 'sdk'

@component('solid-dialog-campaign-allocations')
export class DialogCampaignAllocations extends DialogComponent<{ readonly campaign: Campaign }> {
	protected override get template() {
		return html`
			<mo-dialog size='medium' heading='Allocations' primaryButtonText=''>
				<style>
					mo-dialog::part(content) {
						padding: 0;
					}
				</style>
				<mo-data-grid .data=${this.parameters.campaign.allocation?.entries ?? []}>
					<solid-data-grid-column-campaign-allocation-entry-type width='100px' heading='Type' dataSelector=${nameof<CampaignAllocationEntry>('type')}></solid-data-grid-column-campaign-allocation-entry-type>
					<mo-data-grid-column-date width='100px' hidden heading='Date' dataSelector=${nameof<CampaignAllocationEntry>('creation')}></mo-data-grid-column-date>
					<solid-data-grid-column-payment-method heading='Payment Method' dataSelector=${nameof<CampaignAllocationEntry>('paymentMethodIdentifier')}></solid-data-grid-column-payment-method>
					<mo-data-grid-column-text width='*' heading='Transaction ID' dataSelector=${nameof<CampaignAllocationEntry>('data')}></mo-data-grid-column-text>
					<solid-data-grid-column-amount width='100px' heading='Amount' dataSelector=${nameof<CampaignAllocationEntry>('amount')}></solid-data-grid-column-amount>

					<mo-data-grid-footer-sum slot='sum' heading='Allocated Refunds' foreground='var(--mo-accent)'>
						<solid-amount fontWeight='bold' value=${ifDefined(this.parameters.campaign.allocation?.totalRefundAmount)}></solid-amount>
					</mo-data-grid-footer-sum>

					<mo-data-grid-footer-sum slot='sum' heading='Allocated Funds' foreground='var(--mo-accent)'>
						<solid-amount fontWeight='bold' value=${ifDefined(this.parameters.campaign.allocation?.totalFundAmount)}></solid-amount>
					</mo-data-grid-footer-sum>
				</mo-data-grid>
			</mo-dialog>
		`
	}
}