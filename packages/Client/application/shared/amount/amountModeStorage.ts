import { LocalStorage } from '@a11d/local-storage'

export const enum AmountMode {
	Satoshi = 1,
	Bitcoin = 100_000_000,
}

export const amountModeStorage = new class extends LocalStorage<AmountMode> {
	constructor() {
		super('Solidarity.Amount.Mode', AmountMode.Satoshi)
	}

	getAmountBySatoshi(satoshiAmount: number) {
		return satoshiAmount / this.value
	}

	getSatoshiByAmount(amount: number) {
		return amount * this.value
	}

	getCurrencySymbol() {
		return this.value === AmountMode.Bitcoin ? '₿' : 'sat'
	}

	getFractionDigits() {
		return Math.log10(this.value)
	}
}