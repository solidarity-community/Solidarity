import { component, html } from '@a11d/lit'
import { DataGridColumn } from '@3mo/data-grid'

@component('solid-data-grid-column-clipboard-text-block')
export class DataGridColumnClipboardTextBlock<TData> extends DataGridColumn<TData, string> {
	getContentTemplate(value?: string) {
		return html`<solid-clipboard-text-block>${value}</solid-clipboard-text-block>`
	}

	getEditContentTemplate(value?: string | undefined) {
		return this.getContentTemplate(value)
	}
}

declare global {
	interface HTMLElementTagNameMap {
		'solid-data-grid-column-clipboard-text-block': DataGridColumnClipboardTextBlock<unknown>
	}
}