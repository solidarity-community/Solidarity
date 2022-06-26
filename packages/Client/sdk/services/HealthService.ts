import { API, Health } from 'sdk'

export class HealthService {
	static get() {
		return API.get<Health>('/health', { noHttpErrorOnErrorStatusCode: true })
	}
}