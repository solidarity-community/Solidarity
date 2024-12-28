import { type Component, Controller } from '@a11d/lit'
import { amountModeStorage } from './amountModeStorage'

export class AmountController extends Controller {
	constructor(protected readonly amountComponent: Component, protected readonly callback?: () => void) {
		super(amountComponent)
	}

	override hostConnected() {
		amountModeStorage.changed.subscribe(this.handleChange)
	}

	override hostDisconnected() {
		amountModeStorage.changed.unsubscribe(this.handleChange)
	}

	private handleChange = () => {
		this.amountComponent.requestUpdate()
		this.callback?.()
	}
}