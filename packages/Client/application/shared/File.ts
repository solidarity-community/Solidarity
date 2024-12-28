import { Api } from 'application'

export class File {
	static getPath(uri: string) {
		return `${Api.url}/file/${uri}`
	}

	static getAllowedExtensions() {
		return Api.get<Array<string>>('/file/allowed-extensions')
	}

	static getMaximumSizeInMB() {
		return Api.get<number>('/file/max-size')
	}

	static save(file: File, route?: string, existingId?: Guid) {
		return Api.post<string>('/file', file)
	}
}