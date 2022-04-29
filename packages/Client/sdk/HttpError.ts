import { HttpErrorCode } from '@3mo/model'

export type AspNetError = {
	readonly title: string,
	readonly status: HttpErrorCode
	readonly errors?: Array<Record<string, Array<string>>>
	readonly traceId: string
	readonly type: string
}

export class HttpError extends Error {
	get status() { return this.error.status }
	get traceId() { return this.error.traceId }
	get type() { return this.error.type }

	constructor(protected readonly error: AspNetError) {
		super(
			[error.title,
			!error.errors ? undefined : Object.values(error.errors)
				.map(error => error[0])
				.join('\n')
			].filter(Boolean).join('\n')
		)
	}
}