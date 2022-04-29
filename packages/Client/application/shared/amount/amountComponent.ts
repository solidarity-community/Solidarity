import { Component } from '@3mo/model'
import { amountModeStorage } from './amountModeStorage'

export interface IAmountComponent extends Component {
	amountModeStorageChangeHandler?: () => void
}

export const amountComponent = () => {
	return (componentConstructor: Constructor<Component>) => {
		const Constructor = componentConstructor as Constructor<IAmountComponent>

		const connectedCallback = Constructor.prototype.connectedCallback
		Constructor.prototype.connectedCallback = function (this: IAmountComponent) {
			amountModeStorage.changed.subscribe(this.amountModeStorageChangeHandler ?? (this.amountModeStorageChangeHandler = () => this.requestUpdate()))
			connectedCallback.apply(this)
		}

		const disconnectedCallback = Constructor.prototype.disconnectedCallback
		Constructor.prototype.disconnectedCallback = function (this: IAmountComponent) {
			disconnectedCallback.apply(this)
			amountModeStorage.changed.unsubscribe(this.amountModeStorageChangeHandler!)
		}
	}
}