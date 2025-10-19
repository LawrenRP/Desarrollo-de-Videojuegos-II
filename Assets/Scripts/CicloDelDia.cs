using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CicloDelDia : MonoBehaviour
{
    [Header("Componentes Requeridos")]
    public Camera MainCamera; 
    public Light2D GlobalLight;

    [Header("Fondos Parallax")]
    [Tooltip("Arrastra aquí TODOS los 'Planes' de tu fondo (sky, clouds, sea)")]
    public Renderer[] capasDelFondo;

    // Aquí guardaremos las copias de los materiales para que no haya conflictos
    private Material[] materialesDeCapa;

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
        if (MainCamera == null) { Debug.LogError("¡ERROR! Falta asignar la Main Camera."); }
        if (GlobalLight == null) { Debug.LogError("¡ERROR! Falta asignar la Global Light 2D."); }

        // Llenamos el array de materiales UNA SOLA VEZ al inicio
        materialesDeCapa = new Material[capasDelFondo.Length];
        for (int i = 0; i < capasDelFondo.Length; i++)
        {
            if (capasDelFondo[i] != null)
            {
                // Con .material (minúscula) creamos la instancia y la guardamos
                materialesDeCapa[i] = capasDelFondo[i].material;
            }
        }
    }

    void Update()
    {
        // Cálculos de tiempo y color
        tiempoTranscurrido += Time.deltaTime;
        tiempoTranscurrido %= DuracionCicloEnSegundos; 
        float progresoCiclo = tiempoTranscurrido / DuracionCicloEnSegundos;
        float t_night = 0.5f * (1 - Mathf.Cos(progresoCiclo * 2 * Mathf.PI));
        Color colorActual = Color.Lerp(ColorDia, ColorNoche, t_night);
        float intensidadActual = Mathf.Lerp(IntensidadDia, IntensidadNoche, t_night);

        // Aplicar a cámara y luz
        if (MainCamera != null) { MainCamera.backgroundColor = colorActual; }
        if (GlobalLight != null) { GlobalLight.intensity = intensidadActual; }

        // Aplicar color a los materiales que ya guardamos
        if (materialesDeCapa != null)
        {
            foreach (Material matInstancia in materialesDeCapa)
            {
                if (matInstancia != null)
                {
                    matInstancia.color = colorActual;
                }
            }
        }
    }
}