using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

// Pantallas del menú alineadas con la ilustración: escalado uniforme 16:9.
public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject creditsPanel;
    private RectTransform contenido;
    private GameObject vistaCreditos, vistaNiveles, vistaPremios, vistaPerfil, vistaAjustes;
    private int ultimoAncho=-1, ultimoAlto=-1;
    private const float Ancho=960f, Alto=540f;
    private void Awake()
    {
        Sprite arte=null,creditos=null;
        GameObject fondo=GameObject.Find("ImagenFondoMenu");
        if(fondo != null) { var sr=fondo.GetComponent<SpriteRenderer>(); if(sr!=null) {arte=sr.sprite;sr.enabled=false;} }
        if(creditsPanel!=null){var sr=creditsPanel.GetComponent<SpriteRenderer>();if(sr!=null)creditos=sr.sprite;creditsPanel.SetActive(false);}
        GameObject lienzo = new GameObject("Canvas_MATHI", typeof(RectTransform),typeof(Canvas),typeof(CanvasScaler),typeof(GraphicRaycaster));
        lienzo.GetComponent<Canvas>().renderMode=RenderMode.ScreenSpaceOverlay;
        lienzo.GetComponent<CanvasScaler>().uiScaleMode=CanvasScaler.ScaleMode.ConstantPixelSize;
        GameObject grupo=new GameObject("PantallaCentrada",typeof(RectTransform));grupo.transform.SetParent(lienzo.transform,false);
        contenido=grupo.GetComponent<RectTransform>();contenido.anchorMin=contenido.anchorMax=new Vector2(.5f,.5f);
        contenido.pivot=new Vector2(.5f,.5f);contenido.sizeDelta=new Vector2(Ancho,Alto);
        Imagen(grupo.transform,"ArteMenu",arte);
        Boton(grupo.transform,"Jugar",127,-1,360,55,PlayGame);
        Boton(grupo.transform,"Niveles",127,-65,360,55,OpenLevels);
        Boton(grupo.transform,"Creditos",127,-128,360,55,OpenCredits);
        Boton(grupo.transform,"Premios",-60,-183,103,36,OpenAwards);
        Boton(grupo.transform,"Perfil",41,-183,103,36,OpenProfile);
        Boton(grupo.transform,"Ajustes",142,-183,103,36,OpenSettings);
        Boton(grupo.transform,"Salir",243,-183,103,36,QuitGame);
        var p=Imagen(grupo.transform,"PantallaCreditos",creditos);
        vistaCreditos=p.gameObject;
        Boton(p,"CerrarCreditos",5,-159,245,50,CloseCredits);
        vistaCreditos.SetActive(false);
        var n=Imagen(grupo.transform,"PantallaNiveles",Resources.Load<Sprite>("Mathi/PantallaNiveles"));
        vistaNiveles=n.gameObject;
        Boton(n,"CerrarNiveles",5,-173,245,50,CloseLevels);
        vistaNiveles.SetActive(false);
        vistaPremios=CrearExtra(grupo.transform,"Mathi/PantallaPremios",OpenAwardsOff);
        vistaPerfil=CrearExtra(grupo.transform,"Mathi/PantallaPerfil",OpenProfileOff);
        vistaAjustes=CrearExtra(grupo.transform,"Mathi/PantallaAjustes",OpenSettingsOff);
        if(FindFirstObjectByType<EventSystem>()==null)
            new GameObject("SistemaEventos",typeof(EventSystem),typeof(StandaloneInputModule));
        Ajustar();
    }
    private RectTransform Imagen(Transform padre,string nombre,Sprite arte)
    {
        var obj=new GameObject(nombre,typeof(RectTransform),typeof(Image));obj.transform.SetParent(padre,false);
        var r=obj.GetComponent<RectTransform>();r.anchorMin=r.anchorMax=r.pivot=new Vector2(.5f,.5f);
        r.sizeDelta=new Vector2(Ancho,Alto);r.anchoredPosition=Vector2.zero;
        var image=obj.GetComponent<Image>();image.sprite=arte;image.raycastTarget=false;return r;
    }
    private void Boton(Transform padre,string nombre,float x,float y,float w,float h,UnityAction accion)
    {
        var obj=new GameObject(nombre,typeof(RectTransform),typeof(Image),typeof(Button));obj.transform.SetParent(padre,false);
        var r=obj.GetComponent<RectTransform>();r.anchorMin=r.anchorMax=r.pivot=new Vector2(.5f,.5f);r.anchoredPosition=new Vector2(x,y);r.sizeDelta=new Vector2(w,h);
        var i=obj.GetComponent<Image>();i.color=new Color(1,1,1,.001f);i.raycastTarget=true;
        var b=obj.GetComponent<Button>();b.transition=Selectable.Transition.None;b.onClick.AddListener(accion);
    }
    private void LateUpdate(){
        if(Input.GetKeyDown(KeyCode.F11))Screen.fullScreen=!Screen.fullScreen;
        if(ultimoAncho!=Screen.width || ultimoAlto!=Screen.height) Ajustar();
    }
    private void Ajustar(){ultimoAncho=Screen.width;ultimoAlto=Screen.height;
        if(contenido!=null)contenido.localScale=Vector3.one*Mathf.Max(.01f,Mathf.Min(Screen.width/Ancho,Screen.height/Alto));}
    public void PlayGame(){SceneManager.LoadScene("GameScene");}
    public void OpenCredits(){ if(vistaCreditos!=null)vistaCreditos.SetActive(true);}
    public void CloseCredits(){ if(vistaCreditos!=null)vistaCreditos.SetActive(false);}
    public void OpenLevels(){ if(vistaNiveles!=null)vistaNiveles.SetActive(true);}
    public void CloseLevels(){ if(vistaNiveles!=null)vistaNiveles.SetActive(false);}

    private GameObject CrearExtra(Transform padre,string ruta,UnityAction cerrar)
    {
        var r=Imagen(padre,ruta,Resources.Load<Sprite>(ruta));
        Boton(r,"Volver",5,-159,245,50,cerrar);
        r.gameObject.SetActive(false);return r.gameObject;
    }
    public void OpenAwards(){if(vistaPremios!=null)vistaPremios.SetActive(true);}
    public void OpenAwardsOff(){if(vistaPremios!=null)vistaPremios.SetActive(false);}
    public void OpenProfile(){if(vistaPerfil!=null)vistaPerfil.SetActive(true);}
    public void OpenProfileOff(){if(vistaPerfil!=null)vistaPerfil.SetActive(false);}
    public void OpenSettings(){if(vistaAjustes!=null)vistaAjustes.SetActive(true);}
    public void OpenSettingsOff(){if(vistaAjustes!=null)vistaAjustes.SetActive(false);}
    public void QuitGame(){
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying=false;
#else
        Application.Quit();
#endif
    }
}
