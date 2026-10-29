using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

// Construye los niveles nuevos a partir de las piezas del 1-1.
// Menú: Herramientas > Construir nivel 1-2
public static partial class ConstructorNiveles
{
    private const string Prefabs = "Assets/Prefabs/";
    private const string Escenas = "Assets/Scenes/";

    // Suelo del subterráneo: la fila de arriba es y = -13, se pisa en y = -12.5
    private const int Suelo = -13;
    private const int Techo = -3;

    private static Transform padre;

    [MenuItem("Herramientas/Construir nivel 1-2")]
    public static void ConstruirSubterraneo()
    {
        Escena nivel = PrepararEscena("1-2");

        Transform jugador = nivel.jugador;
        jugador.position = new Vector3(2f, Suelo + 1f, 0f);

        Camera camara = nivel.camara;
        camara.transform.position = new Vector3(10.5f, -9.5f, camara.transform.position.z);

        Transform barrera = nivel.barrera;
        barrera.position = new Vector3(100f, -20f, 0f);
        barrera.GetComponent<BoxCollider2D>().size = new Vector2(400f, 2f);

        padre = new GameObject("Subterráneo").transform;

        // Muro de entrada y techo
        Rect("UndergroundBlock", -20, Suelo - 3, 19, Techo - Suelo + 4);
        Rect("UndergroundBrick", -1, Techo, 171, 1);

        // Suelo con tres fosos
        Rect("UndergroundBlock", -1, Suelo - 3, 41, 4);
        Rect("UndergroundBlock", 43, Suelo - 3, 37, 4);
        Rect("UndergroundBlock", 83, Suelo - 3, 38, 4);
        Rect("UndergroundBlock", 124, Suelo - 3, 46, 4);

        // Tramo 1: bloques sorpresa y los primeros goombas
        Sorpresa(8, Suelo + 4, "BlockCoin");
        Ladrillo(9, Suelo + 4);
        Sorpresa(10, Suelo + 4, "MagicMushroom");
        Ladrillo(11, Suelo + 4);
        Sorpresa(12, Suelo + 4, "BlockCoin");
        Enemigo("Goomba", 18, Suelo + 1);
        Enemigo("Goomba", 20, Suelo + 1);

        // Tramo 2: escalón y plataforma de ladrillos con monedas encima
        Rect("UndergroundBlock", 24, Suelo + 1, 1, 1);
        Rect("UndergroundBlock", 25, Suelo + 1, 1, 2);
        for (int x = 29; x <= 36; x++) Ladrillo(x, Suelo + 4);
        for (int x = 30; x <= 35; x++) Moneda(x, Suelo + 6);
        Enemigo("Goomba", 33, Suelo + 1);

        // Monedas en arco sobre el primer foso
        Moneda(40, Suelo + 2);
        Moneda(41, Suelo + 3);
        Moneda(42, Suelo + 2);

        // Tramo 3: tuberías de alturas crecientes
        Enemigo("Koopa", 49, Suelo + 1);
        Tuberia(54.5f, 2);
        Enemigo("Goomba", 58, Suelo + 1);
        Tuberia(62.5f, 3);
        Enemigo("Goomba", 66, Suelo + 1);
        Tuberia(70.5f, 4);
        for (int y = Suelo + 6; y <= Suelo + 8; y++) Moneda(70, y);

        // Columna colgante de ladrillos antes del segundo foso
        Rect("UndergroundBrick", 76, Suelo + 5, 2, Techo - Suelo - 5);

        // Tramo 4: plataformas en dos alturas
        for (int x = 86; x <= 90; x++) Ladrillo(x, Suelo + 4);
        for (int x = 93; x <= 98; x++)
        {
            if (x == 96) Sorpresa(x, Suelo + 7, "Starman");
            else Ladrillo(x, Suelo + 7);
        }
        for (int x = 94; x <= 97; x++) Moneda(x, Suelo + 9);
        Enemigo("Goomba", 100, Suelo + 1);
        Enemigo("Goomba", 102, Suelo + 1);
        Enemigo("Goomba", 104, Suelo + 1);
        Enemigo("Koopa", 112, Suelo + 1);
        Sorpresa(108, Suelo + 4, "1upMushroom", oculto: true);

        // Plataforma flotante sobre el tercer foso
        Ladrillo(122, Suelo + 3);

        // Tramo 5: escalera de bloques
        for (int i = 0; i < 4; i++) Rect("UndergroundBlock", 130 + i, Suelo + 1, 1, i + 1);
        Rect("UndergroundBlock", 134, Suelo + 1, 2, 4);
        Enemigo("Goomba", 140, Suelo + 1);
        Enemigo("Goomba", 142, Suelo + 1);

        // Salida: tubería lateral que sube a la superficie
        Rect("UndergroundBlock", 165, Suelo + 1, 5, Techo - Suelo - 1);
        GameObject salidaLateral = Instanciar("PipeConnection", new Vector3(160.5f, Suelo + 1.5f, 0f));

        // Superficie con la bandera y el castillo
        padre = new GameObject("Superficie").transform;
        Rect("Ground", 140, 0, 60, 2);
        Instanciar("Pipe", new Vector3(163.5f, 3f, 0f));
        Transform llegada = new GameObject("Llegada").transform;
        llegada.SetParent(padre);
        llegada.position = new Vector3(163.5f, 3f, 0f);
        Asignar(salidaLateral.GetComponentInChildren<Pipe>(), "connection", llegada);

        for (int i = 0; i < 4; i++) Rect("HardBlock", 170 + i, 2, 1, i + 1);
        Rect("HardBlock", 174, 2, 1, 4);

        GameObject bandera = Instanciar("Flag Pole", new Vector3(180f, 2f, 0f));
        GameObject castillo = Instanciar("Castle", new Vector3(186f, 2f, 0f));
        FlagPole poste = bandera.GetComponent<FlagPole>();
        Asignar(poste, "castle", castillo.transform);
        Asignar(poste, "nextWorld", 1);
        Asignar(poste, "nextStage", 3);

        GuardarEscena(nivel);

        // El 1-1 ahora lleva al 1-2
        EditorSceneManager.OpenScene(Escenas + "1-1.unity");
        FlagPole poste11 = Object.FindFirstObjectByType<FlagPole>();
        Asignar(poste11, "nextStage", 2);
        EditorSceneManager.SaveScene(poste11.gameObject.scene);

        ConstructorPantallas.AgregarMarcador(Escenas + "1-2.unity");
        OrdenarBuild();
        Debug.Log("[Constructor] Nivel 1-2 listo");
    }

