
import favicons from 'favicons'
import { writeFileSync, readFileSync, mkdirSync, cpSync, readdirSync, rmSync } from 'fs'

export async function writeAssets(directory) {
	mkdirSync(directory, { recursive: true })
	readdirSync(directory).forEach(f => rmSync(`${directory}/${f}`, { recursive: true }))
	cpSync('./assets', `${directory}/assets`, { recursive: true })
	cpSync('./node_modules/@geoman-io/leaflet-geoman-free/dist/leaflet-geoman.css', `${directory}/leaflet-geoman.css`)
	cpSync('./node_modules/leaflet/dist/leaflet.css', `${directory}/leaflet.css`)
	await {}
}

export async function writeManifest(directory) {
	const manifest = JSON.parse(readFileSync('./assets/solidarity.webmanifest', 'utf8'))
	const response = await favicons('./assets/solidarity.svg', {
		version: manifest.version,
		appName: manifest.name,
		appShortName: manifest.short_name,
		appDescription: manifest.description,
		start_url: manifest.start_url,
		display: manifest.display,
		theme_color: manifest.theme_color,
		background: manifest.background_color,
		developerName: manifest.developer.name,
		developerURL: manifest.developer.url,
		appleStatusBarStyle: 'default'
	});
	[...response.files, ...response.images].forEach(({ name, contents }) => {
		if (name === 'manifest.json') {
			const contentJson = JSON.parse(contents)
			contentJson.display_override = manifest.display_override
			contentJson.shortcuts = manifest.shortcuts
			contents = JSON.stringify(contentJson, null, '\t')
		}
		writeFileSync(`${directory}/${name}`, contents)
	})
	writeFileSync(`${directory}/headers.html`, response.html.join('\n'))
}