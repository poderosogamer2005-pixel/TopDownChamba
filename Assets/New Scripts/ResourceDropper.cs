using UnityEngine;

public class ResourceDropper : MonoBehaviour
{
    [Header("Drop de recursos")]
    [SerializeField] private GameObject recursoPrefab;
    [SerializeField] private int cantidadRecursos = 5;

    private bool destruido;

    public void Destruir()
    {
        if (destruido)
            return;

        destruido = true;

        if (recursoPrefab != null)
        {
            GameObject drop = Instantiate(
                recursoPrefab,
                transform.position + Vector3.up * 0.5f,
                Quaternion.identity
            );

            ResourcePickup pickup =
                drop.GetComponent<ResourcePickup>();

            if (pickup != null)
                pickup.Configurar(cantidadRecursos);
        }

        Destroy(gameObject);
    }
}
