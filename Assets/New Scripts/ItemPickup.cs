using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    [SerializeField] private ItemSO objeto;

    public void Configurar(ItemSO nuevoObjeto)
    {
        objeto = nuevoObjeto;
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerController5 jugador =
            other.GetComponentInParent<PlayerController5>();

        if (jugador == null || objeto == null)
            return;

        jugador.AgregarObjeto(objeto);
        Destroy(gameObject);
    }
}
