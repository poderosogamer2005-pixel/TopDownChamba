using UnityEngine;
using System;
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] 
    private CharacterController characterController; 
    [SerializeField] 
    private float speed = 5f; 
    [Header("Melee")]
    [SerializeField] 
    private float attackDistance = 1.5f; 
    [SerializeField]
    private int attackDamage = 25; 
    [SerializeField] 
    private float attackCooldown = 0.4f; 
    [SerializeField] 
    private LayerMask enemyLayer;
    [Header("References")]
    [SerializeField] private Camera mainCamera;

    private float nextAttackTime;

    //El personaje lo puedo mover con el mouse de la vista
    private void Update()
    {
        Move();

        LookAtMouse();

        if (Input.GetMouseButtonDown(0))
        {
            Attack();
        }
    }

    //Movimiento del personaje y como lo sigue con la camara
    private void Move()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 cameraForward = mainCamera.transform.forward;
        Vector3 cameraRight = mainCamera.transform.right;

        // Ignoremos la inclinación vertical de la cámara como esta
        cameraForward.y = 0f;
        cameraRight.y = 0f;

        cameraForward.Normalize();
        cameraRight.Normalize();

        Vector3 movementVector =
            cameraForward * vertical +
            cameraRight * horizontal;

        movementVector.Normalize();

        characterController.Move(movementVector * speed * Time.deltaTime);
        
        
    }
    private void LookAtMouse()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        // Este es el plano horizontal a la altura del jugador
        
         Plane groundPlane = new Plane(Vector3.up, transform.position);

        if (groundPlane.Raycast(ray, out float distance))
        {
            Vector3 mouseWorldPosition = ray.GetPoint(distance); 
            Vector3 direction = mouseWorldPosition - transform.position; 
            direction.y = 0f;

            if (direction.sqrMagnitude > 0.01f)
            {
                transform.rotation = Quaternion.LookRotation(direction);
            }
        }

    }

    //utilice raycast para cuando ataque, sin embargo debe estar muy cerca del personaje para hacerle daño
    private void Attack()
    {
        if (Time.time < nextAttackTime) 
            return;

        nextAttackTime = Time.time + attackCooldown;

        Vector3 origin = transform.position + Vector3.up * 1f; 
        Vector3 direction = transform.forward;

        if (Physics.Raycast(origin, direction, out RaycastHit hit, attackDistance, enemyLayer))
        {
            Enemy enemy = hit.collider.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(attackDamage);
            }
        }
        Debug.DrawRay(origin, direction * attackDistance, Color.red, 0.2f);
    }
}
