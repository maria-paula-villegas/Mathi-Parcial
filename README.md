# ✨ MATHI: La aventura de los acertijos

**Serious Game 2D de aventura y aprendizaje matemático, desarrollado en Unity.**

MATHI es una aventura para niños y niñas de **7 a 12 años**. El jugador acompaña a Mathi y a su compañero robot **Numi** para superar cinco mundos de desafíos lógico-matemáticos y recuperar los fragmentos del **Cristal del Conocimiento**. El antagonista **Caos** ha desordenado el Mundo Matemático y los acertijos permiten restaurarlo.

## Equipo

- **María Paula Gallego**
- **María Paula Villegas**

**Asignatura:** Desarrollo de Contenidos Digitales y Multimediales  
**Docente:** Luz Karime Castellanos Joya  
**Institución:** Unidades Tecnológicas de Santander (UTS)

## 🌍 Mundos del juego

| Mundo | Tema educativo | Ejemplo de reto |
| --- | --- | --- |
| 1. Bosque de los Patrones | Secuencias y observación | Continuar una secuencia de colores |
| 2. Ciudad de los Números | Series numéricas | Identificar el número faltante |
| 3. Reino de las Figuras | Geometría básica | Reconocer propiedades de las figuras |
| 4. Laboratorio de la Lógica | Razonamiento | Resolver problemas de conteo |
| 5. Estación del Desafío | Integración de aprendizajes | Resolver el acertijo final |

## 🎮 Mecánicas principales

- Movimiento horizontal de Mathi con animaciones de reposo y caminata.
- Escenario 2D con colisiones y cámara de seguimiento.
- Diálogos educativos con **Numi**, activados con la tecla **E**.
- Interacción con un **cuaderno educativo** y una **consola de pistas**.
- Cinco fragmentos del Cristal del Conocimiento coleccionables (uno por mundo).
- Preguntas con tres respuestas y retroalimentación inmediata.
- Sistema de vidas, puntos, progreso y recompensas visible en el HUD.
- Portales que permiten avanzar de mundo al resolver el reto y recuperar el fragmento.
- Menú principal, créditos, premios y cierre de la aventura.

## 🕹️ Controles

| Control | Acción |
| --- | --- |
| `A` / `D` o flechas izquierda/derecha | Mover a Mathi |
| `E` | Dialogar, consultar objetos y activar portales |
| `1`, `2`, `3` o clic del ratón | Elegir una respuesta |
| Botones en pantalla | Navegar por el menú y reiniciar |

## 🗂️ Código fuente

```text
Assets/
└── Scripts/
    ├── Core/
    │   ├── CameraFollow2D.cs
    │   └── ConstruirEntorno.cs
    ├── Player/
    │   └── PlayerController.cs
    ├── Previo2/
    │   ├── MathiGame.cs
    │   └── MathiUI.cs
    └── UI/
        └── MainMenuManager.cs
README.md
.gitignore
```

## 🔧 Tecnología y proyecto Unity

- Motor: **Unity 6.3 LTS**.
- Lenguaje: **C#**.
- Tipo de proyecto: videojuego educativo **2D**, un jugador.
- Escenas del proyecto completo: `MainMenu` y `GameScene`.

**Este repositorio está preparado para la entrega del código fuente del Previo 2.** No incluye las imágenes, animaciones, escenas ni el proyecto Unity completo. Para ejecutar el videojuego, abre el proyecto completo de MATHI en Unity 6.3 LTS; este paquete de GitHub sirve para consultar los scripts y la documentación principal.

## 📚 Contexto académico

El desarrollo sigue el documento de conceptualización *MATHI: La aventura de los acertijos* y las guías de Taller Práctico 2 y Previo 2 de la asignatura. El Previo 2 contempla diálogo con NPC, dos objetos interactivos, recolección, HUD y validación funcional P1–P10.

**Nota sobre validación:** la revisión de archivos de este repositorio no sustituye las pruebas de ejecución, la evidencia gráfica ni la exportación del juego en Unity.
