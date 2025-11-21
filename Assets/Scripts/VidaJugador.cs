using UnityEngine;
using TMPro; // Librería para textos pro
using UnityEngine.SceneManagement; // Para cambiar de escena
using System.Collections;

public class VidaJugador : MonoBehaviour
{
    [Header("Valores")]
    public float vidaMaxima = 100f;
    public float vidaActual;

    [Header("Configuración UI")]
    public TextMeshProUGUI textoVida; // Arrastra aquí tu objeto 'TextoVida'
    public GameObject panelGameOver;  // Arrastra aquí tu 'PanelGameOver'

    private Animator animator;
    private bool estaMuerto = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        vidaActual = vidaMaxima;
        
        // Actualizar el número al iniciar
        ActualizarUI();

        // Asegurarnos de que el Game Over esté apagado al inicio
        if (panelGameOver != null) panelGameOver.SetActive(false);
    }

    // Función para recibir daño (llámala desde las trampas o enemigos)
    public void RecibirDaño(float daño)
    {
        if (estaMuerto) return;

        vidaActual -= daño;

        // Que no baje de 0
        if (vidaActual < 0) vidaActual = 0;

        ActualizarUI();

        // Animación de golpe (si tienes)
        if (animator != null) animator.SetTrigger("Hurt");

        if (vidaActual <= 0)
        {
            Morir();
        }
    }

    void ActualizarUI()
    {
        if (textoVida != null)
        {
            textoVida.text = vidaActual.ToString(); // Muestra solo el número
        }
    }

    void Morir()
    {
        estaMuerto = true;
        
        // Animación de muerte
        if (animator != null) animator.SetTrigger("Muerte");

        // Bloquear movimiento (Desactiva el script de movimiento)
        var movimiento = GetComponent<MovimientosDelJugador>();
        if (movimiento != null) movimiento.enabled = false;

        // Iniciar la secuencia de ir al menú
        StartCoroutine(SecuenciaMuerte());
    }

    IEnumerator SecuenciaMuerte()
    {
        // 1. Esperamos 1.5 segundos (para ver la animación de caer)
        yield return new WaitForSeconds(1.5f);

        // 2. Mostramos el cartel de PERDISTE
        if (panelGameOver != null) panelGameOver.SetActive(true);

        // 3. Esperamos 3 segundos más para que el jugador lea el mensaje
        yield return new WaitForSeconds(3f);

        // 4. Cargamos el Menú Principal
        SceneManager.LoadScene("MenuPrincipal"); 
    }

    public void Curar(float cantidad)
    {
        if (estaMuerto) return;

        vidaActual += cantidad;

        // Asegurarnos de no tener más vida que el máximo
        if (vidaActual > vidaMaxima)
        {
            vidaActual = vidaMaxima;
        }

        // Actualizamos el corazón y el número
        ActualizarUI(); // Asegúrate de que tu función ActualizarUI() no sea 'private' si da error, o copia su contenido aquí.
    }
}