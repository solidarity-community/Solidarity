import { ApplicationLogo } from '@3mo/model'

const logo = `
	<svg id="Layer_1" data-name="Layer 1" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 1999.06 1999.06">
		<defs>
			<style>.cls-1{fill:none;stroke:{{color}};stroke-miterlimit:10;stroke-width:100px;}</style>
		</defs>
		<path class="cls-1" d="M848.4,1757.09c251,0,454.51-203.5,454.51-454.52" />
		<path class="cls-1" d="M848.4,848.91c251,0,454.51,203.49,454.51,454.51" />
		<path class="cls-1" d="M393.88,1302.57c251,0,454.52,203.5,454.52,454.52C597.37,1757.09,393.88,1553.59,393.88,1302.57Z" />
		<path class="cls-1" d="M996.53,288.93c-251,0-454.51,203.49-454.51,454.51" />
		<path class="cls-1" d="M996.53,1197.1c-251,0-454.51-203.49-454.51-454.51" />
		<path class="cls-1" d="M1451,743.44c-251,0-454.51-203.49-454.51-454.51C1247.55,288.93,1451,492.42,1451,743.44Z" />
		<path class="cls-1" d="M1605.18,495.6c-225.15,111-497.65,18.47-608.65-206.67C1221.68,177.93,1494.18,270.46,1605.18,495.6Z" />
	</svg>
`

ApplicationLogo.source = `data:image/svg+xml;utf8,${logo.replaceAll('\r\n', '')}`