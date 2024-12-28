import configs from '@a11d/eslint-config/eslint.config.mjs'

export default [...configs, {
	ignores: ['dist', 'test-temp'],
	rules: {
		'no-duplicate-imports': 'error',
		'@typescript-eslint/consistent-type-imports': ['error',
			{
				prefer: 'type-imports',
				fixStyle: 'inline-type-imports',
				disallowTypeAnnotations: false
			}
		]
	}
}]