import { ReactiveControllerHost, Controller } from '@3mo/model'

export class IntervalController extends Controller {
	private timerId = -1

	constructor(protected override readonly host: ReactiveControllerHost & HTMLElement, private readonly period: TimeSpan, private readonly tickTask: () => void | Promise<void>) {
		super(host)
	}

	override hostConnected() {
		super.hostConnected?.()
		this.handleTick()
		this.timerId = window.setInterval(this.handleTick, this.period.milliseconds)
	}

	override hostDisconnected() {
		super.hostDisconnected?.()
		window.clearInterval(this.timerId)
	}

	private readonly handleTick = async () => {
		await this.tickTask()
	}
}