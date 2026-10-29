using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

// Construye el marcador, el menú principal, la pantalla de vidas y el Game Over.
// Menú: Herramientas > Construir pantallas
public static class ConstructorPantallas
{
    private const string Escenas = "Assets/Scenes/";
    private const string RutaMarcador = "Assets/Prefabs/Marcador.prefab";
    private const string RutaFuente = "Assets/Fonts/PressStart2P-Regular.ttf";

    private static readonly Color Blanco = Color.white;
    private static readonly Color Ladrillo = new Color32(200, 76, 12, 255);
    private static readonly Color Durazno = new Color32(252, 188, 176, 255);

    private static Font fuente;

    [MenuItem("Herramientas/Construir pantallas")]
    public static void Construir()
    {
        PrepararFuente();
        CrearMarcador();

        AgregarMarcador(Escenas + "1-1.unity");
        AgregarMarcador(Escenas + "1-2.unity");

        ConstruirMenu();
        ConstruirVidas();
        ConstruirGameOver();

        ConstructorNiveles.OrdenarBuild();
        Debug.Log("[Constructor] Pantallas listas");
    }

    private static void PrepararFuente()
    {
        // render nítido, sin suavizado, para que se vea pixelada
        var importador = (TrueTypeFontImporter)AssetImporter.GetAtPath(RutaFuente);
        importador.fontRenderingMode = FontRenderingMode.HintedRaster;
        importador.SaveAndReimport();
        fuente = AssetDatabase.LoadAssetAtPath<Font>(RutaFuente);
    }

    // Marcador de arriba: vidas, monedas y mundo
    private static void CrearMarcador()
    {
        GameObject lienzo = Lienzo("Marcador", 10);
        Hud hud = lienzo.AddComponent<Hud>();

        hud.livesText = Texto(lienzo.transform, "MARIO\n  x3", 24, new Vector2(0.1f, 1f), new Vector2(0f, -48f), TextAnchor.UpperLeft);
        Imagen(lienzo.transform, "Coin", new Vector2(0.44f, 1f), new Vector2(0f, -86f), new Vector2(40f, 40f));
        hud.coinsText = Texto(lienzo.transform, "x00", 24, new Vector2(0.44f, 1f), new Vector2(110f, -74f), TextAnchor.UpperLeft);
        hud.worldText = Texto(lienzo.transform, "MUNDO\n 1-1", 24, new Vector2(0.72f, 1f), new Vector2(0f, -48f), TextAnchor.UpperLeft);

        PrefabUtility.SaveAsPrefabAsset(lienzo, RutaMarcador);
        Object.DestroyImmediate(lienzo);
    }

    public static void AgregarMarcador(string rutaEscena)
    {
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(RutaMarcador);
        if (prefab == null || !System.IO.File.Exists(rutaEscena)) return;

        var escena = EditorSceneManager.OpenScene(rutaEscena);
        if (Object.FindFirstObjectByType<Hud>() == null) {
            PrefabUtility.InstantiatePrefab(prefab);
        }
        EditorSceneManager.MarkSceneDirty(escena);
        EditorSceneManager.SaveScene(escena);
    }

    // Menú: el inicio del 1-1 de fondo, con el cartel del título encima
    private static void ConstruirMenu()
    {
        string ruta = Escenas + "Menu.unity";
        AssetDatabase.DeleteAsset(ruta);
        AssetDatabase.CopyAsset(Escenas + "1-1.unity", ruta);
        var escena = EditorSceneManager.OpenScene(ruta);

        // nada se mueve en el fondo
        foreach (var enemigo in Object.FindObjectsByType<EntityMovement>(FindObjectsInactive.Include, FindObjectsSortMode.None)) {
            Object.DestroyImmediate(enemigo.gameObject);
        }
        Player mario = Object.FindFirstObjectByType<Player>();
        mario.GetComponent<PlayerMovement>().enabled = false;
        mario.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;

        GameObject lienzo = Lienzo("Menu", 5);
        MainMenu menu = lienzo.AddComponent<MainMenu>();

        // cartel del título: borde claro y fondo ladrillo
        RectTransform borde = Panel(lienzo.transform, Durazno, new Vector2(0f, 110f), new Vector2(780f, 300f));
        RectTransform cartel = Panel(borde, Ladrillo, Vector2.zero, new Vector2(764f, 284f));
        Text titulo = Texto(cartel, "SUPER\nMARIO BROS.", 56, new Vector2(0.5f, 0.5f), new Vector2(0f, 26f), TextAnchor.MiddleCenter);
        titulo.color = Durazno;
        titulo.lineSpacing = 1.4f;
        titulo.gameObject.AddComponent<Shadow>().effectDistance = new Vector2(5f, -5f);
        Text firma = Texto(cartel, "©2026 VIDEOJUEGOS", 16, new Vector2(0.5f, 0.5f), new Vector2(170f, -116f), TextAnchor.MiddleCenter);
        firma.color = Durazno;

        RectTransform jugar = Texto(lienzo.transform, "1 JUGADOR", 28, new Vector2(0.5f, 0.5f), new Vector2(-110f, -110f), TextAnchor.MiddleLeft).rectTransform;
        RectTransform salir = Texto(lienzo.transform, "SALIR", 28, new Vector2(0.5f, 0.5f), new Vector2(-110f, -160f), TextAnchor.MiddleLeft).rectTransform;
        Texto(lienzo.transform, "ENTER PARA EMPEZAR", 16, new Vector2(0.5f, 0.5f), new Vector2(0f, -230f), TextAnchor.MiddleCenter);

        menu.options = new[] { jugar, salir };
        menu.cursor = Imagen(lienzo.transform, "MagicMushroom", new Vector2(0.5f, 0.5f), new Vector2(-150f, -110f), new Vector2(32f, 32f));

        EditorSceneManager.MarkSceneDirty(escena);
        EditorSceneManager.SaveScene(escena);
    }

