// @ts-check
import { context } from 'esbuild'
import { writeAssets } from './assets.mjs'

const directory = './dist'

await writeAssets(directory)

const ctx = await context({
	bundle: true,
	entryPoints: ['./application/index.ts'],
	splitting: true,
	format: 'esm',
	loader: {
		'.ttf': 'file'
	},
	legalComments: 'none',
	sourcemap: 'inline',
	outdir: directory,
})

await ctx.watch()