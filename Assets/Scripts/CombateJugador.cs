using UnityEngine;

public class CombateJugador : MonoBehaviour
{
    [Header("Configuración de Ataque")]
    public Transform controladorGolpe; // Un objeto vacío en la punta de la espada/mano
    public float radioGolpe = 0.5f;
    public float dañoGolpe = 20f;
    public float tiempoEntreAtaques = 0.5f;

    [Header("Teclas")]
    public KeyCode teclaAtacar = KeyCode.Z; // O KeyCode.Mouse0 para click

    private Animator animator;
    private float siguienteAtaque = 0f;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (Time.time >= siguienteAtaque)
        {
            if (Input.GetKeyDown(teclaAtacar))
            {
                Atacar();
                siguienteAtaque = Time.time + tiempoEntreAtaques;
            }
        }
    }

    void Atacar()
    {
        // 1. Reproducir animación
        if(animator != null) animator.SetTrigger("Atacar");

        // 2. Detectar enemigos en el rango
        Collider2D[] objetosGolpeados = Physics2D.OverlapCircleAll(controladorGolpe.position, radioGolpe);

        // 3. Dañar a los enemigos detectados
        foreach (Collider2D objeto in objetosGolpeados)
        {
            // Buscamos si tiene el script del enemigo
            EnemigoIA enemigo = objeto.GetComponent<EnemigoIA>();
            if (enemigo != null)
            {
                enemigo.TomarDaño(dañoGolpe);
            }
        }
    }

    // Dibujar el círculo de ataque en el editor para ajustarlo
    void OnDrawGizmosSelected()
    {
        if (controladorGolpe == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(controladorGolpe.position, radioGolpe);
    }
}