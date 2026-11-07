using UnityEngine;

public class MaterialesRecogibles : MonoBehaviour
{
    [Header("Configuración del recurso")]
    [SerializeField] private int cantidadRecursos = 5;

    private bool recogido = false;

    private void OnTriggerEnter(Collider other)
    {
        if (recogido)
            return;

        // Comprobar si el objeto que toca es el jugador.
        if (other.CompareTag("Player"))
        {
            PlayerController5 jugador =
                other.GetComponent<PlayerController5>();

            if (jugador == null)
                jugador = other.GetComponentInParent<PlayerController5>();

            if (jugador != null)
            {
                // Entregar recursos al jugador.
                jugador.AgregarRecursos(cantidadRecursos);

                recogido = true;

                Debug.Log("Recogiste " + cantidadRecursos + " recursos.");

                // Destruir el objeto recogido.
                Destroy(gameObject);
            }
        }
    }
}
