import { Component, Controller } from '@a11d/lit'
import { amountModeStorage } from './amountModeStorage'

export class AmountController extends Controller {
	constructor(protected readonly amountComponent: { amountModeChanged?: () => void } & Component) {
		super(amountComponent)
	}

	override hostConnected() {
		amountModeStorage.changed.subscribe(this.callback)
	}

	override hostDisconnected() {
		amountModeStorage.changed.unsubscribe(this.callback)
	}

	private callback = () => this.amountComponent.amountModeChanged?.()
}