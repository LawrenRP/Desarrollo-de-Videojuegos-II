using UnityEngine;

public class MovimientosDelJugador : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    public float velocidadMovimiento = 5f;
    public float fuerzaSalto = 10f;
    public float maxTiempoSalto = 0.3f;

    [Header("Configuración de Audio")]
    [SerializeField] private AudioSource sfxAudioSource; // AudioSource principal (Ataques)
    
    [Space(10)] // Espacio visual en el inspector
    [SerializeField] private AudioClip sonidoPasosPasto;
    [Range(0f, 1f)] public float volumenPasos = 0.5f; // ¡NUEVO! Control de volumen pasos

    [Space(10)]
    [SerializeField] private AudioClip sonidoAtaque;
    [Range(0f, 1f)] public float volumenAtaque = 1f; // ¡NUEVO! Control de volumen ataque

    // Fuente de audio secundaria generada por código para los pasos
    private AudioSource pasosAudioSource; 

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    public Transform verificadorDeSuelo;
    public LayerMask capaDeSuelo;

    private bool estaEnElSuelo;
    private float tiempoSaltoActual;
    private bool saltando;
    private bool estaAtacando = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        
        // 1. Configurar AudioSource Principal
        if (sfxAudioSource == null) sfxAudioSource = GetComponent<AudioSource>();
        
        // 2. Crear AudioSource Secundario (Pasos)
        pasosAudioSource = gameObject.AddComponent<AudioSource>();
        pasosAudioSource.clip = sonidoPasosPasto;
        pasosAudioSource.loop = true; 
        pasosAudioSource.playOnAwake = false;
        pasosAudioSource.volume = volumenPasos; // Asignamos el volumen inicial
        
        // Copiar configuración 3D
        if (sfxAudioSource != null)
        {
            pasosAudioSource.spatialBlend = sfxAudioSource.spatialBlend;
        }
    }

    void Update()
    {
        // Actualizar volumen de pasos en tiempo real (por si lo cambias mientras juegas)
        if (pasosAudioSource != null)
        {
            pasosAudioSource.volume = volumenPasos;
        }

        // === LÓGICA DE SALTO ===
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

            // Reproducir sonido de ataque con el volumen personalizado
            if(sonidoAtaque != null && sfxAudioSource != null)
            {
                // PlayOneShot acepta un segundo parámetro: la escala de volumen (0 a 1)
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