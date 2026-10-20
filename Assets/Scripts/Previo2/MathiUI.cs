using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System;
// Interfaz 1920x1080 con modal exclusivo: no pueden solaparse diálogos y acertijos.
public class MathiUI : MonoBehaviour
{
    public static MathiUI Instancia {get;private set;}
    public bool DialogoAbierto{get;private set;}
    public bool PreguntaAbierta{get;private set;}
    public bool VictoriaAbierta{get;private set;}
    public bool Bloqueado{get{return DialogoAbierto||PreguntaAbierta||VictoriaAbierta||derrotaActiva;}}
    private bool derrotaActiva;
    private Text tituloMundo,objetivo,puntuacionTexto,vidasTexto,fragmentosTexto,pista,dialogTitulo,dialogCuerpo,preguntaTexto,resultado;
    private Text[] opcionesTexto=new Text[3];
    private Button[] opcionesBotones=new Button[3];
    private Image[] estrellas=new Image[5];
    private GameObject ventanaDialogo,ventanaPregunta,ventanaVictoria,ventanaDerrota;
    private Action<int> alResponder;
    private Font fuente;
    private float mensajeHasta;
    private static readonly Color Fondo=new Color(.08f,.07f,.24f,.97f);
    private static readonly Color TextoBlanco=new Color(.98f,.98f,1f);
    private static readonly Color Violeta=new Color(.35f,.27f,.75f);
    private static readonly Color Aqua=new Color(.38f,.95f,.93f);
    private void Awake(){Instancia=this;fuente=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");CrearUI();}
    private void OnDestroy(){if(Instancia==this)Instancia=null;}
    private static RectTransform R(Transform p,string n,Vector2 an,Vector2 xy,Vector2 size)
    {
        var o=new GameObject(n,typeof(RectTransform));o.transform.SetParent(p,false);
        var r=o.GetComponent<RectTransform>();r.anchorMin=r.anchorMax=r.pivot=an;r.anchoredPosition=xy;r.sizeDelta=size;return r;
    }
    private static RectTransform Panel(Transform p,string n,Vector2 an,Vector2 xy,Vector2 size,Color color)
    {
        var r=R(p,n,an,xy,size);r.gameObject.AddComponent<Image>().color=color;return r;
    }
    private Text Texto(Transform p,string n,string valor,int size,Color col,TextAnchor align,Vector2 anch,Vector2 xy,Vector2 wh)
    {
        var r=R(p,n,anch,xy,wh);var t=r.gameObject.AddComponent<Text>();t.font=fuente;t.fontSize=size;t.color=col;t.alignment=align;t.text=valor;
        t.horizontalOverflow=HorizontalWrapMode.Wrap;t.verticalOverflow=VerticalWrapMode.Truncate;t.raycastTarget=false;return t;
    }
    private Button Boton(Transform p,string n,string label,Vector2 xy,Vector2 wh,Color fondo,UnityAction action)
    {
        var r=Panel(p,n,new Vector2(.5f,.5f),xy,wh,fondo);
        var b=r.gameObject.AddComponent<Button>();b.onClick.AddListener(action);
        var cb=b.colors;cb.highlightedColor=new Color(.87f,.87f,1f);cb.pressedColor=new Color(.55f,.52f,.86f);b.colors=cb;
        Texto(r,"Etiqueta",label,36,TextoBlanco,TextAnchor.MiddleCenter,new Vector2(.5f,.5f),Vector2.zero,wh-new Vector2(12,8));
        return b;
    }
    private void CrearUI()
    {
        var root=new GameObject("Canvas_MATHI_Game",typeof(RectTransform),typeof(Canvas),typeof(CanvasScaler),typeof(GraphicRaycaster));
        root.GetComponent<Canvas>().renderMode=RenderMode.ScreenSpaceOverlay;
        var scaler=root.GetComponent<CanvasScaler>();scaler.uiScaleMode=CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution=new Vector2(1920,1080);scaler.screenMatchMode=CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;scaler.matchWidthOrHeight=.5f;
        if(FindFirstObjectByType<EventSystem>()==null)new GameObject("SistemaEventos",typeof(EventSystem),typeof(StandaloneInputModule));
        var hud=Panel(root.transform,"HUD_Superior",new Vector2(.5f,1f),new Vector2(0,-20),new Vector2(1720,140),Fondo);
        tituloMundo=Texto(hud,"NombreMundo","MUNDO 1",37,Aqua,TextAnchor.MiddleLeft,new Vector2(0,.5f),new Vector2(50,21),new Vector2(1060,60));
        objetivo=Texto(hud,"Objetivo","",26,TextoBlanco,TextAnchor.MiddleLeft,new Vector2(0,.5f),new Vector2(50,-30),new Vector2(1070,46));
        puntuacionTexto=Texto(hud,"Puntos","PUNTOS 0",30,new Color(1f,.86f,.5f),TextAnchor.MiddleRight,new Vector2(1,.5f),new Vector2(-50,42),new Vector2(395,48));
        vidasTexto=Texto(hud,"Vidas","VIDAS ♥♥♥",30,TextoBlanco,TextAnchor.MiddleRight,new Vector2(1,.5f),new Vector2(-50,0),new Vector2(395,48));
        fragmentosTexto=Texto(hud,"Cristal","0 / 5",30,Aqua,TextAnchor.MiddleRight,new Vector2(1,.5f),new Vector2(-50,-40),new Vector2(395,48));
        for(int i=0;i<5;i++)
        {
            var r=Panel(root.transform,"CristalCasilla_"+(i+1),new Vector2(0,1),new Vector2(120+i*82,-178),new Vector2(65,65),Violeta);
            estrellas[i]=r.GetComponent<Image>();
            Texto(r,"Estrella","★",38,TextoBlanco,TextAnchor.MiddleCenter,new Vector2(.5f,.5f),Vector2.zero,new Vector2(62,61));
        }
        pista=Texto(root.transform,"PistaTeclado","A / D: MOVER  ·  E: INTERACTUAR",32,TextoBlanco,TextAnchor.MiddleCenter,new Vector2(.5f,0),new Vector2(0,68),new Vector2(1500,85));
        ventanaDialogo=Panel(root.transform,"Dialogo_Unico",new Vector2(.5f,.5f),new Vector2(0,-125),new Vector2(1410,390),Fondo).gameObject;
        dialogTitulo=Texto(ventanaDialogo.transform,"NombreNPC","NUMI",43,Aqua,TextAnchor.MiddleLeft,new Vector2(0,1),new Vector2(65,-62),new Vector2(1280,65));
        dialogCuerpo=Texto(ventanaDialogo.transform,"MensajeNPC","",35,TextoBlanco,TextAnchor.MiddleLeft,new Vector2(.5f,.5f),new Vector2(0,-12),new Vector2(1270,210));
        Texto(ventanaDialogo.transform,"TeclaContinuar","PULSA E PARA CONTINUAR",25,Aqua,TextAnchor.MiddleRight,new Vector2(1,0),new Vector2(-70,35),new Vector2(540,45));
        ventanaPregunta=Panel(root.transform,"VentanaAcertijo",new Vector2(.5f,.5f),new Vector2(0,-70),new Vector2(1430,680),Fondo).gameObject;
        Texto(ventanaPregunta.transform,"Etiqueta","✦  DESAFÍO MATEMÁTICO  ✦",35,Aqua,TextAnchor.MiddleCenter,new Vector2(.5f,1),new Vector2(0,-67),new Vector2(1320,75));
        preguntaTexto=Texto(ventanaPregunta.transform,"Enunciado","",46,TextoBlanco,TextAnchor.MiddleCenter,new Vector2(.5f,.5f),new Vector2(0,105),new Vector2(1330,235));
        for(int i=0;i<3;i++)
        {
            int index=i;
            opcionesBotones[i]=Boton(ventanaPregunta.transform,"Respuesta_"+(i+1),"",new Vector2(-440+i*440,-125),new Vector2(396,130),Violeta,()=>{if(alResponder!=null)alResponder(index);});
            opcionesTexto[i]=opcionesBotones[i].GetComponentInChildren<Text>();
        }
        resultado=Texto(ventanaPregunta.transform,"Feedback","USA 1, 2, 3 O HAZ CLIC",29,Aqua,TextAnchor.MiddleCenter,new Vector2(.5f,0),new Vector2(0,66),new Vector2(1290,120));
        ventanaDerrota=Panel(root.transform,"PanelNuevoIntento",new Vector2(.5f,.5f),Vector2.zero,new Vector2(1170,480),Fondo).gameObject;
        Texto(ventanaDerrota.transform,"Titulo","¡SIGUE INTENTÁNDOLO!",65,Aqua,TextAnchor.MiddleCenter,new Vector2(.5f,.5f),new Vector2(0,142),new Vector2(1050,100));
        Boton(ventanaDerrota.transform,"BotonReintentar","REINTENTAR",new Vector2(0,-140),new Vector2(490,115),Violeta,()=>{if(reintentar!=null)reintentar();});
        ventanaVictoria=Panel(root.transform,"CristalRestaurado",new Vector2(.5f,.5f),Vector2.zero,new Vector2(1390,690),Fondo).gameObject;
        Texto(ventanaVictoria.transform,"Titulo","¡EL CRISTAL ESTÁ RESTAURADO!",68,Aqua,TextAnchor.MiddleCenter,new Vector2(.5f,.5f),new Vector2(0,210),new Vector2(1310,110));
        Texto(ventanaVictoria.transform,"Cierre","¡Salvaste el Mundo Matemático!\nNumi y Mathi celebran tus cinco desafíos resueltos.",41,TextoBlanco,TextAnchor.MiddleCenter,new Vector2(.5f,.5f),new Vector2(0,20),new Vector2(1280,220));
        Boton(ventanaVictoria.transform,"BotonRepetir","VOLVER A JUGAR",new Vector2(-300,-210),new Vector2(520,114),Violeta,()=>{if(nuevaPartida!=null)nuevaPartida();});
        Boton(ventanaVictoria.transform,"BotonMenu","MENÚ PRINCIPAL",new Vector2(300,-210),new Vector2(520,114),Violeta,()=>{if(irMenu!=null)irMenu();});
        ventanaDialogo.SetActive(false);ventanaPregunta.SetActive(false);ventanaVictoria.SetActive(false);ventanaDerrota.SetActive(false);
    }
    private Action reintentar,nuevaPartida,irMenu;
    public void Actualizar(int nivel,string mundo,int piezas,int vidas,int puntos,bool portal)
    {
        if(tituloMundo==null)return;
        tituloMundo.text="MUNDO " + (nivel+1) + " · " + mundo;
        objetivo.text=portal ? "¡PORTAL ABIERTO! RECUPERA EL FRAGMENTO Y AVANZA" : "EXPLORA, RECUPERA EL FRAGMENTO Y RESUELVE EL RETO";
        puntuacionTexto.text="PUNTOS  " + puntos;
        vidasTexto.text="VIDAS  " + new string('♥',Mathf.Clamp(vidas,0,3));
        fragmentosTexto.text="CRISTAL  " + piezas + " / 5";
        for(int i=0;i<5;i++)estrellas[i].color=i<piezas ? new Color(1f,.73f,.35f) : Violeta;
    }
    public void Indicacion(string mensaje){if(pista!=null && Time.time>=mensajeHasta && !Bloqueado)pista.text=mensaje;}
    public void Avisar(string mensaje,float segundos)
    {if(pista==null)return;pista.text=mensaje;mensajeHasta=Time.time+segundos;}
    public void MostrarTexto(string titulo,string mensaje)
    {if(PreguntaAbierta || VictoriaAbierta || derrotaActiva)return;DialogoAbierto=true;dialogTitulo.text=titulo;dialogCuerpo.text=mensaje;ventanaDialogo.SetActive(true);}
    public void CerrarPanel(){DialogoAbierto=false;ventanaDialogo.SetActive(false);Avisar("A / D: MOVER  ·  E: INTERACTUAR",1);}
    public void MostrarPregunta(string enunciado,string a,string b,string c,Action<int> respuesta)
    {
        if(Bloqueado)return;PreguntaAbierta=true;alResponder=respuesta;
        preguntaTexto.text=enunciado;opcionesTexto[0].text="1  ·  " + a;opcionesTexto[1].text="2  ·  " + b;opcionesTexto[2].text="3  ·  " + c;
        resultado.text="ELIGE CON EL MOUSE O LAS TECLAS 1, 2 Y 3";ventanaPregunta.SetActive(true);
    }
    public void ResultadoIncorrecto(string mensaje){if(resultado!=null)resultado.text=mensaje;}
    public void CerrarPregunta(){PreguntaAbierta=false;ventanaPregunta.SetActive(false);alResponder=null;}
    public void MostrarDerrota(string texto,Action volver)
    {derrotaActiva=true;reintentar=volver;ventanaDerrota.SetActive(true);}
    public void CerrarDerrota(){derrotaActiva=false;ventanaDerrota.SetActive(false);}
    public void Victoria(int piezas,int puntos,Action reinicio,Action menu)
    {VictoriaAbierta=true;nuevaPartida=reinicio;irMenu=menu;ventanaVictoria.SetActive(true);}
}
