window.jetxGlobe = {
    wwd: null,
    aircraftLayer: null,
    airwaysLayer: null,
    atmosphereLayer: null,
    dotNetHelper: null,

    loadWorldWindScript: function () {
        return new Promise((resolve, reject) => {
            if (typeof WorldWind !== "undefined") {
                resolve();
                return;
            }
            const script = document.createElement("script");
            script.src = "https://unpkg.com/worldwindjs@1.7.0/build/dist/worldwind.min.js";
            script.async = true;
            script.onload = () => resolve();
            script.onerror = () => reject(new Error("Error al descargar WorldWindJS"));
            document.head.appendChild(script);
        });
    },

    init: async function (canvasId, dotNetRef) {
        this.dotNetHelper = dotNetRef;

        try {
            await this.loadWorldWindScript();
        } catch (err) {
            console.error("-> [JetX Globe] Error de librería:", err);
            return false;
        }

        const canvas = document.getElementById(canvasId);
        if (!canvas) return false;

        // Densidad nativa de pantalla (evita pixelado en pantallas Retina / 1080p+)
        const dpr = window.devicePixelRatio || 1;
        canvas.width = (canvas.parentElement.clientWidth || 1280) * dpr;
        canvas.height = (canvas.parentElement.clientHeight || 720) * dpr;

        try {
            this.wwd = new WorldWind.WorldWindow(canvasId);

            // 1. Capa base fotográfica Blue Marble (Tierra con continentes y océanos)
            const blueMarble = new WorldWind.BMNGOneImageLayer();
            blueMarble.minActiveAltitude = 0;
            this.wwd.addLayer(blueMarble);

            // 2. Fondo del espacio profundo con estrellas
            const stars = new WorldWind.StarFieldLayer();
            stars.time = new Date();
            this.wwd.addLayer(stars);

            // 3. Atmósfera fotométrica con halo azul y ciclo solar Día/Noche
            this.atmosphereLayer = new WorldWind.AtmosphereLayer();
            this.atmosphereLayer.time = new Date(); // Sombra solar exacta en tiempo real
            this.wwd.addLayer(this.atmosphereLayer);

            // 4. Brújula orbital táctica
            this.wwd.addLayer(new WorldWind.CompassLayer());

            // 5. Capa para el Grafo del Espacio Aéreo (Aerovías de Supabase)
            this.airwaysLayer = new WorldWind.RenderableLayer("Red de Aerovias JetX");
            this.wwd.addLayer(this.airwaysLayer);

            // 6. Capa para Aeronaves ADS-B
            this.aircraftLayer = new WorldWind.RenderableLayer("Flota JetX");
            this.wwd.addLayer(this.aircraftLayer);

            // 7. Encuadre inicial de cámara en el espacio aéreo de Norte/Centroamérica
            this.wwd.navigator.lookAtLocation.latitude = 18.0;
            this.wwd.navigator.lookAtLocation.longitude = -92.0;
            this.wwd.navigator.range = 9.5e6; // Altitud adecuada para ver la curvatura esférica
            this.wwd.redraw();

            // Detector táctil/clic para seleccionar aviones en el globo
            const self = this;
            const handleClick = function (recognizer) {
                const x = recognizer.clientX;
                const y = recognizer.clientY;
                const pickList = self.wwd.pick(self.wwd.canvasCoordinates(x, y));

                if (pickList.objects.length > 0) {
                    for (let i = 0; i < pickList.objects.length; i++) {
                        const pickedObj = pickList.objects[i].userObject;
                        if (pickedObj instanceof WorldWind.Placemark && pickedObj.userProperties && pickedObj.userProperties.jetData) {
                            if (self.dotNetHelper) {
                                self.dotNetHelper.invokeMethodAsync("SeleccionarAvionDesdeCanvas", pickedObj.userProperties.jetData);
                            }
                            break;
                        }
                    }
                }
            };
            new WorldWind.ClickRecognizer(this.wwd, handleClick);

            return true;
        } catch (e) {
            console.error("-> [JetX Globe] Error WebGL:", e);
            return false;
        }
    },

    // Renderiza la red de aerovías conectada al grafo PostGIS de Supabase
    renderAirways: function (airways) {
        if (!this.airwaysLayer || !this.wwd) return;
        this.airwaysLayer.removeAllRenderables();

        const airwayAttr = new WorldWind.ShapeAttributes(null);
        airwayAttr.outlineColor = new WorldWind.Color(0, 0.85, 1, 0.55); // Cian brillante luminiscente
        airwayAttr.outlineWidth = 2.2;
        airwayAttr.depthTest = false; // Garantiza visibilidad en penumbra nocturna

        airways.forEach(a => {
            const positions = [
                new WorldWind.Position(a.origenLat, a.origenLon, (a.altitudMin || 24000) * 0.3048),
                new WorldWind.Position(a.destLat, a.destLon, (a.altitudMax || 45000) * 0.3048)
            ];
            const path = new WorldWind.Path(positions, airwayAttr);
            path.altitudeMode = WorldWind.RELATIVE_TO_GROUND;
            path.followTerrain = false;
            this.airwaysLayer.addRenderable(path);
        });

        this.wwd.redraw();
    },

    // Dibuja los jets orientados según su rumbo de vuelo
    renderJets: function (jets) {
        if (!this.aircraftLayer || !this.wwd) return;

        if (this.atmosphereLayer) {
            this.atmosphereLayer.time = new Date();
        }

        this.aircraftLayer.removeAllRenderables();

        const planeJetXSvg = "data:image/svg+xml;utf8,<svg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 24 24' width='48' height='48'><path fill='%2300f2fe' stroke='%23ffffff' stroke-width='1.5' d='M21 16v-2l-8-5V3.5c0-.83-.67-1.5-1.5-1.5S10 2.67 10 3.5V9l-8 5v2l8-2.5V19l-2 1.5V22l3.5-1 3.5 1v-1.5L13 19v-5.5l8 2.5z'/></svg>";
        const trafficCommercialSvg = "data:image/svg+xml;utf8,<svg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 24 24' width='28' height='28'><path fill='%23ffb703' stroke='%23111' stroke-width='0.8' d='M21 16v-2l-8-5V3.5c0-.83-.67-1.5-1.5-1.5S10 2.67 10 3.5V9l-8 5v2l8-2.5V19l-2 1.5V22l3.5-1 3.5 1v-1.5L13 19v-5.5l8 2.5z'/></svg>";

        jets.forEach(jet => {
            const placemarkAttributes = new WorldWind.PlacemarkAttributes(null);
            placemarkAttributes.imageSource = jet.esJetX ? planeJetXSvg : trafficCommercialSvg;
            placemarkAttributes.imageScale = jet.esJetX ? 1.1 : 0.65;
            placemarkAttributes.imageOffset = new WorldWind.Offset(
                WorldWind.OFFSET_MODE_FRACTION, 0.5,
                WorldWind.OFFSET_MODE_FRACTION, 0.5
            );

            // Orientación según heading magnético (0° a 360°)
            placemarkAttributes.imageRotation = jet.rumbo || 0;
            placemarkAttributes.depthTest = false;

            const pos = new WorldWind.Position(jet.latitud, jet.longitud, (jet.altitudPies || 0) * 0.3048);
            const placemark = new WorldWind.Placemark(pos, false, placemarkAttributes);

            if (jet.esJetX) {
                placemark.label = `${jet.matricula} (${jet.modelo})\nFL${Math.round(jet.altitudPies / 100)} | ${jet.velocidad} kts`;
            }

            placemark.altitudeMode = WorldWind.RELATIVE_TO_GROUND;
            placemark.userProperties = { jetData: jet };

            this.aircraftLayer.addRenderable(placemark);
        });

        this.wwd.redraw();
    }
};