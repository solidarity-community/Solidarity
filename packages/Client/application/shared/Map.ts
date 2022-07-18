import { Component, component, css, html, property, query, event, ThemeHelper, Background } from '@3mo/modelx'
import { Map as LMap, TileLayer, Circle, PM, Layer, geoJSON, FeatureGroup } from 'leaflet'
import { GeometryCollection, Feature } from 'geojson'
import { ResizeController } from './utilities'

/** @fires selectedAreaChange CustomEvent<Geography | undefined> */
@component('solid-map')
export class Map extends Component {
	private static readonly tileLayerUrlDark = 'https://{s}.basemaps.cartocdn.com/dark_all/{z}/{x}/{y}{r}.png'
	private static readonly tileLayerUrlLight = 'https://{s}.basemaps.cartocdn.com/light_all/{z}/{x}/{y}{r}.png'
	private static readonly polygonSidesForCircleConversion = 100

	@event() readonly selectedAreaChange!: EventDispatcher<GeometryCollection | undefined>

	@property({
		type: Boolean,
		updated(this: Map) {
			if (this.readOnly) {
				this.map.pm.removeControls()
			} else {
				this.map.pm.addControls({
					position: 'topleft',
					drawMarker: false,
					drawCircleMarker: false,
				})
			}
		}
	}) readOnly = false

	@property({
		type: Object,
		updated(this: Map) {
			this.layers.forEach(layer => layer.remove())

			if (!this.selectedArea) {
				return
			}

			this.selectedArea.geometries.forEach(geo => {
				const layer = geoJSON(geo)
				layer.setStyle({ color: 'var(--mo-accent)' })
				this.map.addLayer(layer)
			})

			Promise.delegateToEventLoop(() => {
				const bounds = new FeatureGroup(this.layers).getBounds()
				this.map.fitBounds(bounds)
			})
		}
	}) selectedArea?: GeometryCollection

	@query('div#map') readonly mapElement!: HTMLDivElement

	private map!: LMap

	protected readonly resizeController = new ResizeController(this, () => this.map.invalidateSize())

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
		ThemeHelper.background.changed.subscribe(() => this.initializeMap())
		this.initializeMap()
	}

	protected initializeMap() {
		// @ts-expect-error
		const styleLayers = () => this.layers.forEach(layer => layer.setStyle({ color: 'var(--mo-accent)' }))

		const dispatchChange = () => {
			const geometries = this.layers.map(layer => {
				// @ts-expect-error
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

		const url = ThemeHelper.background.calculatedValue === Background.Dark ? Map.tileLayerUrlDark : Map.tileLayerUrlLight

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

	private get layers() {
		return this.map.pm.getGeomanLayers() as Array<Layer>
	}
}

declare global {
	interface HTMLElementTagNameMap {
		'solid-map': Map
	}
}