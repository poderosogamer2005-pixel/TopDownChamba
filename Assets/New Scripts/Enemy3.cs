using UnityEngine;
using UnityEngine.AI;

public class Enemy3 : MonoBehaviour, IInteractable, IDamagable
{
    [field: SerializeField] public int Health { get; set; } = 100;
    [SerializeField] private int damagePerHit = 20;
    [SerializeField] private float detectionRadius;
    [SerializeField] private float attackRadius;
    [SerializeField] private LayerMask playerMask;
    [SerializeField] private NavMeshAgent agent;


    private void Update()
    {
        if (Physics.CheckSphere(transform.position, detectionRadius, playerMask))
        {
            agent.SetDestination(FindAnyObjectByType<PlayerController3>().transform.position);
        }
        if (Physics.CheckSphere(transform.position, attackRadius, playerMask))
        {
            Vector3 direction = transform.position - FindAnyObjectByType<PlayerController3>().transform.position;
            Ray attackRay = new Ray(transform.position, direction);
            if (Physics.Raycast(attackRay, attackRadius, playerMask))
            {
                Debug.Log("Le pego al jugador");
            }
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
    public void RecibirDanio(int danio)
    {
        Health -= danio;

        Debug.Log("Vida restante del enemigo: " + Health);

        if (Health <= 0)
        {
            EnemyItemDrop drop = GetComponent<EnemyItemDrop>();

            if (drop != null)
            {
                drop.Morir();
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
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
