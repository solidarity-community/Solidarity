export const enum HealthCheckStatus {
	Healthy = 'Healthy',
	Degraded = 'Degraded',
	Unhealthy = 'Unhealthy',
}

export type Health = {
	readonly status: HealthCheckStatus,
	readonly checks: Array<HealthCheck>
}

export type HealthCheck = {
	readonly key: string,
	readonly status: HealthCheckStatus,
	readonly description: string,
	readonly duration: string,
}