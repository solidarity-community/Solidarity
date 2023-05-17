import { resolve } from 'path'
import TsconfigPathsPlugin from 'tsconfig-paths-webpack-plugin'
import TerserPlugin from 'terser-webpack-plugin'
import ResolveTypeScriptPlugin from 'resolve-typescript-plugin'
import HtmlWebpackPlugin from 'html-webpack-plugin'
import CopyWebpackPlugin from 'copy-webpack-plugin'
import FaviconsWebpackPlugin from 'favicons-webpack-plugin'

export default (_, args) => {
	const mode = args.mode || 'production'
	return {
		entry: './index.ts',
		context: resolve(__dirname),
		watchOptions: {
			poll: 1000, // Check for changes every second
		},
		output: {
			globalObject: 'self',
			path: resolve('dist'),
			publicPath: '/',
			filename: mode === 'development' ? 'main.js' : 'main.[contenthash].js'
		},
		plugins: [
			new HtmlWebpackPlugin({
				templateContent: `
					<!DOCTYPE html>
					<html lang="en">
						<head>
							<meta charset="UTF-8">
							<meta http-equiv="X-UA-Compatible" content="IE=edge">
							<meta name="viewport" content="width=device-width,initial-scale=1.0,maximum-scale=1.0,user-scalable=no">
							<title>Solidarity</title>
						</head>

						<body>
							<solid-application></solid-application>
						</body>
					</html>
				`
			}),
			new FaviconsWebpackPlugin({
				logo: './assets/solidarity.svg',
				manifest: './assets/solidarity.webmanifest',
				favicons: {
					appleStatusBarStyle: 'default'
				}
			}),
			new CopyWebpackPlugin({
				patterns: [
					{ from: 'assets', to: 'assets' },
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
		],
		module: {
			rules: [
				mode === 'development' ? undefined : {
					test: /\.(js|jsx|ts|tsx)$/,
					loader: 'minify-html-literals-loader',
					exclude: [
						/PageDialog.js/,
						/PageError.js/,
						/DataGrid.js/,
						/HTMLMediaComponent.ts/,
						/PopoverHost.js/,
						/Accent.js/,
						/Snackbar.js/,
					],
				},
				{
					test: /\.ts?$/,
					loader: 'ts-loader',
					options: { allowTsInNodeModules: true },
				},
				{
					test: /\.css$/,
					use: ['style-loader', 'css-loader']
				},
				{
					test: /\.ttf$/,
					use: ['file-loader']
				}
			].filter(Boolean)
		},
		resolve: {
			extensions: ['.ts', '.js'],
			plugins: [
				new ResolveTypeScriptPlugin(),
				new TsconfigPathsPlugin({ configFile: './tsconfig.json' }),
			]
		},
		optimization: mode === 'development' ? undefined : {
			minimize: true,
			minimizer: [
				new TerserPlugin({
					terserOptions: {
						output: {
							comments: false,
						},
					},
					extractComments: false,
				})
			],
		}
	}
}