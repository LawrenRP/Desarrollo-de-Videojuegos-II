using UnityEngine;

public class SeguimientoDelJugador : MonoBehaviour
{
    // Una referencia al Transform del jugador que la cámara seguirá.
    public Transform jugador;

    // La distancia (offset) que la cámara mantendrá respecto al jugador.
    // Esto te permite ajustar la posición de la cámara en el Inspector.
    public Vector3 offset;

    // Se llama una vez por fotograma. Es ideal para lógica de cámara.
    void Update()
    {
        // Se asegura de que la referencia al jugador no sea nula.
        if (jugador != null)
        {
            // La nueva posición de la cámara es la posición del jugador más el offset.
            transform.position = jugador.position + offset;
        }
    }
}