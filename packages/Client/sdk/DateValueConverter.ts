import { apiValueConverter, ApiValueConverter } from './API'

@apiValueConverter()
export class DateValueConverter implements ApiValueConverter<Date, string> {
	private static readonly regex = /(\d{4}-[01]\d-[0-3]\dT[0-2]\d:[0-5]\d:[0-5]\d\.\d+([+-][0-2]\d:[0-5]\d|Z))|(\d{4}-[01]\d-[0-3]\dT[0-2]\d:[0-5]\d:[0-5]\d([+-][0-2]\d:[0-5]\d|Z))|(\d{4}-[01]\d-[0-3]\dT[0-2]\d:[0-5]\d([+-][0-2]\d:[0-5]\d|Z))/

	shallConvertFrom = (value: unknown) => value instanceof Date
	convertFrom = (value: Date) => value.toISOString()

	shallConvertTo = (value: unknown) => typeof value === 'string' && DateValueConverter.regex.test(value)
	convertTo = (text: string) => new Date(text)
}