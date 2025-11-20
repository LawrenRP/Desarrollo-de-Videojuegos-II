using UnityEngine;

public class MovimientosDelJugador : MonoBehaviour
{
    public float velocidadMovimiento = 5f;
    public float fuerzaSalto = 10f;
    public float maxTiempoSalto = 0.3f;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    public Transform verificadorDeSuelo;
    public LayerMask capaDeSuelo;

    private bool estaEnElSuelo;
    private float tiempoSaltoActual;
    private bool saltando;
    
    // NUEVA VARIABLE: Para evitar ataques múltiples rápidos
    private bool estaAtacando = false; 

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // === LÓGICA DE SALTO ===
        if (Input.GetKeyDown(KeyCode.Space) && estaEnElSuelo)
        {
            saltando = true;
            tiempoSaltoActual = 0;
            // Corregido: Usar 'velocity' en lugar de 'linearVelocity'
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, fuerzaSalto);
        }

        if (Input.GetKey(KeyCode.Space) && saltando && tiempoSaltoActual < maxTiempoSalto)
        {
            tiempoSaltoActual += Time.deltaTime;
            // Corregido: Usar 'velocity' en lugar de 'linearVelocity'
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, fuerzaSalto);
        }

        if (Input.GetKeyUp(KeyCode.Space) || tiempoSaltoActual >= maxTiempoSalto)
        {
            saltando = false;
        }

        // === NUEVA LÓGICA DE ATAQUE ===
        HandleAttacks();
    }

    void FixedUpdate()
    {
        // === LÓGICA DE MOVIMIENTO ===
        float movimientoHorizontal = Input.GetAxis("Horizontal");

        if (movimientoHorizontal > 0)
        {
            spriteRenderer.flipX = false;
        }
        else if (movimientoHorizontal < 0)
        {
            spriteRenderer.flipX = true;
        }

        // Corregido: Usar 'velocity' en lugar de 'linearVelocity'
        rb.linearVelocity = new Vector2(movimientoHorizontal * velocidadMovimiento, rb.linearVelocity.y);

        // === LÓGICA DE SUELO Y ANIMACIONES DE MOVIMIENTO ===
        estaEnElSuelo = Physics2D.OverlapCircle(verificadorDeSuelo.position, 0.2f, capaDeSuelo);

        if (!estaEnElSuelo)
        {
            animator.SetBool("Saltar", true);
            animator.SetBool("Caminar", false);
        }
        else
        {
            animator.SetBool("Saltar", false);
            
            // Corregido: Usar 'velocity' en lugar de 'linearVelocity'
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

    // =========================================================================
    // FUNCIÓN PARA MANEJAR ATAQUES
    // =========================================================================
    void HandleAttacks()
    {
        // Si el jugador presiona la tecla de ataque (ej. 'Z' o 'Mouse0')
        if (Input.GetKeyDown(KeyCode.Z) && !estaAtacando) 
        {
            estaAtacando = true; // Bloquea nuevos ataques hasta que la animación termine

            if (estaEnElSuelo)
            {
                // Activa el Trigger para el ataque en el suelo
                animator.SetTrigger("Ataque");
            }
            else // Está en el aire
            {
                // Activa el Trigger para el ataque aéreo
                animator.SetTrigger("AtaqueAereo");
            }

            // **IMPORTANTE:** Aquí llamamos a la función que desbloquea el ataque.
            // La retrasamos el tiempo que dure la animación. 
            // ¡AJUSTA ESTE VALOR SEGÚN LA DURACIÓN REAL DE TU ANIMACIÓN DE ATAQUE!
            Invoke("FinishAttack", 0.3f); 
        }
    }

    // FUNCIÓN para resetear el bloqueo de ataque. Se llama con 'Invoke'.
    void FinishAttack()
    {
        estaAtacando = false;
    }
}