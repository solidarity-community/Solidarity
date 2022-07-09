import { apiAuthenticator, ApiAuthenticator } from './API'

@apiAuthenticator()
export class JwtApiAuthenticator implements ApiAuthenticator {
	private static readonly tokenStorageKey = 'Solidarity.Authentication.Token'

	static get token() { return localStorage.getItem(JwtApiAuthenticator.tokenStorageKey) ?? undefined }
	static set token(value) {
		if (value) {
			localStorage.setItem(JwtApiAuthenticator.tokenStorageKey, value)
		} else {
			localStorage.removeItem(JwtApiAuthenticator.tokenStorageKey)
		}
	}

	authenticate(data: string) {
		JwtApiAuthenticator.token = data
	}

	unauthenticate() {
		JwtApiAuthenticator.token = undefined
	}

	isAuthenticated() {
		return !!JwtApiAuthenticator.token
	}

	processRequest(request: Request): Request {
		const token = JwtApiAuthenticator.token
		if (token) {
			const headers = request.headers as Headers
			headers.set('Authorization', `Bearer ${token}`)
		}
		return request
	}
} 