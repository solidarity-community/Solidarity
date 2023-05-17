import { Api } from 'sdk'

export class FileService {
	static getPath(uri: string) {
		return `${Api.url}/file/${uri}`
	}

	static getAllowedExtensions() {
		return Api.get<Array<string>>('/file/allowed-extensions')
	}

	static getMaximumSizeInMB() {
		return Api.get<number>('/file/max-size')
	}

	static save(file: File) {
		return Api.post<string>('/file', file)
	}
}