using UnityEngine;

public class MovimientosDelJugador : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    public float velocidadMovimiento = 5f;
    public float fuerzaSalto = 10f;
    public float maxTiempoSalto = 0.3f; // Tiempo que puedes mantener el salto

    [Header("Configuración de Audio")]
    [SerializeField] private AudioSource sfxAudioSource; 
    
    [Space(10)] 
    [SerializeField] private AudioClip sonidoPasosPasto;
    [Range(0f, 1f)] public float volumenPasos = 0.5f; 

    [Space(10)]
    [SerializeField] private AudioClip sonidoAtaque;
    [Range(0f, 1f)] public float volumenAtaque = 1f; 

    private AudioSource pasosAudioSource; 

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    public Transform verificadorDeSuelo;
    public LayerMask capaDeSuelo;

    private bool estaEnElSuelo;
    
    // Variables para el salto variable
    private float tiempoSaltoActual;
    private bool saltando;
    
    private bool estaAtacando = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        
        if (sfxAudioSource == null) sfxAudioSource = GetComponent<AudioSource>();
        
        pasosAudioSource = gameObject.AddComponent<AudioSource>();
        pasosAudioSource.clip = sonidoPasosPasto;
        pasosAudioSource.loop = true; 
        pasosAudioSource.playOnAwake = false;
        pasosAudioSource.volume = volumenPasos; 
        
        if (sfxAudioSource != null)
        {
            pasosAudioSource.spatialBlend = sfxAudioSource.spatialBlend;
        }
    }

    void Update()
    {
        if (pasosAudioSource != null)
        {
            pasosAudioSource.volume = volumenPasos;
        }

        // === LÓGICA DE SALTO (VARIABLE) ===
        
        // 1. Inicio del salto
        if (Input.GetKeyDown(KeyCode.Space) && estaEnElSuelo)
        {
            saltando = true;
            tiempoSaltoActual = 0;
            // Aplicamos fuerza inicial
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, fuerzaSalto);
        }

        // 2. Mantener salto (mientras la tecla sigue presionada y no se acabe el tiempo)
        if (Input.GetKey(KeyCode.Space) && saltando)
        {
            if (tiempoSaltoActual < maxTiempoSalto)
            {
                tiempoSaltoActual += Time.deltaTime;
                // Mantenemos la velocidad hacia arriba
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, fuerzaSalto);
            }
            else
            {
                saltando = false;
            }
        }

        // 3. Soltar salto
        if (Input.GetKeyUp(KeyCode.Space))
        {
            saltando = false;
        }

        // === LÓGICA DE ATAQUE ===
        HandleAttacks();
    }

    void FixedUpdate()
    {
        // === LÓGICA DE MOVIMIENTO ===
        float movimientoHorizontal = Input.GetAxis("Horizontal");

        if (movimientoHorizontal > 0) spriteRenderer.flipX = false;
        else if (movimientoHorizontal < 0) spriteRenderer.flipX = true;

        rb.linearVelocity = new Vector2(movimientoHorizontal * velocidadMovimiento, rb.linearVelocity.y);

        // === LÓGICA DE SUELO Y ANIMACIONES ===
        estaEnElSuelo = Physics2D.OverlapCircle(verificadorDeSuelo.position, 0.2f, capaDeSuelo);

        // ESTA ES LA PARTE QUE FALTABA PARA TU ANIMACIÓN DE CAÍDA:
        // Le enviamos al Animator la velocidad vertical (Y).
        // Si es positiva (> 0) está saltando. Si es negativa (< 0) está cayendo.
        if (animator != null)
        {
            animator.SetFloat("VelocidadVertical", rb.linearVelocity.y);
            animator.SetBool("EnSuelo", estaEnElSuelo); // Ayuda extra para transiciones
        }

        if (!estaEnElSuelo) // AIRE
        {
            animator.SetBool("Saltar", true);
            animator.SetBool("Caminar", false);
            StopFootsteps();
        }
        else // SUELO
        {
            animator.SetBool("Saltar", false);
            
            if (Mathf.Abs(rb.linearVelocity.x) > 0.1f)
            {
                animator.SetBool("Caminar", true);
                PlayFootsteps();
            }
            else
            {
                animator.SetBool("Caminar", false);
                StopFootsteps();
            }
        }
    }

    void PlayFootsteps()
    {
        if (pasosAudioSource != null && sonidoPasosPasto != null)
        {
            if (!pasosAudioSource.isPlaying)
            {
                pasosAudioSource.Play();
            }
        }
    }

    void StopFootsteps()
    {
        if (pasosAudioSource != null && pasosAudioSource.isPlaying)
        {
            pasosAudioSource.Stop();
        }
    }

    void HandleAttacks()
    {
        if (Input.GetKeyDown(KeyCode.Z) && !estaAtacando) 
        {
            estaAtacando = true; 

            if(sonidoAtaque != null && sfxAudioSource != null)
            {
                sfxAudioSource.PlayOneShot(sonidoAtaque, volumenAtaque);
            }

            if (estaEnElSuelo) animator.SetTrigger("Ataque");
            else animator.SetTrigger("AtaqueAereo");

            Invoke("FinishAttack", 0.3f); 
        }
    }

    void FinishAttack()
    {
        estaAtacando = false;
    }
}