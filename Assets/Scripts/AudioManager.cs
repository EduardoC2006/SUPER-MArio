using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Plays the music of each scene and the sound effects. It creates itself
// when the game starts, so no scene needs to include it.
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    private AudioSource music;
    private AudioSource effects;
    private readonly Dictionary<string, AudioClip> clips = new Dictionary<string, AudioClip>();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Create()
    {
        GameObject manager = new GameObject("Audio Manager");
        manager.AddComponent<AudioManager>();
        DontDestroyOnLoad(manager);
    }

    private void Awake()
    {
        Instance = this;

        music = gameObject.AddComponent<AudioSource>();
        music.loop = true;
        music.volume = 0.5f;

        effects = gameObject.AddComponent<AudioSource>();
        effects.volume = 0.8f;

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayMusic(MusicFor(scene.name));
    }

    private static string MusicFor(string scene)
    {
        switch (scene)
        {
            case "Menu": return "Menu";
            case "1-1": return "Superficie";
            case "1-2": return "Subterraneo";
            case "1-3": return "Acuatico";
            case "1-4": return "Castillo";
            default: return null;
        }
    }

    // Levels with a surface and an underground part switch music with the camera
    public static void SetUnderground(bool underground)
    {
        if (Instance == null) {
            return;
        }

        string current = Instance.music.clip != null ? Instance.music.clip.name : null;

        if (current == "Superficie" || current == "Subterraneo") {
            Instance.PlayMusic(underground ? "Subterraneo" : "Superficie");
        }
    }

    public static void Play(string effect)
    {
        if (Instance != null) {
            Instance.effects.PlayOneShot(Instance.Load("Efectos/" + effect));
        }
    }

    public static void StopMusic()
    {
        if (Instance != null) {
            Instance.music.Stop();
        }
    }

    private void PlayMusic(string name)
    {
        AudioClip clip = name != null ? Load("Musica/" + name) : null;

        if (clip == music.clip && music.isPlaying) {
            return;
        }

        music.clip = clip;

        if (clip != null) {
            music.Play();
        } else {
            music.Stop();
        }
    }

    private AudioClip Load(string path)
    {
        if (!clips.TryGetValue(path, out AudioClip clip))
        {
            clip = Resources.Load<AudioClip>("Audio/" + path);
            clips[path] = clip;
        }

        return clip;
    }

}
