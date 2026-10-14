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
        PlayerController3 jugador =
            other.GetComponentInParent<PlayerController3>();

        if (jugador == null || objeto == null)
            return;

        jugador.AgregarObjeto(objeto);
        Destroy(gameObject);
    }
}
