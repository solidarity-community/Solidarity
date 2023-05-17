export default {
	extends: [
		"./node_modules/@a11d/lit/.eslintrc.json"
	],
	parserOptions: {
		"tsconfigRootDir": __dirname
	},
	settings: {
		"import/resolver": {
			node: {
				extensions: [".js", ".ts"],
				paths: ["."]
			}
		}
	}
}