// @ts-check
import { build } from 'esbuild'
import { writeAssets, writeManifest } from './assets.mjs'

const directory = './dist'

await writeAssets(directory)
await writeManifest(directory)

await build({
	bundle: true,
	entryPoints: ['./application/index.ts'],
	splitting: true,
	format: 'esm',
	loader: {
		'.ttf': 'file'
	},
	legalComments: 'none',
	outdir: directory,
	entryNames: '[name]-[hash]',
	chunkNames: '[name]-[hash]',
	minify: true,
})