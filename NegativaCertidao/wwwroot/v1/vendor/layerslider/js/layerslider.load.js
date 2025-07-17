$(document).ready(function () {
	"use strict";

	// Calling LayerSlider 
		
        $('#slider').layerSlider({
            responsive: true,
			fitScreenWidth:false,
            responsiveUnder: 1280,
            layersContainer: 1280,
            skin: 'outline',
			thumbnailNavigation: 'disabled',
            hoverPrevNext: true,
            skinsPath: './v1/vendor/layerslider/skins/',
            autoStart: true,
			autoPlayVideos : false
        });

		
 });