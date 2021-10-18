/* eslint-disable */
// @ts-nocheck
const path = require('path')
const MoDeLWebpackConfigFactory = require('@3mo/model/build/WebpackConfig.ts')
const CopyPlugin = require('copy-webpack-plugin')
// const FaviconsWebpackPlugin = require('favicons-webpack-plugin')

module.exports = (_, arguments) => MoDeLWebpackConfigFactory(arguments.mode, {
	cache: false,
	entry: './index.ts',
	context: path.resolve(__dirname),
	output: {
		filename: 'main.js',
		path: path.resolve(__dirname, 'dist'),
		publicPath: '/'
	},
	devServer: {
		host: '0.0.0.0',
		port: 8080,
		contentBase: path.join(__dirname, 'dist'),
		historyApiFallback: true,
		watchOptions: {
			poll: true // enable polling since "fsevents" are not supported in docker
		}
	},
}, [
	// new FaviconsWebpackPlugin({
	// 	logo: 'assets/solidarity.svg',
	// 	logo: false,
	// 	manifest: './assets/solidarity.webmanifest'
	// }),
	new CopyPlugin({
		patterns: [
			{
				from: 'assets',
				to: 'assets',
			}
		]
	})
])