import { component, DataGridColumn, html, ifDefined } from '@3mo/modelx'
import { CampaignAllocationEntryType } from 'sdk'

@component('solid-data-grid-column-campaign-allocation-entry-type')
export class DataGridColumnCampaignAllocationEntryType<TData> extends DataGridColumn<TData, CampaignAllocationEntryType> {
	constructor() {
		super()
		this.width = '60px'
	}

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