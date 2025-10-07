using UnityEngine;
using TMPro; // Necesario para usar TextMeshPro

public class RecoleccionDeFrutas : MonoBehaviour
{
    // Una variable pública para mostrar el texto de la UI.
    public TextMeshProUGUI textoContador;

    // Un contador para el inventario. Es público para que puedas verlo en el Inspector.
    public int manzanasEnInventario = 0;

    // Start se llama una vez al inicio del juego.
    void Start()
    {
        // Actualizamos el texto de la UI al inicio.
        ActualizarTextoContador();
    }

    // Update se llama en cada fotograma del juego.
    void Update()
    {
        // === Lógica de Consumo (Tecla 'R') ===
        // Si el jugador presiona la tecla 'R' y tiene al menos una manzana en el inventario...
        if (Input.GetKeyDown(KeyCode.R) && manzanasEnInventario > 0)
        {
            // Reduce el inventario en 1.
            manzanasEnInventario--;
            Debug.Log("Manzana comida. Inventario: " + manzanasEnInventario);
            // Actualiza el texto en la pantalla.
            ActualizarTextoContador();
        }
    }

    // Se activa cuando un objeto con un Collider2D entra en el Trigger.
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Revisa si el objeto que colisionó tiene el tag "Fruta".
        if (collision.gameObject.CompareTag("Fruta"))
        {
            // Aumenta el contador de manzanas.
            manzanasEnInventario++;
            Debug.Log("Manzana recogida. Inventario: " + manzanasEnInventario);
            // Destruye la manzana para que desaparezca.
            Destroy(collision.gameObject);
            // Actualiza el texto en la pantalla.
            ActualizarTextoContador();
        }
    }

    // Función para actualizar el texto en la UI.
    void ActualizarTextoContador()
    {
        // Si la referencia al texto no es nula, actualizamos el contenido.
        if (textoContador != null)
        {
            textoContador.text = "Manzanas: " + manzanasEnInventario;
        }
    }
}