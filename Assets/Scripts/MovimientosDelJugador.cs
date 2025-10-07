using UnityEngine;

public class MovimientosDelJugador : MonoBehaviour
{
    // Variables ajustables en el Inspector para controlar el movimiento y salto.
    public float velocidadMovimiento = 5f;
    public float fuerzaSalto = 10f;
    public float maxTiempoSalto = 0.3f; // Tiempo máximo que el personaje puede seguir saltando.

    // Referencias a los componentes de tu personaje.
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer; // Referencia al SpriteRenderer
    private Animator animator; // ¡Nueva referencia al Animator!
    public Transform verificadorDeSuelo;
    public LayerMask capaDeSuelo;

    // Variables de estado que el script usa internamente.
    private bool estaEnElSuelo;
    private float tiempoSaltoActual;
    private bool saltando; // Para saber si el personaje está en un salto variable.

    // Start se llama una vez al inicio.
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>(); // Obtenemos el componente Animator.
    }

    // Update se llama en cada fotograma.
    void Update()
    {
        // === Lógica de Salto Variable ===
        if (Input.GetKeyDown(KeyCode.Space) && estaEnElSuelo)
        {
            saltando = true;
            tiempoSaltoActual = 0;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, fuerzaSalto);
        }

        if (Input.GetKey(KeyCode.Space) && saltando && tiempoSaltoActual < maxTiempoSalto)
        {
            tiempoSaltoActual += Time.deltaTime;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, fuerzaSalto);
        }

        if (Input.GetKeyUp(KeyCode.Space) || tiempoSaltoActual >= maxTiempoSalto)
        {
            saltando = false;
        }
    }

    // FixedUpdate se usa para la física, asegurando un movimiento suave.
    // FixedUpdate se usa para la física, asegurando un movimiento suave.
    void FixedUpdate()
    {
        // === Movimiento Horizontal con WASD y Flechas ===
        float movimientoHorizontal = Input.GetAxis("Horizontal");

        // Aquí es donde aplicamos el volteo del sprite.
        if (movimientoHorizontal > 0)
        {
            spriteRenderer.flipX = false;
        }
        else if (movimientoHorizontal < 0)
        {
            spriteRenderer.flipX = true;
        }

        rb.linearVelocity = new Vector2(movimientoHorizontal * velocidadMovimiento, rb.linearVelocity.y);

        // === Detección de Suelo ===
        estaEnElSuelo = Physics2D.OverlapCircle(verificadorDeSuelo.position, 0.2f, capaDeSuelo);

        // === Lógica de Animación ===
        // Si el personaje NO está en el suelo...
        if (!estaEnElSuelo)
        {
            // ...activa la animación de salto y desactiva la de caminar.
            animator.SetBool("Saltar", true);
            animator.SetBool("Caminar", false);
        }
        else // Si el personaje SÍ está en el suelo...
        {
            // ...desactiva la animación de salto.
            animator.SetBool("Saltar", false);
            
            // ...y decide si activar la animación de caminar según la velocidad.
            if (Mathf.Abs(rb.linearVelocity.x) > 0.1f)
            {
                animator.SetBool("Caminar", true);
            }
            else
            {
                animator.SetBool("Caminar", false);
            }
        }
    }
}