    // El suelo del inicio llega más a la izquierda para que no quede un hueco
    // cuando la ventana es más ancha que 16:9
    [MenuItem("Herramientas/Alargar suelo del 1-1 y del menú")]
    public static void AlargarSuelo()
    {
        foreach (string nombre in new[] { "1-1", "Menu" })
        {
            var escena = EditorSceneManager.OpenScene(Escenas + nombre + ".unity");
            SpriteRenderer primero = null;

            foreach (var sprite in Object.FindObjectsByType<SpriteRenderer>(FindObjectsSortMode.None))
            {
                if (sprite.gameObject.name != "Ground") continue;
                if (primero == null || sprite.transform.position.x < primero.transform.position.x) primero = sprite;
            }

            const float borde = -20.5f;
            float derecha = primero.transform.position.x + primero.size.x / 2f;
            if (primero.transform.position.x - primero.size.x / 2f <= borde) continue;

            var tamano = new Vector2(derecha - borde, primero.size.y);
            Vector3 posicion = primero.transform.position;
            posicion.x = (derecha + borde) / 2f;
            primero.transform.position = posicion;
            primero.size = tamano;
            primero.GetComponent<BoxCollider2D>().size = tamano;

            EditorSceneManager.MarkSceneDirty(escena);
            EditorSceneManager.SaveScene(escena);
        }
        Debug.Log("[Constructor] Suelo alargado");
    }

    private struct Escena
    {
        public UnityEngine.SceneManagement.Scene escena;
        public Transform jugador;
        public Camera camara;
        public Transform barrera;
    }

