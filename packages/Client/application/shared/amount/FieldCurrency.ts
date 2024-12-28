import { component } from '@a11d/lit'
import { FieldCurrency as FieldCurrencyBase } from '@3mo/number-fields'
import { amountModeStorage } from '.'
import { AmountController } from './AmountController'

@component('solid-field-currency')
export class FieldCurrency extends FieldCurrencyBase {
	protected readonly amountController = new AmountController(this)

	protected override format = (value: number) => {
		return amountModeStorage.getAmountBySatoshi(value).formatAsCurrency(undefined, {
			minimumFractionDigits: amountModeStorage.getFractionDigits(),
			maximumFractionDigits: amountModeStorage.getFractionDigits(),
		})
	}

	protected override handleInput(value?: number, e?: Event) {
		const number = value === undefined ? undefined : amountModeStorage.getSatoshiByAmount(value)
		return super.handleInput(number, e)
	}

	protected get currencySymbolText() {
		return amountModeStorage.getCurrencySymbol()
	}
}

declare global {
	interface HTMLElementTagNameMap {
		'solid-field-currency': FieldCurrency
	}
}