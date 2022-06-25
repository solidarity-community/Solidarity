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

type FetchOptions = {
	readonly noHttpErrorOnErrorStatusCode?: boolean
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

	static get<T = void>(route: string, options?: FetchOptions) {
		return this.fetch<T>('GET', route, null, options)
	}

	static post<T = void, TData = unknown>(route: string, data?: TData, options?: FetchOptions) {
		return this.fetch<T>('POST', route, JSON.stringify(API.deconstruct(data)), options)
	}

	static postFile<T = void>(route: string, file: File, options?: FetchOptions) {
		const form = new FormData
		form.set('file', file, file.name)
		return this.fetch<T>('POST', route, form, options)
	}

	static put<T = void, TData = unknown>(route: string, data?: TData, options?: FetchOptions) {
		return this.fetch<T>('PUT', route, JSON.stringify(API.deconstruct(data)), options)
	}

	static delete<T = void>(route: string, options?: FetchOptions) {
		return this.fetch<T>('DELETE', route, null, options)
	}

	private static async fetch<T = void>(method: HTTPMethod, route: string, body: BodyInit | null = null, options?: FetchOptions) {
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

		if (response.status >= 400 && !options?.noHttpErrorOnErrorStatusCode) {
			throw new HttpError(await response.json())
		}

		try {
			return API.construct<T>(await response.json())
		} catch (error) {
			return undefined!
		}
	}

	private static construct<T>(data: any, isChild = false): T {
		data = isChild ? data : { ROOT: data }
		const response = !data || typeof data !== 'object' ? data : Object.assign(
			data,
			Object.fromEntries(
				Object.entries(data).map(([key, value]) => [
					key,
					API.construct([...API.valueConstructors].find(converter => converter.shallConstruct(value))?.construct(value) ?? value, true)
				])
			)
		)
		return isChild ? response as T : response.ROOT
	}

	private static deconstruct<T>(data: T, isChild = false): any {
		data = (isChild ? data : { ROOT: data }) as T
		const response = !data || typeof data !== 'object' ? data : Object.assign(
			data,
			Object.fromEntries(
				Object.entries(data).map(([key, value]) => [
					key,
					API.deconstruct([...API.valueConstructors].find(converter => converter.shallDeconstruct?.(value) ?? false)?.deconstruct?.(value) ?? value, true)
				])
			)
		) as any
		return isChild ? response as T : response.ROOT
	}
}