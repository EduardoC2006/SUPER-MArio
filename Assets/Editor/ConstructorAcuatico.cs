using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

// Nivel 1-3 acuático. Usa la misma base que el 1-2 (PrepararEscena, Rect...)
// Menú: Herramientas > Construir nivel 1-3
public static partial class ConstructorNiveles
{
    // El agua llega hasta la fila 10, la fila 11 son las olas
    private const int Superficie = 11;
    private const int FinAgua = 262;

    private static readonly string[] SpritesMarinos = {
        "Agua", "Ola", "FondoMarino", "Coral", "Alga1", "Alga2", "Pez1", "Pez2", "Medusa1", "Medusa2",
    };

    [MenuItem("Herramientas/Construir nivel 1-3")]
    public static void ConstruirAcuatico()
    {
        ImportarSpritesMarinos();
        CrearPrefabsMarinos();

        Escena nivel = PrepararEscena("1-3");
        nivel.jugador.position = new Vector3(2f, 2f, 0f);
        nivel.camara.transform.position = new Vector3(10.5f, 6.5f, nivel.camara.transform.position.z);
        nivel.barrera.position = new Vector3(140f, -2f, 0f);
        nivel.barrera.GetComponent<BoxCollider2D>().size = new Vector2(500f, 2f);

        padre = new GameObject("Agua").transform;
        Fondo("Agua", -20, 0, FinAgua + 21, Superficie);
        Fondo("Ola", -20, Superficie, FinAgua + 21, 1);
        ZonaDeAgua();

        // techo invisible: Mario no puede salir del agua nadando
        var techo = new GameObject("Techo del agua").AddComponent<BoxCollider2D>();
        techo.transform.SetParent(padre);
        techo.transform.position = new Vector3((FinAgua - 20) / 2f, Superficie + 1.5f, 0f);
        techo.size = new Vector2(FinAgua + 21, 1f);

        padre = new GameObject("Fondo marino").transform;
        Rect("FondoMarino", -20, 0, 61, 2);
        Rect("FondoMarino", 45, 0, 51, 2);
        Rect("FondoMarino", 100, 0, 51, 2);
        Rect("FondoMarino", 154, 0, FinAgua - 154 + 1, 2);
        Rect("FondoMarino", -20, 2, 18, Superficie - 1);

        padre = new GameObject("Arrecife").transform;

        // Tramo 1: primeras columnas y un bloque con champiñón
        Rect("Coral", 10, 2, 2, 3);
        Sorpresa(15, 6, "MagicMushroom");
        Rect("Coral", 18, 7, 7, 1);
        for (int x = 19; x <= 23; x++) MonedaNormal(x, 5);
        Rect("Coral", 30, 2, 2, 6);

        // Monedas sobre el primer foso
        for (int x = 41; x <= 44; x++) MonedaNormal(x, 4);

        // Tramo 2: plataformas en escalera
        Rect("Coral", 50, 5, 7, 1);
        Rect("Coral", 58, 9, 7, 1);
        for (int x = 59; x <= 63; x++) MonedaNormal(x, 7);

        // Tramo 3: pasillo angosto entre corales de arriba y de abajo
        Rect("Coral", 70, 2, 16, 3);
        Rect("Coral", 70, 8, 16, Superficie - 8);
        for (int x = 72; x <= 84; x += 2) MonedaNormal(x, 6);

        // Tramo 4: escalera y monedas sobre el segundo foso
        for (int i = 0; i < 4; i++) Rect("FondoMarino", 89 + i, 2, 1, i + 1);
        for (int x = 96; x <= 99; x++) MonedaNormal(x, 7);

        // Tramo 5: columnas alternadas arriba y abajo
        Rect("Coral", 105, 2, 2, 5);
        Rect("Coral", 116, 6, 7, 1);
        Sorpresa(119, 9, "Starman");
        Rect("Coral", 128, 7, 3, Superficie - 7);
        Rect("Coral", 136, 2, 3, 4);
        Rect("Coral", 144, 8, 3, Superficie - 8);

        // Tramo 6: tercer foso y bosque de columnas
        for (int x = 151; x <= 153; x++) MonedaNormal(x, 5);
        for (int x = 160; x <= 184; x += 6)
        {
            bool abajo = (x / 6) % 2 == 0;
            if (abajo) Rect("Coral", x, 2, 2, 5);
            else Rect("Coral", x, 6, 2, Superficie - 6);
        }
        Sorpresa(176, 4, "1upMushroom");

        // Tramo 7: pasillo final con monedas y la tubería de salida
        Rect("Coral", 195, 2, 20, 2);
        Rect("Coral", 195, 9, 20, Superficie - 9);
        for (int x = 197; x <= 213; x += 2) MonedaNormal(x, 6);
        Rect("Coral", 225, 2, 3, 3);
        Rect("Coral", 235, 7, 3, Superficie - 7);

        padre = new GameObject("Algas").transform;
        foreach (int x in new[] { 4, 13, 26, 36, 48, 67, 87, 103, 112, 125, 133, 141, 157, 190, 220, 231, 242 }) {
            Instanciar("Alga", new Vector3(x, 2.5f, 0f));
        }

        padre = new GameObject("Enemigos").transform;
        int[,] peces = {
            { 35, 6 }, { 48, 9 }, { 62, 3 }, { 79, 6 }, { 88, 9 }, { 102, 4 }, { 112, 8 }, { 125, 5 },
            { 138, 9 }, { 147, 4 }, { 165, 8 }, { 172, 3 }, { 190, 7 }, { 205, 6 }, { 218, 9 }, { 228, 6 }, { 240, 3 },
        };
        for (int i = 0; i < peces.GetLength(0); i++) Instanciar("Pez", new Vector3(peces[i, 0], peces[i, 1], 0f));
        foreach (Vector2 medusa in new[] { new Vector2(27, 9), new Vector2(66, 4), new Vector2(110, 9), new Vector2(158, 8), new Vector2(200, 7), new Vector2(233, 9) }) {
            Instanciar("Medusa", medusa);
        }

        // Salida: tubería lateral al fondo y pared al final del agua
        padre = new GameObject("Salida").transform;
        Rect("FondoMarino", 255, 2, FinAgua - 255 + 1, Superficie);
        GameObject salidaLateral = Instanciar("PipeConnection", new Vector3(250.5f, 2.5f, 0f));

        // Tierra firme con la bandera, lejos del agua
        Rect("Ground", 270, 0, 70, 2);
        Rect("HardBlock", 285, 2, 1, 12);
        Instanciar("Pipe", new Vector3(300.5f, 3f, 0f));
        Transform llegada = new GameObject("Llegada").transform;
        llegada.SetParent(padre);
        llegada.position = new Vector3(300.5f, 3f, 0f);
        Asignar(salidaLateral.GetComponentInChildren<Pipe>(), "connection", llegada);

        for (int i = 0; i < 4; i++) Rect("HardBlock", 306 + i, 2, 1, i + 1);
        Rect("HardBlock", 310, 2, 1, 4);
        GameObject bandera = Instanciar("Flag Pole", new Vector3(316f, 2f, 0f));
        GameObject castillo = Instanciar("Castle", new Vector3(322f, 2f, 0f));
        FlagPole poste = bandera.GetComponent<FlagPole>();
        Asignar(poste, "castle", castillo.transform);
        Asignar(poste, "nextWorld", 1);
        Asignar(poste, "nextStage", 1);

        GuardarEscena(nivel);
        ConstructorPantallas.AgregarMarcador(Escenas + "1-3.unity");

        // El 1-2 ahora lleva al 1-3 (sin regenerarlo, para no perder ajustes a mano)
        EditorSceneManager.OpenScene(Escenas + "1-2.unity");
        FlagPole poste12 = Object.FindFirstObjectByType<FlagPole>();
        Asignar(poste12, "nextStage", 3);
        EditorSceneManager.SaveScene(poste12.gameObject.scene);

        OrdenarBuild();
        Debug.Log("[Constructor] Nivel 1-3 listo");
    }

