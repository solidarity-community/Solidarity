import { API } from 'sdk'

export class FileService {
	static getPath(uri: string) {
		return `${API.url}/file/${uri}`
	}

	static getAllowedExtensions() {
		return API.get<Array<string>>('/file/allowed-extensions')
	}

	static getMaximumSizeInMB() {
		return API.get<number>('/file/max-size')
	}

	static save(file: File) {
		return API.postFile<string>('/file', file)
	}
}