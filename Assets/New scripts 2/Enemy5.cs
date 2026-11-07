using UnityEngine;
using UnityEngine.AI;

public class Enemy5 : MonoBehaviour, IInteractable, IDamagable
{
    [field: SerializeField]
    public int Health { get; set; } = 100;

    [Header("Daño al enemigo")]
    [SerializeField] private int damagePerHit = 20;

    [Header("Daño al jugador")]
    public int playerDamage = 10;
    public float attackInterval = 2f;

    [Header("Detección")]
    public float detectionRadius = 10f;
    public float attackRadius = 2f;
    public LayerMask playerMask;

    [Header("Movimiento")]
    public NavMeshAgent agent;
    public float randomMovementRange = 5f;

    [Header("Tiempo de estados")]
    public float patrolInterval = 3f;
    public float confusionTime = 5f;

    [Header("Jugador")]
    public PlayerController5 player;

    [Header("Máquina de estados")]
    public StateMachine stateMachine;
    public IdleState idleState;
    public ChaseState chaseState;
    public ConfusionState confusionState;
    public AttackState attackState;

    [field: SerializeField]
    public LayerMask hitBy { get; set; }

    [HideInInspector]
    public Vector3 lastKnownPosition;

    private bool yaMurio;

    public bool EstaMuerto => yaMurio;

    private void Awake()
    {
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        player = FindAnyObjectByType<PlayerController5>();

        stateMachine = new StateMachine();

        idleState = new IdleState(this, stateMachine);
        chaseState = new ChaseState(this, stateMachine);
        confusionState = new ConfusionState(this, stateMachine);
        attackState = new AttackState(this, stateMachine);

        stateMachine.Initialize(idleState);
    }

    private void Update()
    {
        if (yaMurio)
            return;

        if (player == null)
        {
            player = FindAnyObjectByType<PlayerController5>();
            return;
        }

        stateMachine.currentState.FrameUpdate();
    }

    public bool DetectarJugador()
    {
        Collider[] colliders = Physics.OverlapSphere(
            transform.position,
            detectionRadius,
            playerMask
        );

        foreach (Collider collider in colliders)
        {
            PlayerController5 jugadorDetectado =
                collider.GetComponentInParent<PlayerController5>();

            if (jugadorDetectado == player)
            {
                lastKnownPosition = player.transform.position;
                return true;
            }
        }

        return false;
    }

    public void CambiarEstado(EnemyState nuevoEstado)
    {
        if (stateMachine.currentState == nuevoEstado)
            return;

        stateMachine.ChangeState(nuevoEstado);
    }

    public void RecibirDanio(int danio)
    {
        if (yaMurio || danio <= 0)
            return;

        Health = Mathf.Max(Health - danio, 0);

        Debug.Log(name + " recibió " + danio +
                  " de daño. Vida: " + Health);

        if (Health <= 0)
            Morir();
    }

    public void Interact()
    {
        if (yaMurio)
            return;

        Health = Mathf.Max(Health - damagePerHit, 0);

        Debug.Log(name + " recibió un golpe. Vida: " + Health);

        if (Health <= 0)
            Morir();
    }

    private void Morir()
    {
        if (yaMurio)
            return;

        yaMurio = true;

        if (agent != null)
        {
            agent.isStopped = true;
            agent.ResetPath();
        }

        Debug.Log(name + " ha sido derrotado.");

        EnemyItemDrop drop = GetComponent<EnemyItemDrop>();

        if (drop != null)
            drop.Morir();
        else
            Destroy(gameObject);
    }

    public void Die()
    {
        Morir();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}
