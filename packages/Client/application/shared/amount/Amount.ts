import { FormatHelper, component, Amount as AmountBase } from '@3mo/model'
import { amountComponent, IAmountComponent } from './amountComponent'
import { amountModeStorage } from './amountModeStorage'

@component('solid-amount')
@amountComponent()
export class Amount extends AmountBase implements IAmountComponent {
	override readonly currencySymbol = amountModeStorage.getCurrencySymbol()

	protected override get amountText() {
		const value = amountModeStorage.getAmountBySatoshi(this.value)
		return FormatHelper.amount(value, {
			signDisplay: this.signDisplay,
			minimumFractionDigits: amountModeStorage.getFractionDigits(),
			maximumFractionDigits: amountModeStorage.getFractionDigits(),
		})
	}

	protected override get currencySymbolText() {
		return amountModeStorage.getCurrencySymbol()
	}
}

declare global {
	interface HTMLElementTagNameMap {
		'solid-amount': Amount
	}
}