    // Pantalla negra entre niveles: MUNDO 1-1 y Mario x 3
    private static void ConstruirVidas()
    {
        var escena = EscenaNegra("Lives");

        GameObject lienzo = Lienzo("Vidas", 5);
        LivesScreen pantalla = lienzo.AddComponent<LivesScreen>();
        pantalla.worldText = Texto(lienzo.transform, "MUNDO 1-1", 32, new Vector2(0.5f, 0.5f), new Vector2(0f, 70f), TextAnchor.MiddleCenter);
        Imagen(lienzo.transform, "Mario_Small_Idle", new Vector2(0.5f, 0.5f), new Vector2(-70f, -20f), new Vector2(48f, 48f));
        pantalla.livesText = Texto(lienzo.transform, "x  3", 32, new Vector2(0.5f, 0.5f), new Vector2(60f, -20f), TextAnchor.MiddleCenter);

        GuardarNueva(escena, "Lives");
    }

    private static void ConstruirGameOver()
    {
        var escena = EscenaNegra("GameOver");

        GameObject lienzo = Lienzo("GameOver", 5);
        lienzo.AddComponent<GameOverScreen>();
        Texto(lienzo.transform, "GAME OVER", 40, new Vector2(0.5f, 0.5f), Vector2.zero, TextAnchor.MiddleCenter);

        GuardarNueva(escena, "GameOver");
    }

    private static UnityEngine.SceneManagement.Scene EscenaNegra(string nombre)
    {
        var escena = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        var camara = new GameObject("Main Camera").AddComponent<Camera>();
        camara.tag = "MainCamera";
        camara.orthographic = true;
        camara.clearFlags = CameraClearFlags.SolidColor;
        camara.backgroundColor = Color.black;
        camara.transform.position = new Vector3(0f, 0f, -10f);

        new GameObject("Game Manager").AddComponent<GameManager>();
        PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(RutaMarcador));
        return escena;
    }

    private static void GuardarNueva(UnityEngine.SceneManagement.Scene escena, string nombre)
    {
        EditorSceneManager.SaveScene(escena, Escenas + nombre + ".unity");
    }

    private static GameObject Lienzo(string nombre, int orden)
    {
        var objeto = new GameObject(nombre, typeof(RectTransform));
        var canvas = objeto.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = orden;

        var escalador = objeto.AddComponent<CanvasScaler>();
        escalador.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        escalador.referenceResolution = new Vector2(1280f, 720f);
        escalador.matchWidthOrHeight = 1f;
        return objeto;
    }

    private static Text Texto(Transform padre, string contenido, int tamano, Vector2 ancla, Vector2 posicion, TextAnchor alineacion)
    {
        var objeto = new GameObject(contenido.Split('\n')[0], typeof(RectTransform));
        objeto.transform.SetParent(padre, false);

        var texto = objeto.AddComponent<Text>();
        texto.font = fuente;
        texto.fontSize = tamano;
        texto.text = contenido;
        texto.color = Blanco;
        texto.alignment = alineacion;
        texto.horizontalOverflow = HorizontalWrapMode.Overflow;
        texto.verticalOverflow = VerticalWrapMode.Overflow;
        texto.raycastTarget = false;

        Colocar(texto.rectTransform, ancla, posicion, new Vector2(600f, 80f), alineacion);
        return texto;
    }

    private static RectTransform Imagen(Transform padre, string sprite, Vector2 ancla, Vector2 posicion, Vector2 tamano)
    {
        var objeto = new GameObject(sprite, typeof(RectTransform));
        objeto.transform.SetParent(padre, false);

        var imagen = objeto.AddComponent<Image>();
        imagen.sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/" + sprite + ".png");
        imagen.preserveAspect = true;
        imagen.raycastTarget = false;

        Colocar(imagen.rectTransform, ancla, posicion, tamano, TextAnchor.MiddleCenter);
        return imagen.rectTransform;
    }

    private static RectTransform Panel(Transform padre, Color color, Vector2 posicion, Vector2 tamano)
    {
        var objeto = new GameObject("Panel", typeof(RectTransform));
        objeto.transform.SetParent(padre, false);

        var imagen = objeto.AddComponent<Image>();
        imagen.color = color;
        imagen.raycastTarget = false;

        Colocar(imagen.rectTransform, new Vector2(0.5f, 0.5f), posicion, tamano, TextAnchor.MiddleCenter);
        return imagen.rectTransform;
    }

    private static void Colocar(RectTransform rect, Vector2 ancla, Vector2 posicion, Vector2 tamano, TextAnchor alineacion)
    {
        rect.anchorMin = ancla;
        rect.anchorMax = ancla;
        rect.pivot = alineacion switch {
            TextAnchor.UpperLeft => new Vector2(0f, 1f),
            TextAnchor.MiddleLeft => new Vector2(0f, 0.5f),
            _ => new Vector2(0.5f, 0.5f),
        };
        rect.sizeDelta = tamano;
        rect.anchoredPosition = posicion;
    }
}
