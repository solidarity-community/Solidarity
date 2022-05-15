import { HttpError } from 'sdk'

export type HTTPMethod = 'GET' | 'POST' | 'PUT' | 'PATCH' | 'DELETE' | 'OPTIONS' | 'HEAD'

export type ApiValueConstructor<TConstructed, TDeconstructed> = {
	shallConstruct(text: unknown): boolean
	construct(text: TDeconstructed): TConstructed

	shallDeconstruct?(value: unknown): boolean
	deconstruct?(value: TConstructed): TDeconstructed
}

export const apiValueConstructor = () => {
	return (Constructor: Constructor<ApiValueConstructor<unknown, unknown>>) => {
		API.valueConstructors.add(new Constructor)
	}
}

export class API {
	static readonly valueConstructors = new Set<ApiValueConstructor<unknown, unknown>>()
	static readonly url = '/api'
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
		return this.fetch<T>('POST', route, JSON.stringify(API.deconstruct(data)))
	}

	static postFile<T = void>(route: string, file: File) {
		const form = new FormData()
		form.set('file', file, file.name)
		return this.fetch<T>('POST', route, form)
	}

	static put<T = void, TData = unknown>(route: string, data?: TData) {
		return this.fetch<T>('PUT', route, JSON.stringify(API.deconstruct(data)))
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
			throw new HttpError(await response.json())
		}

		try {
			return API.construct<T>(await response.json())
		} catch (error) {
			return undefined!
		}
	}

	private static construct<T>(response: any): T {
		return typeof response !== 'object' ? response : response instanceof Array
			? response.map(item => API.construct(item)) as unknown as T
			: Object.fromEntries(
				Object.entries(response).map(([key, value]) => [
					key,
					API.isPrimitiveObject(value)
						? API.construct(value)
						: [...API.valueConstructors].find(converter => converter.shallConstruct(value))?.construct(value) ?? value
				])
			) as unknown as T
	}

	private static deconstruct<T>(data: T): any {
		return typeof data !== 'object' ? data : data instanceof Array
			? data.map(item => API.deconstruct(item))
			: Object.fromEntries(
				Object.entries(data).map(([key, value]) => [
					key,
					[...API.valueConstructors].find(converter => converter.shallDeconstruct?.(value) ?? false)?.deconstruct?.(value) ?? value
				])
			)
	}

	private static isPrimitiveObject(value: unknown): value is object {
		return typeof value === 'object'
			&& value !== null
			&& Object.getPrototypeOf(value) === Object.prototype
	}
}