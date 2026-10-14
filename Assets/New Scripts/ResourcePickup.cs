using UnityEngine;

public class ResourcePickup : MonoBehaviour
{
    [SerializeField] private int cantidad = 5;

    public void Configurar(int nuevaCantidad)
    {
        cantidad = nuevaCantidad;
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerController3 jugador =
            other.GetComponentInParent<PlayerController3>();

        if (jugador == null)
            return;

        jugador.AgregarRecursos(cantidad);
        Destroy(gameObject);
    }
}
