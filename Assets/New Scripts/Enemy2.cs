using UnityEngine;

public class Enemy2 : MonoBehaviour, IInteractable, IDamagable
{
    [field: SerializeField] public int Health { get; set; } = 100;

    [SerializeField] private int damagePerHit = 20;

    public void Interact()
    {
        // El jugador golpea al enemigo al presionar el boton de la E.
        Health -= damagePerHit;

        // Evita que la vida sea negativa.
        Health = Mathf.Max(Health, 0);

        // Esta parte muestra la vida actual del enemigo.
        Debug.Log(gameObject.name + " recibió un golpe. Vida restante: " + Health);

        // Desaparece el enemigo solamente cuando no tenga vida.
        if (Health <= 0)
        {
            Debug.Log(gameObject.name + " ha sido derrotado.");
            Destroy(gameObject);
        }
    }
}
