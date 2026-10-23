using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class Enemy4 : MonoBehaviour
{
    private static readonly List<Enemy4> activeEnemies =new List<Enemy4>();

    [Header("Vida y daño")]
    [SerializeField] private int health = 100;
    [SerializeField] private int damagePerHit = 5;

    [Header("Persecución")]
    [SerializeField] private float detectionRadius = 8f;
    [SerializeField] private float attackRadius = 1.5f;
    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private NavMeshAgent agent;

    private PlayerController6 player;
    private DiaNocheManager dayNightManager;

    private float nextAttackTime;
    private bool isDead;
    private bool isNightEnemy;

    public bool IsDead => isDead;
    public bool IsNightEnemy => isNightEnemy;

    public static Enemy4[] GetAllEnemies()
    {
        return activeEnemies.ToArray();
    }

    private void OnEnable()
    {
        if (!activeEnemies.Contains(this))
            activeEnemies.Add(this);
    }

    private void OnDisable()
    {
        activeEnemies.Remove(this);
    }

    private void Awake()
    {
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        player = FindAnyObjectByType<PlayerController6>();

        if (dayNightManager == null)
            dayNightManager = FindAnyObjectByType<DiaNocheManager>();
    }

    public void ConfigureNightEnemy(DiaNocheManager manager)
    {
        isNightEnemy = true;
        dayNightManager = manager;
    }

    private void Update()
    {
        if (isDead || player == null || player.IsDead)
        {
            if (agent != null && agent.enabled && agent.isOnNavMesh)
                agent.isStopped = true;

            return;
        }

        float distance = Vector3.Distance(
            transform.position,
            player.transform.position
        );

        bool shouldChase =
            isNightEnemy || distance <= detectionRadius;

        if (agent != null && agent.enabled && agent.isOnNavMesh)
        {
            agent.isStopped = !shouldChase;

            if (shouldChase && distance > attackRadius)
                agent.SetDestination(player.transform.position);
        }

        if (shouldChase && distance <= attackRadius)
            AttackPlayer();
    }

    private void AttackPlayer()
    {
        if (Time.time < nextAttackTime)
            return;

        nextAttackTime = Time.time + attackCooldown;

        if (player != null && !player.IsDead)
            player.TakeDamage(damagePerHit);
    }

    public void TakeDamage(int damage)
    {
        if (isDead || damage <= 0)
            return;

        health -= damage;

        Debug.Log( gameObject.name + " recibió " + damage + " de daño. Vida restante: " + Mathf.Max(health, 0));

        if (health <= 0)
            Die();
    }

    private void Die()
    {
        if (isDead)
            return;

        isDead = true;

        if (isNightEnemy && dayNightManager != null)
            dayNightManager.NotifyNightEnemyKilled();

        Destroy(gameObject);
    }

    // Elimina al enemigo al cambiar de día o de noche.
    // No cuenta como una muerte normal de la oleada.
    public void DespawnForPhaseChange()
    {
        if (isDead)
            return;

        isDead = true;
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}
