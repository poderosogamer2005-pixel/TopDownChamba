using UnityEngine;

public class PosicionesDeTrampas : MonoBehaviour, IInteractable
{
    [Header("Construcción")]
    [SerializeField] private GameObject prefabTrampa;

    [SerializeField] private int costoRecursos = 10;

    [Header("Posición de construcción")]
    [SerializeField] private Transform puntoConstruccion;

    private bool trampaConstruida = false;

    public void Interact()
    {
        // Comprobar si ya existe una trampa.
        if (trampaConstruida)
        {
            Debug.Log("Ya existe una trampa en esta posición.");
            return;
        }

        // Buscar al jugador.
        PlayerController5 jugador =
            FindAnyObjectByType<PlayerController5>();

        if (jugador == null)
            return;

        // Comprobar los recursos disponibles.
        if (jugador.Recursos < costoRecursos)
        {
            Debug.Log("No tienes suficientes recursos para construir la trampa.");
            Debug.Log("Necesitas: " + costoRecursos);
            return;
        }

        if (prefabTrampa == null)
        {
            Debug.LogError("No has asignado el prefab de la trampa.");
            return;
        }

        // Descontar recursos.
        jugador.GastarRecursos(costoRecursos);

        // Determinar dónde aparecerá la trampa.
        Transform punto = puntoConstruccion != null
            ? puntoConstruccion
            : transform;

        // Construir la trampa.
        Instantiate(
            prefabTrampa,
            punto.position,
            punto.rotation
        );

        trampaConstruida = true;

        Debug.Log("¡Trampa construida correctamente!");
    }
}

