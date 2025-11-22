using UnityEngine;

public class Trampa : MonoBehaviour
{
    [Header("Configuración")]
    public float daño = 20f;
    public float fuerzaEmpuje = 10f; // Fuerza horizontal
    public float fuerzaLevante = 5f; // Un pequeño salto hacia arriba

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            VidaJugador scriptVida = collision.gameObject.GetComponent<VidaJugador>();

            // Solo aplicamos daño y empuje si el jugador NO es invulnerable
            if (scriptVida != null && !scriptVida.esInvulnerable)
            {
                // 1. Aplicar Daño
                scriptVida.RecibirDaño(daño);

                // 2. Aplicar Empuje (Knockback)
                Rigidbody2D rbJugador = collision.gameObject.GetComponent<Rigidbody2D>();
                if (rbJugador != null)
                {
                    // Detenemos al jugador para que el empuje sea seco y consistente
                    rbJugador.linearVelocity = Vector2.zero;

                    // Calculamos la dirección: (Jugador - Trampa) nos da un vector que "se aleja" de la trampa
                    Vector2 direccion = (collision.transform.position - transform.position).normalized;

                    // Aplicamos fuerza: Empuje en la dirección contraria + un poco hacia arriba para hacer un arco
                    Vector2 empujeFinal = new Vector2(Mathf.Sign(direccion.x) * fuerzaEmpuje, fuerzaLevante);
                    
                    rbJugador.AddForce(empujeFinal, ForceMode2D.Impulse);
                }
            }
        }
    }
}