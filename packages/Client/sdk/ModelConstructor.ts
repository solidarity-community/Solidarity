import { apiValueConstructor, ApiValueConstructor } from './API'

export const model = (csharpTypeName: string) => {
	return (Constructor: Constructor<unknown>) => {
		ModelConstructor.modelConstructorsByCsharpTypeName.set(csharpTypeName, Constructor)
	}
}

@apiValueConstructor()
export class ModelConstructor implements ApiValueConstructor<unknown, unknown> {
	static readonly modelConstructorsByCsharpTypeName = new Map<string, Constructor<unknown>>()
	private static readonly csharpTypeNameKeyName = '__typeName__'

	shallConstruct(value: unknown): boolean {
		const isObject = typeof value === 'object' && value !== null
		const isArray = Array.isArray(value)
		return !isObject && !isArray
			? false
			: isArray
				? value.some(v => this.shallConstruct(v))
				: ModelConstructor.csharpTypeNameKeyName in value
	}

	construct(value: unknown): unknown {
		const isObject = typeof value === 'object' && value !== null
		const isArray = Array.isArray(value)
		return !isObject && !isArray
			? value
			: isArray
				? value.map(v => this.construct(v))
				: this.constructObject(value)
	}

	private constructObject(object: object) {
		const csharpTypeNameKey = ModelConstructor.csharpTypeNameKeyName as keyof typeof object
		const csharpTypeName = object[csharpTypeNameKey] as string
		const Constructor = ModelConstructor.modelConstructorsByCsharpTypeName.get(csharpTypeName)
		return !Constructor ? object : Object.assign(new Constructor, object)
	}
}