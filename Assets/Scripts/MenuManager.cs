using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    // Esta función cargará el juego
    public void JugarJuego()
    {
        // Asegúrate de que el nombre aquí sea EXACTO al de tu escena de juego.
        // He visto en tus archivos que se llama "Crimson Harvest" o "SC Demo Scene - Village Props".
        // Usaré "Crimson Harvest" como ejemplo:
        SceneManager.LoadScene("Crimson Harvest"); 
    }

    // Esta función cerrará el juego
    public void SalirJuego()
    {
        Debug.Log("¡Saliendo del juego!");
        Application.Quit();
    }
}