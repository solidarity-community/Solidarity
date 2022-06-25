export const enum HealthCheckStatus { Unhealthy, Degraded, Healthy }

export type Health = {
	readonly status: HealthCheckStatus,
	readonly checks: Array<HealthCheck>
}

export type HealthCheck = {
	readonly key: string,
	readonly status: HealthCheckStatus,
	readonly checks?: Array<HealthCheck>
}