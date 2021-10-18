import { HttpErrorCode } from '@3mo/model'

export class HttpError extends Error {
	constructor(public readonly errorCode: HttpErrorCode) {
		super()
	}
}