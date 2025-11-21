using UnityEngine;
using TMPro;

public class RecoleccionDeFrutas : MonoBehaviour
{
    [Header("Interfaz (UI)")]
    public TextMeshProUGUI textoContador;

    [Header("Inventario")]
    public int manzanasEnInventario = 0;
    public float vidaQueCura = 10f; // Cuánta vida recuperas al comer

    [Header("Audio")]
    public AudioClip sonidoRecoger; // Arrastra el audio de "coin" o "pickup"
    public AudioClip sonidoComer;   // Arrastra el audio de "mordisco"

    private VidaJugador scriptVida;
    private AudioSource audioSource;

    void Start()
    {
        // Obtenemos las referencias automáticamente
        scriptVida = GetComponent<VidaJugador>();
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null) // Por si acaso no tienes AudioSource
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        ActualizarTextoContador();
    }

    void Update()
    {
        // === COMER MANZANA (Tecla X) ===
        if (Input.GetKeyDown(KeyCode.X) && manzanasEnInventario > 0)
        {
            // 1. Solo comemos si la vida no está llena
            if (scriptVida.vidaActual < scriptVida.vidaMaxima)
            {
                ComerManzana();
            }
            else
            {
                Debug.Log("¡Vida llena! No es necesario comer.");
            }
        }
    }

    void ComerManzana()
    {
        manzanasEnInventario--;
        
        // Reproducir sonido
        if (sonidoComer != null) audioSource.PlayOneShot(sonidoComer);

        // Curar al jugador
        if (scriptVida != null)
        {
            scriptVida.Curar(vidaQueCura); 
        }

        ActualizarTextoContador();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Fruta"))
        {
            // Recoger manzana
            manzanasEnInventario++;
            
            // Sonido de recolección
            if (sonidoRecoger != null) audioSource.PlayOneShot(sonidoRecoger);

            Destroy(collision.gameObject);
            ActualizarTextoContador();
        }
    }

    void ActualizarTextoContador()
    {
        if (textoContador != null)
        {
            // CAMBIO AQUÍ: Solo mostramos el número
            textoContador.text = manzanasEnInventario.ToString();
        }
    }
}