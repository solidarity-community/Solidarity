import { component, html, ifDefined, property } from '@a11d/lit'
import { DataGridColumn } from '@3mo/data-grid'
import { CampaignAllocationEntryType } from 'sdk'

@component('solid-data-grid-column-campaign-allocation-entry-type')
export class DataGridColumnCampaignAllocationEntryType<TData> extends DataGridColumn<TData, CampaignAllocationEntryType> {
	@property() override width = '60px'

	getContentTemplate(value?: CampaignAllocationEntryType) {
		return html`<solid-campaign-allocation-entry-type-label type=${ifDefined(value)}></solid-campaign-allocation-entry-type-label>`
	}

	getEditContentTemplate(value?: CampaignAllocationEntryType) {
		return this.getContentTemplate(value)
	}
}

declare global {
	interface HTMLElementTagNameMap {
		'solid-data-grid-column-campaign-allocation-entry-type': DataGridColumnCampaignAllocationEntryType<unknown>
	}
}