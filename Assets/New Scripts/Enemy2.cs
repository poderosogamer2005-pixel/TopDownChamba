using UnityEngine;

public class Enemy2 : MonoBehaviour, IInteractable, IDamagable
{
    [field: SerializeField]
    public int Health { get; set; } = 100;

    [Header("Daño")]
    [SerializeField] private int damagePerHit = 20;

    [Header("Objeto que suelta")]
    [SerializeField] private GameObject objetoCurativo;
    [SerializeField] private Transform puntoDeCaida;

    public void Interact()
    {
        // El jugador golpea el objeto al presionar E.
        Health -= damagePerHit;

        // Evita que la vida sea negativa.
        Health = Mathf.Max(Health, 0);

        Debug.Log(gameObject.name +
            " recibió un golpe. Vida restante: " + Health);

        // Destruye el objeto cuando se queda sin vida.
        if (Health <= 0)
        {
            DestruirObjeto();
        }
    }

    private void DestruirObjeto()
    {
        Debug.Log(gameObject.name + " ha sido destruido.");

        // Si se asignó un objeto curativo, lo genera.
        if (objetoCurativo != null)
        {
            Vector3 posicion = puntoDeCaida != null
                ? puntoDeCaida.position
                : transform.position;

            Instantiate(
                objetoCurativo,
                posicion,
                Quaternion.identity
            );
        }
        else
        {
            Debug.LogWarning(
                "No se ha asignado el objeto curativo en el Inspector."
            );
        }

        // Destruye el objeto original.
        Destroy(gameObject);
    }
}
