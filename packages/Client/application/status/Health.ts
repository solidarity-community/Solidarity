import { Api } from 'application'

export const enum HealthCheckStatus { Unhealthy, Degraded, Healthy }

export class Health {
	static async get() {
		const health = await Api.get<Health>('/health', { noHttpErrorOnErrorStatusCode: true })
		return new Health(health)
	}

	constructor(init?: Partial<Health>) {
		Object.assign(this, init)
	}

	readonly status!: HealthCheckStatus
	readonly checks!: Array<HealthCheck>
}

export type HealthCheck = {
	readonly key: string
	readonly status: HealthCheckStatus
	readonly checks: Array<HealthCheck>
}