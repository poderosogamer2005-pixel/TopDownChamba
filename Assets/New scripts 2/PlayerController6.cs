using System.Collections.Generic;
using UnityEngine;

using System.Linq;

public class PlayerController6 : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private CharacterController characterController;
    [SerializeField] private float speed = 5f;
    [SerializeField] private Camera mainCamera;

    [Header("Vida del jugador")]
    [SerializeField] private int maxHealth = 200;
    [SerializeField] private int currentHealth;
    [SerializeField] private bool destroyOnDeath = true;

    [Header("Melee")]
    [SerializeField] private float attackDistance = 1.5f;
    [SerializeField] private int attackDamage = 25;
    [SerializeField] private float attackCooldown = 0.4f;
    [SerializeField] private LayerMask enemyLayer;

    [Header("Interaction")]
    [SerializeField] private float interactableRadius = 2f;

    private float nextAttackTime;
    private bool isDead;

    public int CurrentHealth => currentHealth;
    public bool IsDead => isDead;
    public void Heal(int amount)
    {
        if (isDead || amount <= 0)
            return;

        currentHealth = Mathf.Clamp(currentHealth + amount, 0,maxHealth);

        Debug.Log("Jugador curado. Vida: " + currentHealth + "/" + maxHealth);
    }

    private void Awake()
    {
        if (characterController == null)
            characterController = GetComponent<CharacterController>();

        if (mainCamera == null)
            mainCamera = Camera.main;

        currentHealth = maxHealth;
    }

    private void Update()
    {
        if (isDead)
            return;

        Move();
        LookAtMouse();
        HandleInteraction();

        if (Input.GetMouseButtonDown(0))
            Attack();
    }

    private void Move()
    {
        if (characterController == null || mainCamera == null)
            return;

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 cameraForward = mainCamera.transform.up;
        Vector3 cameraRight = mainCamera.transform.right;

        cameraForward.y = 0f;
        cameraRight.y = 0f;

        cameraForward.Normalize();
        cameraRight.Normalize();

        Vector3 movement =
            cameraForward * vertical + cameraRight * horizontal;

        movement = Vector3.ClampMagnitude(movement, 1f);

        characterController.Move(movement * speed * Time.deltaTime);
    }

    private void LookAtMouse()
    {
        if (mainCamera == null)
            return;

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, transform.position);

        if (groundPlane.Raycast(ray, out float distance))
        {
            Vector3 mousePosition = ray.GetPoint(distance);
            Vector3 direction = mousePosition - transform.position;
            direction.y = 0f;

            if (direction.sqrMagnitude > 0.01f)
                transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    private void HandleInteraction()
    {
        IInteractable closestInteractable = null;
        float closestDistance = Mathf.Infinity;

        Collider[] colliders = Physics.OverlapSphere(transform.position + transform.forward, interactableRadius);

        foreach (Collider col in colliders)
        {
            IInteractable interactable =
                col.GetComponentInParent<IInteractable>();

            if (interactable == null)
                continue;

            float distance = Vector3.Distance(
                col.transform.position,
                transform.position
            );

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestInteractable = interactable;
            }
        }

        if (Input.GetKeyDown(KeyCode.E))
            closestInteractable?.Interact();
    }

    private void Attack()
    {
        if (Time.time < nextAttackTime)
            return;

        nextAttackTime = Time.time + attackCooldown;

        Vector3 origin = transform.position + Vector3.up;
        Vector3 direction = transform.forward;

        if (Physics.Raycast(origin, direction, out RaycastHit hit,attackDistance,enemyLayer))
        {
            Enemy4 enemy = hit.collider.GetComponentInParent<Enemy4>();

            if (enemy != null)
                enemy.TakeDamage(attackDamage);
        }

        Debug.DrawRay(origin, direction * attackDistance, Color.red,0.2f);
    }

    public void TakeDamage(int damage)
    {
        if (isDead || damage <= 0)
            return;

        currentHealth = Mathf.Max(0, currentHealth - damage);

        Debug.Log("Jugador recibió " + damage +" de daño. Vida: " + currentHealth + "/" + maxHealth);

        if (currentHealth <= 0)
            Die();
    }

    private void Die()
    {
        if (isDead)
            return;

        isDead = true;

        Debug.Log("¡El jugador ha muerto!");

        if (destroyOnDeath)
        {
            Destroy(gameObject);
        }
        else
        {
            if (characterController != null)
                characterController.enabled = false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;

        Gizmos.DrawWireSphere(
            transform.position + transform.forward,
            interactableRadius
        );
    }
}
