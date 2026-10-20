using System;
using UnityEngine;
using UnityEngine.SceneManagement;
// MATHI: cinco mundos matemáticos, Numi, dos objetos educativos, fragmentos y acertijos.
public class MathiGame : MonoBehaviour
{
    public static MathiGame Instancia {get;private set;}
    private static readonly string[] Mundos={"BOSQUE DE LOS PATRONES","CIUDAD DE LOS NÚMEROS","REINO DE LAS FIGURAS","LABORATORIO DE LA LÓGICA","ESTACIÓN DEL DESAFÍO"};
    private static readonly string[] Preguntas={
      "¿Qué color continúa la secuencia?\nROJO  →  AZUL  →  VERDE  →  ROJO  →  AZUL  →  ?",
      "Completa la serie: 2, 4, 6, 8, ¿...?",
      "¿Cuántos lados tiene un triángulo?",
      "Si dos robots tienen dos baterías cada uno, ¿cuántas hay?",
      "Último reto: 3 + ? = 7. ¿Qué número falta?"};
    private static readonly string[,] Respuestas={
        {"ROJO","VERDE","AMARILLO"}, {"9","10","12"}, {"4","5","3"}, {"2","6","4"}, {"3","5","4"}};
    private static readonly int[] Correcta={1,1,2,2,2};
    private static readonly string[] Explicacion={
     "El patrón repite rojo, azul y verde. ¡Has descubierto la regla!",
     "La secuencia aumenta de dos en dos: 2, 4, 6, 8, 10.",
     "Un triángulo tiene exactamente tres lados y tres vértices.",
     "Cada uno de los dos robots tiene dos baterías: 2 × 2 = 4.",
     "Para llegar a siete desde tres, necesitas cuatro: 3 + 4 = 7."};
    private readonly bool[] fragmentoTomado=new bool[5];
    private Transform jugador;
    private Rigidbody2D cuerpo;
    private MathiUI ui;
    private GameObject decoracion;
    private GameObject fragmento;
    private GameObject puerta;
    private int nivel, fragmentos, vidas=3, puntuacion;
    private bool solucionado,dialogoActivo,lecturaActiva;
    private int linea;
    private float tiempoMensaje;
    private const float SEPARACION=19.2f;
    private void Awake()
    {
        Instancia=this;
        GameObject personaje=GameObject.Find("Player");
        if(personaje==null){Debug.LogError("MATHI: falta GameObject Player.");enabled=false;return;}
        jugador=personaje.transform;cuerpo=personaje.GetComponent<Rigidbody2D>();
        ui=gameObject.GetComponent<MathiUI>();if(ui==null)ui=gameObject.AddComponent<MathiUI>();
    }
    private void Start(){EntrarMundo(0);}
    private void OnDestroy(){if(Instancia==this)Instancia=null;}
    private void Update()
    {
        if(ui==null || jugador==null || ui.VictoriaAbierta)return;
        if(ui.PreguntaAbierta)
        {
            if(ui.Bloqueado && !ui.PreguntaAbierta)return;
            if(Input.GetKeyDown(KeyCode.Alpha1))Resolver(0);
            if(Input.GetKeyDown(KeyCode.Alpha2))Resolver(1);
            if(Input.GetKeyDown(KeyCode.Alpha3))Resolver(2);
            return;
        }
        if(Input.GetKeyDown(KeyCode.E) && ui.DialogoAbierto)
        {
            if(dialogoActivo)SiguienteFrase();else {ui.CerrarPanel();lecturaActiva=false;}
            return;
        }
        if(!ui.Bloqueado)
        {
            if(!fragmentoTomado[nivel] && Mathf.Abs(jugador.position.x-(nivel*SEPARACION+2.4f)) < .61f)
            {
                fragmentoTomado[nivel]=true;fragmentos++;puntuacion+=20;
                if(fragmento!=null)fragmento.SetActive(false);
                ActualizarHUD();ui.Avisar("¡Encontraste un fragmento del Cristal!",2.8f);
            }
            if(Input.GetKeyDown(KeyCode.E))Interactuar();
            else
            {
                float rel=jugador.position.x-nivel*SEPARACION;
                string indicacion= Mathf.Abs(rel+3.1f)<1.5f ? "E: HABLAR CON NUMI" :
                    Mathf.Abs(rel+1.2f)<1.25f ? "E: LEER CUADERNO" :
                    Mathf.Abs(rel-1.0f)<1.25f ? "E: CONSULTAR PISTAS" :
                    Mathf.Abs(rel-5.3f)<1.5f ? "E: RESOLVER / CRUZAR PORTAL" :
                    "A / D: MOVER  ·  E: INTERACTUAR";
                ui.Indicacion(indicacion);
            }
        }
    }
    private void Interactuar()
    {
        float x=jugador.position.x-nivel*SEPARACION;
        if(Mathf.Abs(x+3.1f)<1.5f){AbrirConversacion();return;}
        if(Mathf.Abs(x+1.2f)<1.25f)
        {
            lecturaActiva=true; ui.MostrarTexto("CUADERNO DE MATHI", "Cada desafío tiene una regla matemática. Observa con calma, busca lo que se repite o cambia y prueba una respuesta. Aprender es descubrir patrones.");return;
        }
        if(Mathf.Abs(x-1f)<1.25f)
        {
            lecturaActiva=true;ui.MostrarTexto("CONSEJO DE NUMI",Consejo());return;
        }
        if(Mathf.Abs(x-5.3f)<1.5f)
        {
            if(!solucionado){ui.MostrarPregunta(Preguntas[nivel],Respuestas[nivel,0],Respuestas[nivel,1],Respuestas[nivel,2],Resolver);return;}
            if(!fragmentoTomado[nivel]){ui.Avisar("Antes de salir, recupera el fragmento brillante del nivel.",3);return;}
            if(nivel==4){ui.Victoria(fragmentos,puntuacion,()=>Reiniciar(),()=>SceneManager.LoadScene("MainMenu"));return;}
            EntrarMundo(nivel+1);
        }
    }
    private string Consejo()
    {
        string[] consejos={"Busca la secuencia que se repite cada tres colores.","Suma la misma cantidad en cada paso de la serie.","Cuenta los lados de cada figura sin confundirlos con sus esquinas.","Agrupa los objetos por robot y suma sus cantidades.","Piensa qué número debes agregar a tres para obtener siete."};
        return consejos[nivel]+"\n\nPista: cometer errores es parte de aprender.";
    }
    private void AbrirConversacion()
    {
        dialogoActivo=true;linea=0;ui.MostrarTexto("NUMI · ROBOT GUÍA",TextoNumi(linea));
    }
    private void SiguienteFrase()
    {
        linea++;
        if(linea>=4){dialogoActivo=false;ui.CerrarPanel();return;}
        ui.MostrarTexto("NUMI · ROBOT GUÍA",TextoNumi(linea));
    }
    private string TextoNumi(int indice)
    {
        switch(indice)
        {
          case 0:return "¡Hola, Mathi! Soy Numi, tu compañero. ¡Caos ha desordenado el Mundo Matemático! Necesitamos tus acertijos para repararlo.";
          case 1:return "Estamos en " + Mundos[nivel] + ". Aquí nos espera un acertijo.";
          case 2:return "Busca el fragmento brillante del Cristal del Conocimiento y resuelve el desafío del portal.";
          default:return "Usa E para investigar. En las preguntas, elige con el mouse o con las teclas 1, 2 y 3. ¡Tú puedes!";
        }
    }
    private void Resolver(int opcion)
    {
        if(opcion==Correcta[nivel])
        {
            solucionado=true;puntuacion+=100;
            ui.CerrarPregunta();ActualizarHUD();
            ui.Avisar("¡RESPUESTA CORRECTA! " + Explicacion[nivel] + " Acércate al portal y pulsa E.",6f);
            if(puerta!=null){var sr=puerta.GetComponent<SpriteRenderer>();if(sr!=null)sr.color=new Color(.5f,1f,.85f);}
        }
        else
        {
            vidas--;ActualizarHUD();
            if(vidas<=0)
            {
                ui.CerrarPregunta();ui.MostrarDerrota("Numi dice: vuelve a intentarlo. ¡Cada error te enseña algo!",()=>Reintentar());
            }
            else ui.ResultadoIncorrecto("Todavía no. " + Consejo() + "  ·  Vidas: " + vidas);
        }
    }
    private void Reintentar(){vidas=3;ui.CerrarDerrota();ActualizarHUD();ui.Avisar("¡Ánimo, Mathi! Prueba otra estrategia.",3);}
    private void Reiniciar(){SceneManager.LoadScene("GameScene");}
    private void EntrarMundo(int nuevo)
    {
        nivel=nuevo;solucionado=false;vidas=3;dialogoActivo=false;lecturaActiva=false;
        if(decoracion!=null)Destroy(decoracion);
        decoracion=new GameObject("Mundo_"+(nivel+1)+"_ObjetosInteractivos");
        float origen=nivel*SEPARACION;
        jugador.position=new Vector3(origen,-1.75f,0);
        if(cuerpo!=null){cuerpo.position=new Vector2(origen,-1.75f);cuerpo.linearVelocity=Vector2.zero;}
        if(Camera.main!=null)Camera.main.transform.position=new Vector3(origen,1,-10);
        CrearSprite("Numi_Guia", "Mathi/Numi",origen-3.1f,-.85f,.31f,32);
        CrearSprite("CuadernoEducativo", "Mathi/Libro",origen-1.2f,-1f,.36f,30);
        CrearSprite("ConsolaDePistas", "Mathi/Consola",origen+1f,-1f,.36f,30);
        fragmento=CrearSprite("FragmentoCristal", "Mathi/Fragmento",origen+2.4f,-.72f,.32f,33);
        if(fragmento!=null) fragmento.SetActive(!fragmentoTomado[nivel]);
        puerta=CrearSprite("PortalAlSiguienteMundo", nivel==0 ? "Mathi/Puente" : "Mathi/Portal",origen+5.3f,-.52f,.60f,24);
        if(nivel==4) CrearSprite("Caos_Antagonista", "Mathi/Caos",origen+6.55f,.75f,.36f,28);
        CrearLimite("MuroDelNivel_Izquierdo",origen-8.2f);
        CrearLimite("MuroDelNivel_Derecho",origen+7.75f);
        ActualizarHUD();ui.Indicacion("MUNDO " + (nivel+1) + " · HABLA CON NUMI Y RESUELVE EL RETO");
    }
    private GameObject CrearSprite(string nombre,string recurso,float x,float y,float escala,int orden)
    {
        Sprite sprite=Resources.Load<Sprite>(recurso);
        if(sprite==null){Debug.LogError("Falta recurso de MATHI: "+recurso);return null;}
        var obj=new GameObject(nombre,typeof(SpriteRenderer));obj.transform.SetParent(decoracion.transform);
        obj.transform.position=new Vector3(x,y,0);obj.transform.localScale=new Vector3(escala,escala,1);
        var sr=obj.GetComponent<SpriteRenderer>();sr.sprite=sprite;sr.sortingOrder=orden;return obj;
    }
    private void CrearLimite(string nombre,float x)
    {
        var obj=new GameObject(nombre,typeof(BoxCollider2D),typeof(SpriteRenderer));obj.transform.SetParent(decoracion.transform);
        obj.transform.position=new Vector3(x,-.8f,0);
        var col=obj.GetComponent<BoxCollider2D>();col.size=new Vector2(.5f,5f);col.isTrigger=false;
        var sr=obj.GetComponent<SpriteRenderer>();sr.sprite=Resources.Load<Sprite>("Mathi/Barrera");sr.sortingOrder=25;
        // Dibujar la barrera a escala sin modificar la colisión física del muro.
        GameObject imagen=new GameObject("BrilloMuro",typeof(SpriteRenderer));imagen.transform.SetParent(obj.transform,false);
        imagen.GetComponent<SpriteRenderer>().sprite=sr.sprite;sr.enabled=false;
        imagen.transform.localScale=new Vector3(.38f,.45f,1f);
        imagen.GetComponent<SpriteRenderer>().sortingOrder=25;
    }
    private void ActualizarHUD(){if(ui!=null)ui.Actualizar(nivel,Mundos[nivel],fragmentos,vidas,puntuacion,solucionado);}
}
