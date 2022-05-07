/* eslint-disable */
// @ts-nocheck
const path = require('path')
const MoDeLWebpackConfigFactory = require('@3mo/model/build/WebpackConfig.ts')
const CopyPlugin = require('copy-webpack-plugin')
const FaviconsWebpackPlugin = require('favicons-webpack-plugin')

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
		proxy: {
			'/api': {
				target: 'http://solidarity_server',
				pathRewrite: { '^/api': '' },
				secure: false,
				changeOrigin: true,
			},
		},
		watchOptions: {
			poll: true // enable polling since "fsevents" are not supported in docker
		}
	},
}, [
	new CopyPlugin({
		patterns: [
			{
				from: 'assets',
				to: 'assets',
			},
			{
				from: 'node_modules/@geoman-io/leaflet-geoman-free/dist/leaflet-geoman.css',
				to: 'leaflet-geoman.css',
			},
			{
				from: 'node_modules/leaflet/dist/leaflet.css',
				to: 'leaflet.css',
			},
		]
	}),
	new FaviconsWebpackPlugin({
		logo: './assets/solidarity.svg',
		manifest: './assets/solidarity.webmanifest',
	}),
])