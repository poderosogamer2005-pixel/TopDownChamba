using UnityEngine;

public class EnemyItemDrop : MonoBehaviour
{
    [Header("Objetos que puede soltar")]
    [SerializeField] private ItemSO[] objetosDisponibles;

    [Header("Objeto físico que aparecerá")]
    [SerializeField] private GameObject objetoRecogiblePrefab;

    [Range(0f, 1f)]
    [SerializeField] private float probabilidadDrop = 1f;

    private bool yaMurio;

    public void Morir()
    {
       
        if (yaMurio)
            return;

        yaMurio = true;

        if (objetosDisponibles.Length > 0 &&
            Random.value <= probabilidadDrop)
        {
            ItemSO objeto = objetosDisponibles[
                Random.Range(0, objetosDisponibles.Length)
            ];

            if (objeto != null && objetoRecogiblePrefab != null)
            {
                GameObject drop = Instantiate(
                    objetoRecogiblePrefab,
                    transform.position + Vector3.up * 0.5f,
                    Quaternion.identity
                );

                ItemPickup pickup = drop.GetComponent<ItemPickup>();

                if (pickup != null)
                    pickup.Configurar(objeto);
            }
        }

        Destroy(gameObject);
    }
}
