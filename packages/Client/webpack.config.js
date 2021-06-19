/* eslint-disable */
// @ts-nocheck
const path = require('path')
const MoDeLWebpackConfigFactory = require('@3mo/model/build/WebpackConfig.ts')

module.exports = (_, arguments) => MoDeLWebpackConfigFactory(arguments.mode, {
	cache: false,
	entry: './application/index.ts',
	context: path.resolve(__dirname),
	output: {
		filename: 'main.js',
		path: path.resolve(__dirname, 'dist'),
		publicPath: '/'
	},
	devServer: {
		contentBase: path.join(__dirname, 'dist'),
		historyApiFallback: true,
		clientLogLevel: 'silent',
	}
})