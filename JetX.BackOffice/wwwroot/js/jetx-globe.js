window.jetxGlobe = {
    wwd: null,
    aircraftLayer: null,
    atmosphereLayer: null,

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

    init: async function (canvasId) {
        try {
            await this.loadWorldWindScript();
        } catch (err) {
            console.error("-> [JetX Globe] Error de librería:", err);
            return false;
        }

        const canvas = document.getElementById(canvasId);
        if (!canvas) return false;

        canvas.width = canvas.parentElement.clientWidth || 1280;
        canvas.height = canvas.parentElement.clientHeight || 720;

        try {
            this.wwd = new WorldWind.WorldWindow(canvasId);

            // 1. Capa Satelital Fotográfica Azul/Verde (Blue Marble de la NASA)
            const blueMarble = new WorldWind.BMNGLayer();
            this.wwd.addLayer(blueMarble);

            // 2. Capa de luces nocturnas de ciudades en la oscuridad (Earth at Night / Black Marble)
            const nightLayer = new WorldWind.RenderableLayer("Luces Nocturnas");
            this.wwd.addLayer(nightLayer);

            // 3. Capa de Atmósfera con Resplandor Exterior Azul y Luz Solar Dinámica
            this.atmosphereLayer = new WorldWind.AtmosphereLayer();
            // Asigna la fecha y hora UTC real para proyectar la sombra del terminador día/noche
            this.atmosphereLayer.time = new Date();
            this.wwd.addLayer(this.atmosphereLayer);

            // 4. Fondo de estrellas del espacio profundo y brújula de orientación
            this.wwd.addLayer(new WorldWind.StarFieldLayer());
            this.wwd.addLayer(new WorldWind.CompassLayer());

            // 5. Capa para la Flota Ejecutiva de JetX y Tráfico ADS-B
            this.aircraftLayer = new WorldWind.RenderableLayer("Flota JetX");
            this.wwd.addLayer(this.aircraftLayer);

            // 6. Configurar la cámara centrada sobre el espacio aéreo de las Américas
            this.wwd.navigator.lookAtLocation.latitude = 19.43;
            this.wwd.navigator.lookAtLocation.longitude = -99.07;
            this.wwd.navigator.range = 8.5e6; // Altitud de 8,500 km para apreciar el contorno de la Tierra

            this.wwd.redraw();
            return true;
        } catch (e) {
            console.error("-> [JetX Globe] Error al inicializar WebGL:", e);
            return false;
        }
    },

    renderJets: function (jets) {
        if (!this.aircraftLayer || !this.wwd) return;

        // Actualizar la hora de la atmósfera cada vez que se refresca el radar para mover la sombra del sol
        if (this.atmosphereLayer) {
            this.atmosphereLayer.time = new Date();
        }

        this.aircraftLayer.removeAllRenderables();

        // Siluetas SVG aerodinámicas orientadas al Norte (0°)
        const planeJetXSvg = "data:image/svg+xml;utf8,<svg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 24 24' width='48' height='48'><path fill='%2300d2ff' stroke='%23ffffff' stroke-width='1.2' d='M21 16v-2l-8-5V3.5c0-.83-.67-1.5-1.5-1.5S10 2.67 10 3.5V9l-8 5v2l8-2.5V19l-2 1.5V22l3.5-1 3.5 1v-1.5L13 19v-5.5l8 2.5z'/></svg>";
        const trafficCommercialSvg = "data:image/svg+xml;utf8,<svg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 24 24' width='32' height='32'><path fill='%23ffb703' stroke='%23000000' stroke-width='0.8' d='M21 16v-2l-8-5V3.5c0-.83-.67-1.5-1.5-1.5S10 2.67 10 3.5V9l-8 5v2l8-2.5V19l-2 1.5V22l3.5-1 3.5 1v-1.5L13 19v-5.5l8 2.5z'/></svg>";

        jets.forEach(jet => {
            const placemarkAttributes = new WorldWind.PlacemarkAttributes(null);
            placemarkAttributes.imageSource = jet.esJetX ? planeJetXSvg : trafficCommercialSvg;
            placemarkAttributes.imageScale = jet.esJetX ? 1.0 : 0.65;
            placemarkAttributes.imageOffset = new WorldWind.Offset(
                WorldWind.OFFSET_MODE_FRACTION, 0.5,
                WorldWind.OFFSET_MODE_FRACTION, 0.5
            );

            // Rotación según el rumbo real ADS-B (0° a 360°)
            placemarkAttributes.imageRotation = jet.rumbo || 0;

            const pos = new WorldWind.Position(jet.latitud, jet.longitud, (jet.altitudPies || 0) * 0.3048);
            const placemark = new WorldWind.Placemark(pos, false, placemarkAttributes);

            if (jet.esJetX) {
                placemark.label = `${jet.matricula} (${jet.modelo})\nFL${Math.round(jet.altitudPies / 100)} | ${jet.velocidad} kts`;
            } else {
                placemark.label = `${jet.matricula}\n${jet.altitudPies} ft`;
            }

            placemark.altitudeMode = WorldWind.RELATIVE_TO_GROUND;
            this.aircraftLayer.addRenderable(placemark);
        });

        this.wwd.redraw();
    }
};