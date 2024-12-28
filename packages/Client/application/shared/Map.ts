import { Component, component, css, html, property, query, event } from '@a11d/lit'
import { Theme, Background } from '@3mo/theme'
import { ResizeController } from '@3mo/resize-observer'
import { Map as LMap, TileLayer, type Circle, PM, type Layer, geoJSON, FeatureGroup, type LatLngBoundsExpression } from 'leaflet'
import { type GeometryCollection, type Feature } from 'geojson'

/** @fires selectedAreaChange CustomEvent<Geography | undefined> */
@component('solid-map')
export class Map extends Component {
	private static readonly tileLayerUrlDark = 'https://{s}.basemaps.cartocdn.com/dark_all/{z}/{x}/{y}{r}.png'
	private static readonly tileLayerUrlLight = 'https://{s}.basemaps.cartocdn.com/light_all/{z}/{x}/{y}{r}.png'
	private static readonly polygonSidesForCircleConversion = 100

	@event() readonly selectedAreaChange!: EventDispatcher<GeometryCollection | undefined>

	@property({
		type: Boolean,
		async updated(this: Map) {
			const map = await this.mapUpdated
			if (this.readOnly) {
				map.pm.removeControls()
			} else {
				map.pm.addControls({
					position: 'topleft',
					drawMarker: false,
					drawCircleMarker: false,
				})
			}
		}
	}) readOnly = false

	@property({
		type: Object,
		bindingDefault: true,
		event: 'selectedAreaChange',
		updated(this: Map) {
			this.layers = this.selectedArea?.geometries.map(geo => geoJSON(geo)) ?? []
		}
	}) selectedArea?: GeometryCollection

	private get layers() { return this.map?.pm.getGeomanLayers() as Array<Layer> ?? [] }
	private set layers(layers: Array<Layer>) {
		this.mapUpdated.then(map => {
			this.layers.forEach(layer => map.removeLayer(layer))
			for (const layer of layers) {
				(layer as any).setStyle({ color: 'var(--mo-color-accent)' })
				map.addLayer(layer)
			}
			new Promise(r => setTimeout(() => this.fitBounds().then(r), 100))
		})
	}

	@query('div#map') readonly mapElement!: HTMLDivElement

	private map?: LMap

	private get mapUpdated() {
		return this.updateComplete.then(() => this.map!)
	}

	async fitBounds(bounds?: LatLngBoundsExpression) {
		const map = await this.mapUpdated
		bounds ??= !this.layers ? undefined : new FeatureGroup(this.layers).getBounds()
		if (bounds) {
			map.fitBounds(bounds)
		}
	}

	protected readonly resizeController = new ResizeController(this, { callback: () => this.map?.invalidateSize() })

	static override get styles() {
		return css`
			:host {
				position: relative;
				display: flex;
				min-height: 400px;
			}

			#map {
				flex: 1;
				z-index: 0;
			}
		`
	}

	protected override get template() {
		return html`
			<link rel='stylesheet' href='/leaflet.css'>
			<link rel='stylesheet' href='/leaflet-geoman.css'>
			<div id='map'></div>
		`
	}

	protected override initialized() {
		Theme.background.changed.subscribe(() => this.initializeMap())
		this.initializeMap()
	}

	protected initializeMap() {
		// @ts-expect-error - The leaflet-geoman plugin is not typed
		const styleLayers = () => this.layers.forEach(layer => layer.setStyle({ color: 'var(--mo-color-accent)' }))

		const dispatchChange = () => {
			const geometries = this.layers.map(layer => {
				// @ts-expect-error - The layer has a _shape property
				const shape = layer.pm._shape as string | undefined

				if (!shape || ('toGeoJSON' in layer) === false) {
					return undefined
				}

				switch (shape) {
					case 'Circle':
						return PM.Utils.circleToPolygon(layer as Circle, Map.polygonSidesForCircleConversion).toGeoJSON()
					default:
						// @ts-expect-error - The layer has a toGeoJSON method
						return layer.toGeoJSON()
				}
			}).filter((geo): geo is Feature => geo !== undefined).map(feature => feature.geometry)

			const geometryCollection: GeometryCollection | undefined = geometries.length === 0 ? undefined : {
				type: 'GeometryCollection',
				geometries: geometries
			}

			this.selectedAreaChange.dispatch(geometryCollection)
		}

		const url = Theme.background.calculatedValue === Background.Dark ? Map.tileLayerUrlDark : Map.tileLayerUrlLight

		this.map = new LMap(this.mapElement)
			.setView([0, 0], 2)
			.addLayer(new TileLayer(url, { minZoom: 2 }))

		this.map
			.on('pm:create', dispatchChange).on('pm:create', styleLayers)
			.on('pm:edit', dispatchChange)
			.on('pm:rotate', dispatchChange)
			.on('pm:resize', dispatchChange)
			.on('pm:remove', dispatchChange)
			.on('pm:drag', dispatchChange)
	}
}

declare global {
	interface HTMLElementTagNameMap {
		'solid-map': Map
	}
}