    private static void ImportarSpritesMarinos()
    {
        foreach (string nombre in SpritesMarinos)
        {
            var importador = (TextureImporter)AssetImporter.GetAtPath("Assets/Sprites/" + nombre + ".png");
            importador.textureType = TextureImporterType.Sprite;
            importador.spriteImportMode = SpriteImportMode.Single;
            importador.spritePixelsPerUnit = 16;
            importador.filterMode = FilterMode.Point;
            importador.textureCompression = TextureImporterCompression.Uncompressed;
            importador.alphaIsTransparency = true;

            var ajustes = new TextureImporterSettings();
            importador.ReadTextureSettings(ajustes);
            ajustes.spriteMeshType = SpriteMeshType.FullRect;
            importador.SetTextureSettings(ajustes);
            importador.SaveAndReimport();
        }
    }

    private static void CrearPrefabsMarinos()
    {
        // bloques: copia del bloque subterráneo con otro sprite
        foreach (string nombre in new[] { "FondoMarino", "Coral" })
        {
            var original = AssetDatabase.LoadAssetAtPath<GameObject>(Prefabs + "UndergroundBlock.prefab");
            var bloque = (GameObject)PrefabUtility.InstantiatePrefab(original);
            PrefabUtility.UnpackPrefabInstance(bloque, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
            bloque.name = nombre;
            bloque.GetComponent<SpriteRenderer>().sprite = Sprite(nombre);
            PrefabUtility.SaveAsPrefabAsset(bloque, Prefabs + nombre + ".prefab");
            Object.DestroyImmediate(bloque);
        }

        var alga = new GameObject("Alga");
        var algaSprite = alga.AddComponent<SpriteRenderer>();
        algaSprite.sprite = Sprite("Alga1");
        algaSprite.sortingOrder = -5;
        var algaAnimacion = alga.AddComponent<AnimatedSprite>();
        algaAnimacion.sprites = new[] { Sprite("Alga1"), Sprite("Alga2") };
        algaAnimacion.framerate = 0.5f;
        PrefabUtility.SaveAsPrefabAsset(alga, Prefabs + "Alga.prefab");
        Object.DestroyImmediate(alga);

        CrearEnemigoMarino("Pez", SeaEnemy.Kind.Fish, 2.5f, new Vector2(0.9f, 0.6f));
        CrearEnemigoMarino("Medusa", SeaEnemy.Kind.Jellyfish, 2f, new Vector2(0.8f, 1.1f));
    }

    private static void CrearEnemigoMarino(string nombre, SeaEnemy.Kind tipo, float velocidad, Vector2 tamano)
    {
        var enemigo = new GameObject(nombre);
        enemigo.layer = LayerMask.NameToLayer("Enemy");

        var sprite = enemigo.AddComponent<SpriteRenderer>();
        sprite.sprite = Sprite(nombre + "1");
        sprite.sortingOrder = 2;

        var animacion = enemigo.AddComponent<AnimatedSprite>();
        animacion.sprites = new[] { Sprite(nombre + "1"), Sprite(nombre + "2") };
        animacion.framerate = 0.25f;

        var cuerpo = enemigo.AddComponent<Rigidbody2D>();
        cuerpo.bodyType = RigidbodyType2D.Kinematic;

        var colision = enemigo.AddComponent<BoxCollider2D>();
        colision.isTrigger = true;
        colision.size = tamano;

        var muerte = enemigo.AddComponent<DeathAnimation>();
        muerte.spriteRenderer = sprite;
        muerte.enabled = false;

        var marino = enemigo.AddComponent<SeaEnemy>();
        marino.kind = tipo;
        marino.speed = velocidad;

        PrefabUtility.SaveAsPrefabAsset(enemigo, Prefabs + nombre + ".prefab");
        Object.DestroyImmediate(enemigo);
    }

    // Sprite de fondo sin colisión (el agua y las olas)
    private static void Fondo(string sprite, int x, int y, int ancho, int alto)
    {
        var objeto = new GameObject(sprite);
        objeto.transform.SetParent(padre);
        objeto.transform.position = new Vector3(x + (ancho - 1) / 2f, y + (alto - 1) / 2f, 0f);

        var renderer = objeto.AddComponent<SpriteRenderer>();
        renderer.sprite = Sprite(sprite);
        renderer.drawMode = SpriteDrawMode.Tiled;
        renderer.size = new Vector2(ancho, alto);
        renderer.sortingOrder = -10;
    }

    private static void ZonaDeAgua()
    {
        var zona = new GameObject("Zona de agua");
        zona.transform.SetParent(padre);
        zona.layer = 2; // Ignore Raycast: no cuenta como suelo
        zona.transform.position = new Vector3((FinAgua - 20) / 2f, (Superficie - 0.5f) / 2f, 0f);

        var area = zona.AddComponent<BoxCollider2D>();
        area.isTrigger = true;
        area.size = new Vector2(FinAgua + 21, Superficie + 1.5f);
        zona.AddComponent<Water>();
    }

    private static void MonedaNormal(int x, int y)
    {
        Instanciar("Coin", new Vector3(x, y, 0f));
    }
}
