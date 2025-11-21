using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Fuente de Audio")]
    [SerializeField] private AudioSource musicSource;

    [Header("Clips de Música")]
    public AudioClip menuMusic; // Música para el Menú
    public AudioClip gameMusic; // Música para Crimson Harvest(1-3)

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Aseguramos que se repita en bucle
        if (musicSource != null) musicSource.loop = true;
        
        CheckSceneMusic(SceneManager.GetActiveScene().name);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        CheckSceneMusic(scene.name);
    }

    // AQUÍ ESTÁ EL CAMBIO IMPORTANTE
    private void CheckSceneMusic(string sceneName)
    {
        // 1. Si estamos en el menú
        if (sceneName == "MenuPrincipal") 
        {
            PlayMusic(menuMusic);
        }
        // 2. Si estamos en la escena del juego (niveles 1-3 acumulados)
        else if (sceneName == "Crimson Harvest(1-3)") 
        {
            PlayMusic(gameMusic);
        }
    }

    public void PlayMusic(AudioClip clip)
    {
        if (clip == null || musicSource.clip == clip) return;

        musicSource.Stop();
        musicSource.clip = clip;
        musicSource.Play();
    }
}