    // Copia el 1-1, deja solo la cámara, el GameManager, la barrera y a Mario
    private static Escena PrepararEscena(string nombre)
    {
        string ruta = Escenas + nombre + ".unity";
        // se copia el contenido encima para que la escena conserve su GUID
        System.IO.File.Copy(Escenas + "1-1.unity", ruta, true);
        AssetDatabase.ImportAsset(ruta, ImportAssetOptions.ForceUpdate);

        var escena = EditorSceneManager.OpenScene(ruta);
        Transform jugador = Object.FindFirstObjectByType<Player>().transform;
        Camera camara = Object.FindFirstObjectByType<Camera>();
        Transform barrera = Object.FindFirstObjectByType<DeathBarrier>().transform;
        Transform gestor = Object.FindFirstObjectByType<GameManager>().transform;

        foreach (Transform conservar in new[] { jugador, camara.transform, barrera, gestor }) {
            if (conservar.parent != null) conservar.SetParent(null);
        }

        foreach (GameObject raiz in escena.GetRootGameObjects())
        {
            if (raiz.transform == jugador || raiz.transform == camara.transform) continue;
            if (raiz.transform == barrera || raiz.transform == gestor) continue;
            Object.DestroyImmediate(raiz);
        }

        return new Escena {
            escena = escena,
            jugador = jugador,
            camara = camara,
            barrera = barrera,
        };
    }

    private static void GuardarEscena(Escena nivel)
    {
        EditorSceneManager.MarkSceneDirty(nivel.escena);
        EditorSceneManager.SaveScene(nivel.escena);
    }

    // El menú va primero para que el ejecutable arranque en él
    public static void OrdenarBuild()
    {
        string[] orden = { "Menu", "Lives", "GameOver", "1-1", "1-2", "1-3", "1-4" };
        var lista = new System.Collections.Generic.List<EditorBuildSettingsScene>();
        foreach (string nombre in orden)
        {
            string ruta = Escenas + nombre + ".unity";
            if (System.IO.File.Exists(ruta)) lista.Add(new EditorBuildSettingsScene(ruta, true));
        }
        EditorBuildSettings.scenes = lista.ToArray();
    }

    private static GameObject Instanciar(string prefab, Vector3 posicion)
    {
        var original = AssetDatabase.LoadAssetAtPath<GameObject>(Prefabs + prefab + ".prefab");
        var objeto = (GameObject)PrefabUtility.InstantiatePrefab(original);
        objeto.transform.SetParent(padre);
        objeto.transform.position = posicion;
        return objeto;
    }

    // Bloque de celdas: (x, y) es la celda de abajo a la izquierda
    private static GameObject Rect(string prefab, int x, int y, int ancho, int alto)
    {
        var centro = new Vector3(x + (ancho - 1) / 2f, y + (alto - 1) / 2f, 0f);
        GameObject objeto = Instanciar(prefab, centro);
        var tamano = new Vector2(ancho, alto);
        objeto.GetComponent<SpriteRenderer>().size = tamano;
        objeto.GetComponent<BoxCollider2D>().size = tamano;
        return objeto;
    }

    private static void Ladrillo(int x, int y)
    {
        GameObject ladrillo = Instanciar("Brick", new Vector3(x, y, 0f));
        ladrillo.GetComponent<SpriteRenderer>().sprite = Sprite("UndergroundBrick");
    }

    private static void Sorpresa(int x, int y, string item, bool oculto = false)
    {
        GameObject bloque = Instanciar("MysteryBlock", new Vector3(x, y, 0f));
        var golpe = bloque.GetComponent<BlockHit>();
        var objeto = AssetDatabase.LoadAssetAtPath<GameObject>(Prefabs + item + ".prefab");
        Asignar(golpe, "item", objeto);
        if (oculto) bloque.GetComponent<SpriteRenderer>().enabled = false;
    }

    private static void Moneda(int x, int y)
    {
        GameObject moneda = Instanciar("Coin", new Vector3(x, y, 0f));
        moneda.GetComponent<SpriteRenderer>().sprite = Sprite("Coin_Underground");
    }

    private static void Enemigo(string prefab, int x, int y)
    {
        Instanciar(prefab, new Vector3(x, y, 0f));
    }

    // Tubería vertical de "alto" celdas sobre el suelo del subterráneo
    private static void Tuberia(float x, int alto)
    {
        Instanciar("Pipe", new Vector3(x, Suelo + alto, 0f));
    }

    private static Sprite Sprite(string nombre)
    {
        return AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/" + nombre + ".png");
    }

    private static void Asignar(Object componente, string campo, object valor)
    {
        var serializado = new SerializedObject(componente);
        SerializedProperty propiedad = serializado.FindProperty(campo);
        if (valor is int entero) propiedad.intValue = entero;
        else propiedad.objectReferenceValue = (Object)valor;
        serializado.ApplyModifiedProperties();
    }
}
