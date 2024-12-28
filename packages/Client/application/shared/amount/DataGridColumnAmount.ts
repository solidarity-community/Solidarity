import { DataGridColumnNumberBase } from '@3mo/data-grid/dist/columns/number/DataGridColumnNumberBase'
import { component, html } from '@a11d/lit'

@component('solid-data-grid-column-amount')
export class DataGridColumnAmount<TData> extends DataGridColumnNumberBase<TData> {
	getContentTemplate(value?: number) {
		return html`<solid-amount value=${value ?? 0}></solid-amount>`
	}

	getEditContentTemplate(value?: number | undefined) {
		return this.getContentTemplate(value)
	}

	getSumTemplate(sum: number) {
		return html`${sum}`
	}
}

declare global {
	interface HTMLElementTagNameMap {
		'solid-data-grid-column-amount': DataGridColumnAmount<unknown>
	}
}