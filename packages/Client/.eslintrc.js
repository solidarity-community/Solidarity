export default {
	extends: [
		"./node_modules/@3mo/model/.eslintrc.json"
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