import { component, FieldAmount as FieldAmountBase } from '@a11d/lit'
import { amountModeStorage } from '.'
import { amountComponent, IAmountComponent } from './AmountController'

// TODO: Migrate

@component('solid-field-amount')
@amountComponent()
export class FieldAmount extends FieldAmountBase implements IAmountComponent {
	amountModeChanged = () => this.value = this._value

	protected override fromValue(value: number | undefined) {
		return typeof value === 'number' ? FormatHelper.amount(amountModeStorage.getAmountBySatoshi(value), {
			minimumFractionDigits: amountModeStorage.getFractionDigits(),
			maximumFractionDigits: amountModeStorage.getFractionDigits(),
		}) : ''
	}

	protected override toValue(value: string) {
		const number = super.toValue(value)
		return number === undefined ? number : amountModeStorage.getSatoshiByAmount(number)
	}

	protected override get currencySymbolText() {
		return amountModeStorage.getCurrencySymbol()
	}
}

declare global {
	interface HTMLElementTagNameMap {
		'solid-field-amount': FieldAmount
	}
}