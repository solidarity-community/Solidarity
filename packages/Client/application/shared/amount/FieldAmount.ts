import { component, FieldAmount as FieldAmountBase, FormatHelper } from '@3mo/modelx'
import { amountModeStorage } from '.'
import { amountComponent, IAmountComponent } from './amountComponent'

@component('solid-field-amount')
@amountComponent()
export class FieldAmount extends FieldAmountBase implements IAmountComponent {
	amountModeStorageChangeHandler = () => this.value = this._value

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