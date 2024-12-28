import { component, html, ifDefined } from '@a11d/lit'
import { DataGridColumnComponent } from '@3mo/data-grid'
import { type CampaignAllocationEntryType } from 'application'

@component('solid-data-grid-column-campaign-allocation-entry-type')
export class DataGridColumnCampaignAllocationEntryType<TData> extends DataGridColumnComponent<TData, CampaignAllocationEntryType> {
	override getContentTemplate(value?: CampaignAllocationEntryType) {
		return html`<solid-campaign-allocation-entry-type-label type=${ifDefined(value)}></solid-campaign-allocation-entry-type-label>`
	}

	override getEditContentTemplate(value?: CampaignAllocationEntryType) {
		return this.getContentTemplate(value)
	}
}

declare global {
	interface HTMLElementTagNameMap {
		'solid-data-grid-column-campaign-allocation-entry-type': DataGridColumnCampaignAllocationEntryType<unknown>
	}
}