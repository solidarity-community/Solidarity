import { API, AuthenticationMethodType } from 'sdk'

export class AuthenticationService {
	static async getAll() {
		const isNewByAuthenticationObject = await API.get<Record<AuthenticationMethodType, boolean>>('/authentication')
		return new Map(Object.entries(isNewByAuthenticationObject)) as Map<AuthenticationMethodType, boolean>
	}

	static async isAuthenticated() {
		return !!API.token && await API.get<boolean>(`/authentication/check`)
	}

	static async authenticateWithPassword(username: string, password: string) {
		API.token = await API.get<string>(`/authentication/password?username=${username}&password=${password}`)
	}

	static updatePassword(newPassword: string, oldPassword: string) {
		return API.put(`/authentication/password?newPassword=${newPassword}${oldPassword ? `&oldPassword=${oldPassword}` : ''}`)
	}

	static unauthenticate() {
		API.token = undefined
	}
}