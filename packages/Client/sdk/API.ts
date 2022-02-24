import { HttpError } from 'sdk'

export type HTTPMethod = 'GET' | 'POST' | 'PUT' | 'PATCH' | 'DELETE' | 'OPTIONS' | 'HEAD'

export class API {
	private static readonly url = '/api'
	private static readonly tokenStorageKey = 'Solidarity.Authentication.Token'

	static get token() { return localStorage.getItem(API.tokenStorageKey) ?? undefined }
	static set token(value) {
		if (value) {
			localStorage.setItem(API.tokenStorageKey, value)
		} else {
			localStorage.removeItem(API.tokenStorageKey)
		}
	}

	static get<T = void>(route: string) {
		return this.fetch<T>('GET', route)
	}

	static post<T = void, TData = unknown>(route: string, data?: TData) {
		return this.fetch<T>('POST', route, JSON.stringify(data))
	}

	static postFile<T = void>(route: string, fileList: FileList) {
		const form = new FormData()
		form.set('formFile', fileList[0], fileList[0].name)
		return this.fetch<T>('POST', route, form)
	}

	static put<T = void, TData = unknown>(route: string, data?: TData) {
		return this.fetch<T>('PUT', route, JSON.stringify(data))
	}

	static delete<T = void>(route: string) {
		return this.fetch<T>('DELETE', route)
	}

	private static async fetch<T = void>(method: HTTPMethod, route: string, body: BodyInit | null = null) {
		const headers: HeadersInit = {
			Accept: 'application/json',
			Authorization: `Bearer ${this.token}`
		}

		const isForm = body instanceof FormData
		if (isForm === false) {
			headers['Content-Type'] = 'application/json'
		}

		const response = await fetch(API.url + route, {
			method: method,
			credentials: 'omit',
			headers: headers,
			referrer: 'no-referrer',
			body: body
		})

		if (response.status >= 400) {
			throw new HttpError(response.status)
		}

		try {
			return await response.json() as T
		} catch (error) {
			return undefined!
		}
	}
}