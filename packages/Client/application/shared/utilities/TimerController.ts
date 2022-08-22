import { Controller, ReactiveControllerHost } from '@3mo/model'

export class TimerController extends Controller {
	private timerId = -1

	constructor(
		protected override readonly host: ReactiveControllerHost & Element,
		private readonly interval: number,
		private readonly callback: () => void
	) { super(host) }

	override hostConnected() {
		this.timerId = window.setInterval(() => this.callback(), this.interval)
	}

	override hostDisconnected() {
		window.clearInterval(this.timerId)
	}
}