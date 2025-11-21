using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuPausa : MonoBehaviour
{
    [Header("Referencias UI")]
    public GameObject objetoMenuPausa;
    public Slider sliderVolumen;

    public static bool juegoPausado = false;

    void Start()
    {
        if (objetoMenuPausa != null)
            objetoMenuPausa.SetActive(false);

        // 1. SINCRONIZAR EL SLIDER CON EL VOLUMEN ACTUAL AL INICIAR
        if (sliderVolumen != null)
        {
            sliderVolumen.value = AudioListener.volume;
            // Aseguramos que el slider llame a la función al moverse
            sliderVolumen.onValueChanged.AddListener(CambiarVolumen);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (juegoPausado) Reanudar();
            else Pausar();
        }
    }

    public void Reanudar()
    {
        objetoMenuPausa.SetActive(false);
        Time.timeScale = 1f; // El tiempo corre normal
        juegoPausado = false;
    }

    public void Pausar()
    {
        objetoMenuPausa.SetActive(true);
        Time.timeScale = 0f; // El tiempo se congela
        juegoPausado = true;
    }

    // 2. FUNCIÓN PARA IR AL MENÚ PRINCIPAL
    public void IrAlMenuPrincipal()
    {
        Time.timeScale = 1f; // IMPORTANTE: Descongelar el tiempo antes de cambiar de escena
        juegoPausado = false;
        SceneManager.LoadScene("MenuPrincipal"); // Asegúrate que el nombre sea exacto
    }

    public void CambiarVolumen(float volumen)
    {
        AudioListener.volume = volumen;
    }

    public void SalirDelJuego()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit();
    }
}