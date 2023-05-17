import { Api, Health } from 'sdk'

export class HealthService {
	static get() {
		return Api.get<Health>('/health', { noHttpErrorOnErrorStatusCode: true })
	}
}