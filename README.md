# JetX — Ecosistema Distribuido de Reserva de Jets Privados en Tiempo Real

**JetX** es una plataforma tecnológica empresarial inspirada en el modelo de movilidad bajo demanda de Uber, pero adaptada al sector de la aviación privada ejecutiva. El sistema está diseñado mediante una arquitectura distribuida que conecta un núcleo de servicios en la nube con bases de datos geoespaciales, un panel de administración inteligente y un cliente móvil nativo de alto rendimiento.

---

## Arquitectura General del Ecosistema

El ecosistema de JetX se divide en tres componentes de software completamente desacoplados que se comunican mediante protocolos estándar y sincronización en tiempo real:

1. **JetX Backend Central (`.NET 10 Web API`):** El cerebro del sistema. Aloja los algoritmos matemáticos complejos, el procesamiento de grafos del espacio aéreo, el control financiero y las integraciones de inteligencia artificial corporativa.
2. **JetX Back-Office (`Blazor Web App`):** El panel de control administrativo e industrial para operadores de vuelos. Permite la gestión de la flota, asignación de tripulaciones y visualización de telemetría aérea.
3. **JetX Cliente Móvil (`Android Native / Kotlin`):** La interfaz ligera del pasajero, desarrollada en Jetpack Compose, con visor de cabinas 3D y mapas geodésicos interactivos.

---

## Arquitectura de Software del Backend (Clean Architecture)

El proyecto de C# está construido bajo los principios de **Clean Architecture (Arquitectura Hexagonal)**, garantizando una separación estricta de responsabilidades e independencia absoluta de frameworks, bases de datos o APIs externas:

*   **`JetX.Domain` (Núcleo Puro):** Contiene los modelos de dominio (`Jet`, `Waypoint`, `Vuelo`, `Pago`) y las reglas de negocio base. No tiene dependencias de ninguna librería externa.
*   **`JetX.Application` (Casos de Uso):** Contiene la lógica de la aplicación y define las interfaces (*interfaces/ports*). Aquí residen los casos de uso principales como `CalcularRutaAereaUseCase` y `BuscarPiernasVaciasUseCase`.
*   **`JetX.Infrastructure` (Adaptadores de Datos):** Implementa las interfaces de la capa de aplicación. Contiene las conexiones físicas a **Supabase (PostgreSQL/PostGIS)**, la integración del SDK de **Stripe** y la configuración de **Gemini AI**.
*   **`JetX.WebApi` (Capa de Presentación REST):** Expone los endpoints HTTP JSON que consume la aplicación móvil Android de forma desacoplada.

---

##  Características Principales del Ecosistema

### Backend y Motor Core (.NET 10 & PostgreSQL/PostGIS)
*   **Grafo del Espacio Aéreo Real:** Base de datos estructurada con la red de aerovías y fijos de navegación (*Waypoints*) reales, indexada geográficamente mediante extensiones **PostGIS (GIST Indexes)**.
*   **Algoritmo de Enrutamiento Inteligente:** Motor de cálculo basado en variantes de Dijkstra/A* que calcula rutas siguiendo las "carreteras del cielo" físicas en lugar de líneas rectas artificiales.
*   **Cazador de Piernas Vacías (*Empty Legs*):** Algoritmo automatizado que detecta vuelos de reposicionamiento de aeronaves sin pasajeros y genera ofertas automáticas con tarifas de descuento en tiempo real.
*   **Telemetría y Tiempo Real:** Flujo constante de posicionamiento satelital de aeronaves simuladas administrado desde la infraestructura de replicación en tiempo real de **Supabase Realtime**.

### Panel Administrativo Back-Office (Blazor Server & WebAssembly)
*   **Tablero de Control Interactivo:** Interfaz SPA (Single Page Application) moderna construida en **Blazor** que permite monitorear el estado global de la flota (*Disponible, En Vuelo, Mantenimiento*).
*   **Gestión de Operaciones:** Despacho manual de aeronaves, control de la tripulación (`crew_assignments`) y monitoreo en vivo de solicitudes de vuelos.
*   **Auditoría Financiera:** Panel conectado de forma segura al historial de preautorizaciones y transacciones completadas mediante el SDK de **Stripe.net**.

### Cliente Pasajero (Android Native Kotlin)
*   **Render y Radar 3D:** Mapa interactivo en modo oscuro con *NASA WorldWind para Android* y *Google Maps Compose SDK*, proyectando tráfico aéreo en vivo alimentado por la red ADS-B de **OpenSky Network**.
*   **Visor de Cabina 3D:** Modelos tridimensionales ejecutivos (`.glb`) procesados con *Google Sceneview/Filament* para la selección táctil interactiva de asientos.
*   **Asistente Virtual de Operaciones (Google Gemini):** Integración avanzada de **Gemini 2.5 Pro** con *Function Calling/Tool Use*, capaz de procesar lenguaje natural para sugerir cambios de rutas o analizar el clima de destinos a través de reportes meteorológicos oficiales NOAA (METAR/TAF).

---

## Seguridad y Resguardo de Credenciales

El proyecto implementa estándares corporativos de ciberseguridad para evitar la filtración de datos sensibles:
*   **Protección de Repositorio:** El archivo `.gitignore` está optimizado para ocultar cachés de compilación de .NET (`bin/`, `obj/`).
*   **Administrador de Secretos de Usuario:** Las API Keys sensibles de Supabase, Stripe y Gemini no están escritas en el código fuente ni en el archivo `appsettings.json`. Se administran localmente a través de la herramienta cifrada fuera del proyecto de **User Secrets (`secrets.json`)**.

---

## Integrantes del Desarrollo

El diseño, arquitectura y programación de este ecosistema ha sido desarrollado por el siguiente equipo técnico:

*   **MIGUEL ANGEL CRUZ OLMEDO** —— **Carnet: 27-2723-2025**
*   **PEDRO RICARDO ORANTES CUELLAR** —— **Carnet: 27-6587-2013**
*   **ADRIANA MELISSA ROQUE DE PAZ** —— **Carnet: 25-0674-2022**
*   **JESÚS ENRIQUE MARTINEZ JUÁREZ** —— **Carnet: 27-0637-2025**
*   **GEOVANNY DANIEL BARRERA HERRERA** —— **Carnet: 25-0615-2022**
*   **ABNER ESAU VASQUEZ DIAZ** —— **Carnet: 23-3426-2022**
*   **DIEGO ALEJANDRO SAMAYOA TORRES** —— **Carnet: 17-2106-2021**
