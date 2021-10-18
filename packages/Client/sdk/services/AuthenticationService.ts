import { API } from 'sdk'

export class AuthenticationService {
	static async isAuthenticated() {
		return !!API.token && await API.get<boolean>(`/authentication/check`)
	}

	static async authenticateWithPassword(username: string, password: string) {
		API.token = await API.get<string>(`/authentication/password?username=${username}&password=${password}`)
	}

	static unauthenticate() {
		API.token = undefined
	}
}