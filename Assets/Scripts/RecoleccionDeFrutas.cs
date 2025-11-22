using UnityEngine;
using TMPro;

public class RecoleccionDeFrutas : MonoBehaviour
{
    [Header("Interfaz (UI)")]
    public TextMeshProUGUI textoContador;

    [Header("Inventario")]
    public int manzanasEnInventario = 0;
    public float vidaQueCura = 10f; 

    [Header("Audio")]
    public AudioClip sonidoRecoger; 
    public AudioClip sonidoComer;   

    private VidaJugador scriptVida;
    private AudioSource audioSource;

    void Start()
    {
        scriptVida = GetComponent<VidaJugador>();
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null) 
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        ActualizarTextoContador();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.X) && manzanasEnInventario > 0)
        {
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
        
        if (sonidoComer != null) audioSource.PlayOneShot(sonidoComer);

        if (scriptVida != null)
        {
            scriptVida.Curar(vidaQueCura); 
        }

        // Avisamos al Manager (pasando el inventario restante)
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.RegistrarManzanaComida(manzanasEnInventario);
        }

        ActualizarTextoContador();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Fruta"))
        {
            // === SOLUCIÓN DEFINITIVA AL DOBLE CONTEO ===
            
            // 1. Verificación de seguridad: Si la fruta ya se apagó, ignoramos este contacto.
            if (!collision.gameObject.activeSelf) return;

            // 2. APAGAR la fruta INMEDIATAMENTE. 
            // Esto la elimina del juego instantáneamente para los demás colliders.
            collision.gameObject.SetActive(false);
            
            // ===========================================

            manzanasEnInventario++;
            
            if (sonidoRecoger != null) audioSource.PlayOneShot(sonidoRecoger);

            // Destruimos el objeto (Unity lo borrará de la memoria al final del frame)
            Destroy(collision.gameObject);

            if (LevelManager.Instance != null)
            {
                LevelManager.Instance.RegistrarManzanaRecolectada();
            }

            ActualizarTextoContador();
        }
    }

    void ActualizarTextoContador()
    {
        if (textoContador != null)
        {
            textoContador.text = manzanasEnInventario.ToString();
        }
    }
}