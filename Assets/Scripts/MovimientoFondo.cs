using UnityEngine;
using UnityEngine.Rendering.Universal;

public class MovimientoFondo : MonoBehaviour
{
    // === VARIABLES DE PARALLAX ===
    Transform cam;
    Vector3 camStartPos;
    float distance;

    GameObject[] backgrounds;
    Material[] mat; // Materiales instanciados
    float[] backSpeed;

    float farthestBack;

    [Range(0.01f, 1f)]
    public float parallaxSpeed;

    // === VARIABLES DE CICLO DE DÍA ===
    [Header("Componentes Requeridos")]
    public Camera MainCamera; 
    public Light2D GlobalLight;

    [Header("Configuración del Ciclo")]
    public float DuracionCicloEnSegundos = 600f; 

    [Header("Colores de Ambiente")]
    public Color ColorDia = new Color(1f, 1f, 1f, 1f);
    public Color ColorNoche = new Color(0.2f, 0.25f, 0.4f, 1f);

    [Header("Intensidad de Luz Global")]
    public float IntensidadDia = 1f;
    public float IntensidadNoche = 0.2f;

    private float tiempoTranscurrido = 0f;

    void Start()
    {
        cam = Camera.main.transform;
        camStartPos = cam.position;

        int backCount = transform.childCount;
        mat = new Material[backCount];
        backSpeed = new float[backCount];
        backgrounds = new GameObject[backCount];

        for (int i = 0; i < backCount; i++)
        {
            backgrounds[i] = transform.GetChild(i).gameObject;
            
            // SOLUCIÓN: Usar sharedMaterial para obtener el material original
            // y luego crear una instancia con 'material' (no 'sharedMaterial')
            Renderer renderer = backgrounds[i].GetComponent<Renderer>();
            if (renderer != null)
            {
                // Esto crea automáticamente una instancia del material
                mat[i] = renderer.material;
            }
        }

        BackSpeedCalculate(backCount);
        
        if (MainCamera == null) { MainCamera = Camera.main; }
        if (GlobalLight == null) { Debug.LogWarning("Falta asignar la Global Light 2D."); }
    }

    void BackSpeedCalculate(int backCount)
    {
        for (int i = 0; i < backCount; i++) 
        {
            if ((backgrounds[i].transform.position.z - cam.position.z) > farthestBack)
            {
                farthestBack = backgrounds[i].transform.position.z - cam.position.z;
            }
        }

        for (int i = 0; i < backCount; i++)
        {
            backSpeed[i] = 1 - (backgrounds[i].transform.position.z - cam.position.z) / farthestBack;
        }
    }

    private void LateUpdate()
    {
        // === CÁLCULO DE CICLO DE DÍA ===
        tiempoTranscurrido += Time.deltaTime;
        tiempoTranscurrido %= DuracionCicloEnSegundos; 
        float progresoCiclo = tiempoTranscurrido / DuracionCicloEnSegundos;
        float t_night = 0.5f * (1 - Mathf.Cos(progresoCiclo * 2 * Mathf.PI));
        Color colorActual = Color.Lerp(ColorDia, ColorNoche, t_night);
        float intensidadActual = Mathf.Lerp(IntensidadDia, IntensidadNoche, t_night);

        // === APLICAR CAMBIOS DE ILUMINACIÓN ===
        if (MainCamera != null) { MainCamera.backgroundColor = colorActual; }
        if (GlobalLight != null) { GlobalLight.intensity = intensidadActual; }
        
        // === PARALLAX ===
        distance = cam.position.x - camStartPos.x;
        transform.position = new Vector3(cam.position.x, transform.position.y, transform.position.z);

        // === APLICAR PARALLAX Y COLOR ===
        for (int i = 0; i < backgrounds.Length; i++)
        {
            if (mat[i] != null)
            {
                // Aplicar offset de textura (Parallax)
                float speed = backSpeed[i] * parallaxSpeed;
                // Para URP/Lit shader, usar "_BaseMap" en vez de "_MainTex"
                mat[i].SetTextureOffset("_BaseMap", new Vector2(distance, 0) * speed);
                
                // Aplicar color del ciclo de día
                mat[i].color = colorActual;
            }
        }
    }

    // Importante: Liberar materiales instanciados al destruir el objeto
    void OnDestroy()
    {
        if (mat != null)
        {
            foreach (Material m in mat)
            {
                if (m != null)
                {
                    Destroy(m);
                }
            }
        }
    }
}