using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance; 

    [System.Serializable]
    public class Nivel
    {
        public string nombreNivel = "Nivel 1";
        public int manzanasNecesarias; 
        public GameObject[] murosADesaparecer; 
    }

    [Header("Configuración de Niveles")]
    public Nivel[] niveles; 
    public int nivelActualIndex = 0;

    [Header("Estado del Nivel Actual")]
    public int manzanasRecolectadasNivel = 0;
    public int manzanasComidasNivel = 0;

    [Header("UI Global")]
    public GameObject panelNivelCompletado; 
    public TextMeshProUGUI textoTituloNivel;
    public TextMeshProUGUI textoEstadisticas;
    public GameObject panelGanarJuego; 
    public GameObject panelGameOver; 

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if(panelNivelCompletado != null) panelNivelCompletado.SetActive(false);
        if(panelGanarJuego != null) panelGanarJuego.SetActive(false);
        
        if(panelGameOver == null)
        {
            var vida = FindFirstObjectByType<VidaJugador>();
            if(vida != null) panelGameOver = vida.panelGameOver;
        }
    }

    // --- LÓGICA DE MANZANAS ---

    public void RegistrarManzanaRecolectada()
    {
        manzanasRecolectadasNivel++;
        VerificarVictoriaNivel();
    }

    // CAMBIO AQUÍ: Pedimos el inventario actual para calcular el límite real
    public void RegistrarManzanaComida(int inventarioActual)
    {
        manzanasComidasNivel++;

        // El total de manzanas que has tenido disponibles es:
        // Lo que te queda en la bolsa + Lo que ya te comiste en este nivel
        int totalDisponibles = inventarioActual + manzanasComidasNivel;
        
        // El límite es la mitad de ese total
        float limite = totalDisponibles / 2.0f;

        if (manzanasComidasNivel > limite)
        {
            Debug.Log($"¡Game Over! Comiste {manzanasComidasNivel}. El límite era {limite} (Total disponibles: {totalDisponibles})");
            PerderJuego();
        }
    }

    // --- VERIFICACIÓN DE VICTORIA ---

    void VerificarVictoriaNivel()
    {
        if (nivelActualIndex >= niveles.Length) return;

        Nivel nivel = niveles[nivelActualIndex];

        if (manzanasRecolectadasNivel >= nivel.manzanasNecesarias)
        {
            NivelCompletado();
        }
    }

    void NivelCompletado()
    {
        Time.timeScale = 0f; 

        if (nivelActualIndex == niveles.Length - 1)
        {
            if(panelGanarJuego != null) panelGanarJuego.SetActive(true);
        }
        else
        {
            if(panelNivelCompletado != null)
            {
                panelNivelCompletado.SetActive(true);
                textoTituloNivel.text = "¡" + niveles[nivelActualIndex].nombreNivel + " Completado!";
                textoEstadisticas.text = $"Recolectaste {manzanasRecolectadasNivel} manzanas (Comiste {manzanasComidasNivel})";
            }
        }
    }

    void PerderJuego()
    {
        Time.timeScale = 0f; 
        if (panelGameOver != null)
        {
            panelGameOver.SetActive(true);
        }
    }

    // --- BOTONES UI ---

    public void BotonContinuar()
    {
        if (nivelActualIndex < niveles.Length)
        {
            Nivel nivel = niveles[nivelActualIndex];
            foreach(GameObject muro in nivel.murosADesaparecer)
            {
                if(muro != null) muro.SetActive(false);
            }
        }

        manzanasRecolectadasNivel = 0;
        manzanasComidasNivel = 0; // Reiniciamos el contador de gula para el nuevo nivel
        nivelActualIndex++;

        if(panelNivelCompletado != null) panelNivelCompletado.SetActive(false);
        Time.timeScale = 1f;
    }

    public void BotonSalirMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MenuPrincipal"); 
    }
    
    public void BotonReiniciar()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}