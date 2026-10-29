using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

// Exporta el juego a la carpeta Builds (ignorada por git).
// Menú: Herramientas > Exportar para Windows / Linux
public static class ConstructorEjecutable
{
    private const string Nombre = "Super Mario - Eduardo";
    private const string Archivo = "SuperMario";

    [MenuItem("Herramientas/Exportar para Windows")]
    public static void ExportarWindows()
    {
        Exportar(BuildTarget.StandaloneWindows64, "Builds/Windows/" + Archivo + ".exe");
    }

    [MenuItem("Herramientas/Exportar para Linux")]
    public static void ExportarLinux()
    {
        Exportar(BuildTarget.StandaloneLinux64, "Builds/Linux/" + Archivo + ".x86_64");
    }

    private static void Exportar(BuildTarget plataforma, string ruta)
    {
        PlayerSettings.productName = Nombre;
        PlayerSettings.companyName = "Eduardo";
        PlayerSettings.fullScreenMode = FullScreenMode.Windowed;
        PlayerSettings.defaultScreenWidth = 1280;
        PlayerSettings.defaultScreenHeight = 720;
        PlayerSettings.resizableWindow = true;

        ConstructorNiveles.OrdenarBuild();

        var opciones = new BuildPlayerOptions {
            scenes = System.Array.ConvertAll(EditorBuildSettings.scenes, escena => escena.path),
            locationPathName = ruta,
            target = plataforma,
            options = BuildOptions.None,
        };

        BuildReport reporte = BuildPipeline.BuildPlayer(opciones);
        Debug.Log($"[Constructor] {plataforma}: {reporte.summary.result}, {reporte.summary.totalSize / 1048576f:F1} MB, {reporte.summary.totalErrors} errores");

        if (reporte.summary.result != BuildResult.Succeeded) {
            EditorApplication.Exit(1);
        }
    }
}
