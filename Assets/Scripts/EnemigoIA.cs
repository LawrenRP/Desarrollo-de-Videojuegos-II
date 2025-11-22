using UnityEngine;

public class EnemigoIA : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    public float velocidadPatrulla = 2f;
    public float velocidadPersecucion = 3.5f; // Un poco más rápido al perseguir
    public Transform puntoA;
    public Transform puntoB;
    
    [Header("Detección y Ataque")]
    public float rangoDeteccion = 5f; // Distancia para empezar a perseguir
    public float rangoAtaque = 1.2f;  // Distancia para detenerse y lanzar golpe
    public float dañoAlJugador = 10f;
    public float fuerzaEmpuje = 10f; 
    public float tiempoEntreAtaques = 2f; // Cooldown para no spamear animación

    [Header("Vida del Enemigo")]
    public float vidaMaxima = 50f;
    private float vidaActual;

    private Transform objetivoActual;
    private Transform jugador;
    private Animator animator;
    private Rigidbody2D rb;
    private bool estaMuerto = false;
    private float siguienteAtaque = 0f; // Temporizador interno

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        
        // Buscamos al jugador por Tag
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) jugador = playerObj.transform;
        
        objetivoActual = puntoA;
        vidaActual = vidaMaxima;
    }

    void Update()
    {
        if (estaMuerto || jugador == null) return;

        float distanciaJugador = Vector2.Distance(transform.position, jugador.position);

        // === MÁQUINA DE ESTADOS SIMPLE ===

        // 1. Si está en RANGO DE ATAQUE (Muy cerca)
        if (distanciaJugador < rangoAtaque)
        {
            // Detenemos al enemigo para que golpee quieto
            rb.linearVelocity = Vector2.zero; 
            
            // Dejamos de caminar en la animación
            if(animator != null) animator.SetBool("Caminar", false);

            // Comprobamos cooldown
            if (Time.time > siguienteAtaque)
            {
                if(animator != null) animator.SetTrigger("Atacar");
                siguienteAtaque = Time.time + tiempoEntreAtaques;
            }
        }
        // 2. Si está en RANGO DE DETECCIÓN (Lo ve pero no lo alcanza)
        else if (distanciaJugador < rangoDeteccion)
        {
            PerseguirJugador();
        }
        // 3. Si está LEJOS (Patrullar)
        else
        {
            Patrullar();
        }
    }

    void Patrullar()
    {
        Moverse(objetivoActual.position, velocidadPatrulla);

        if (Vector2.Distance(transform.position, objetivoActual.position) < 0.5f)
        {
            objetivoActual = (objetivoActual == puntoA) ? puntoB : puntoA;
        }
    }

    void PerseguirJugador()
    {
        Moverse(jugador.position, velocidadPersecucion);
    }

    void Moverse(Vector3 destino, float velocidad)
    {
        // Mueve la posición
        transform.position = Vector2.MoveTowards(transform.position, destino, velocidad * Time.deltaTime);

        // Animación de caminar
        if(animator != null) animator.SetBool("Caminar", true);

        // Girar sprite
        if (destino.x > transform.position.x) 
            transform.localScale = new Vector3(1, 1, 1); // Derecha
        else 
            transform.localScale = new Vector3(-1, 1, 1); // Izquierda
    }

    // === DAÑO FÍSICO AL JUGADOR (Si te toca el cuerpo) ===
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (estaMuerto) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            VidaJugador scriptVida = collision.gameObject.GetComponent<VidaJugador>();
            
            if (scriptVida != null && !scriptVida.esInvulnerable)
            {
                // 1. Hacer Daño
                scriptVida.RecibirDaño(dañoAlJugador);

                // 2. Empujar al jugador (Knockback)
                Rigidbody2D rbJugador = collision.gameObject.GetComponent<Rigidbody2D>();
                if (rbJugador != null)
                {
                    Vector2 direccion = (collision.transform.position - transform.position).normalized;
                    Vector2 empuje = new Vector2(Mathf.Sign(direccion.x) * fuerzaEmpuje, 5f);
                    
                    rbJugador.linearVelocity = Vector2.zero; // Usamos velocity estándar
                    rbJugador.AddForce(empuje, ForceMode2D.Impulse);
                }
            }
        }
    }

    // === RECIBIR DAÑO DEL JUGADOR ===
    public void TomarDaño(float daño)
    {
        if (estaMuerto) return;

        vidaActual -= daño;

        if (animator != null) animator.SetTrigger("Hurt");

        if (vidaActual <= 0)
        {
            Morir();
        }
    }

    void Morir()
    {
        estaMuerto = true;
        if (animator != null) animator.SetTrigger("Muerte");
        
        // Desactivar colisiones
        GetComponent<Collider2D>().enabled = false;
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.linearVelocity = Vector2.zero;
        this.enabled = false; 

        Destroy(gameObject, 2f);
    }

    // Dibuja los rangos en el editor
    private void OnDrawGizmosSelected()
    {
        // Rango visión (Rojo)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, rangoDeteccion);

        // Rango ataque (Amarillo)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, rangoAtaque);
    }
}