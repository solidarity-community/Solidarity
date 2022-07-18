import { Controller, ReactiveControllerHost } from '@3mo/modelx'

export class ResizeController extends Controller {
	protected observer?: ResizeObserver

	constructor(
		protected override readonly host: ReactiveControllerHost & Element,
		private readonly callback: ResizeObserverCallback,
		private readonly options?: ResizeObserverOptions,
	) { super(host) }

	override hostConnected() {
		this.observer = new ResizeObserver(this.callback)
		this.observer.observe(this.host, this.options)
	}

	override hostDisconnected() {
		this.observer?.disconnect()
	}
}