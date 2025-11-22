using UnityEngine;
using TMPro; 
using UnityEngine.SceneManagement; 
using System.Collections;

public class VidaJugador : MonoBehaviour
{
    [Header("Valores")]
    public float vidaMaxima = 100f;
    public float vidaActual;

    [Header("Configuración UI")]
    public TextMeshProUGUI textoVida; 
    public GameObject panelGameOver;  

    private Animator animator;
    private Rigidbody2D rb; // Referencia para bloquear físicas al morir
    private SpriteRenderer spriteRenderer; // Para el parpadeo
    
    private bool estaMuerto = false;
    public bool esInvulnerable = false; // Nueva variable para la inmunidad

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        vidaActual = vidaMaxima;
        ActualizarUI();

        if (panelGameOver != null) panelGameOver.SetActive(false);
    }

    public void RecibirDaño(float daño)
    {
        // Si ya está muerto o es invulnerable, no hacemos nada
        if (estaMuerto || esInvulnerable) return;

        vidaActual -= daño;
        if (vidaActual < 0) vidaActual = 0;

        ActualizarUI();

        if (animator != null) animator.SetTrigger("Hurt");

        if (vidaActual <= 0)
        {
            Morir();
        }
        else
        {
            // Si sigue vivo, activamos la inmunidad de 2 segundos
            StartCoroutine(RutinaInmunidad(2f));
        }
    }

    // Corutina para la inmunidad y el parpadeo
    IEnumerator RutinaInmunidad(float tiempo)
    {
        esInvulnerable = true;
        
        // Hacemos parpadear al jugador
        float temporizador = 0;
        while (temporizador < tiempo)
        {
            if(spriteRenderer != null) spriteRenderer.enabled = !spriteRenderer.enabled;
            yield return new WaitForSeconds(0.1f); // Velocidad del parpadeo
            temporizador += 0.1f;
        }
        
        if(spriteRenderer != null) spriteRenderer.enabled = true; // Aseguramos que sea visible al final
        esInvulnerable = false;
    }

    void ActualizarUI()
    {
        if (textoVida != null) textoVida.text = vidaActual.ToString();
    }

    void Morir()
    {
        estaMuerto = true;
        if (animator != null) animator.SetTrigger("Muerte");

        // Desactivar movimiento
        var movimiento = GetComponent<MovimientosDelJugador>();
        if (movimiento != null) movimiento.enabled = false;
        
        // Detener al personaje por completo para que no deslice muerto
        if (rb != null) rb.linearVelocity = Vector2.zero;

        StartCoroutine(SecuenciaMuerte());
    }

    IEnumerator SecuenciaMuerte()
    {
        yield return new WaitForSeconds(1.5f);
        if (panelGameOver != null) panelGameOver.SetActive(true);
        
        yield return new WaitForSeconds(3f);

        // CAMBIO: Cargar la escena actual en vez del menú
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); 
    }

    public void Curar(float cantidad)
    {
        if (estaMuerto) return;
        vidaActual += cantidad;
        if (vidaActual > vidaMaxima) vidaActual = vidaMaxima;
        ActualizarUI(); 
    }
}