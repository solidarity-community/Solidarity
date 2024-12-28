import { component, Component, property, css, html } from '@a11d/lit'
import { AmountController } from './AmountController'
import { amountModeStorage } from './amountModeStorage'

@component('solid-amount')
export class Amount extends Component {
	protected readonly amountController = new AmountController(this)

	@property({ type: Number }) value = 0

	static override get styles() {
		return css`
			div {
				white-space: nowrap;
				text-overflow: ellipsis;
				overflow: hidden;
				color: var(--mo-amount-color);
			}

			:host([negative][redNegative]) div {
				color: var(--mo-color-error);
			}

			span {
				color: inherit;
			}
		`
	}

	protected override get template() {
		return html`
			<div>
				<span>${this.amountText}</span>
				<span>${this.currencySymbolText}</span>
			</div>
		`
	}

	protected get amountText() {
		const value = amountModeStorage.getAmountBySatoshi(this.value)
		return value.formatAsCurrency(undefined, {
			minimumFractionDigits: amountModeStorage.getFractionDigits(),
			maximumFractionDigits: amountModeStorage.getFractionDigits(),
		})
	}

	protected get currencySymbolText() {
		return amountModeStorage.getCurrencySymbol()
	}
}

declare global {
	interface HTMLElementTagNameMap {
		'solid-amount': Amount
	}
}