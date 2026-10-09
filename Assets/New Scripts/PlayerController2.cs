using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerController2 : MonoBehaviour
{
    [SerializeField] private CharacterController characterController;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float interactableRadius = 2f;
    [SerializeField] private LayerMask enemyLayer;

    [Header("Apuntar con el mouse")]
    [SerializeField] private Camera camara;
    [SerializeField] private LayerMask sueloLayer;
    [SerializeField] private float rotationSpeed = 20f;

    private void Update()
    {
        Movement();
        Looking();
        Shooting();

        IInteractable closestInteractable = null;

        List<Collider> interactableColliders = Physics.OverlapSphere(transform.position + transform.forward, interactableRadius).ToList();

        float closestDistance = Mathf.Infinity;

        foreach (Collider collider in interactableColliders)
        {
            if (collider.transform.TryGetComponent<IInteractable>(
                out IInteractable interactable))
            {
                float distance = Vector3.Distance(collider.transform.position, transform.position);

                if (distance < closestDistance)
                {
                    closestInteractable = interactable;
                    closestDistance = distance;
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            closestInteractable?.Interact();
        }
    }

    private void Movement()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        // Movimiento independiente de la dirección de la mirada que de
        Vector3 movementVector = new Vector3(horizontal,0f,vertical);

        // Evita que el movimiento diagonal sea más rápido
        movementVector = Vector3.ClampMagnitude(movementVector, 1f);

        characterController.Move(
            movementVector * speed * Time.deltaTime);
    }

    private void Looking()
    {
        if (camara == null)
            return;

        Ray ray = camara.ScreenPointToRay(Input.mousePosition);

        // El rayo solo debe detectar el suelo
        if (Physics.Raycast(ray, out RaycastHit hitInfo, 500f,sueloLayer))
        {
            // Dirección desde el jugador hasta el mouse
            Vector3 direction = hitInfo.point - transform.position;

            // Impide que el personaje se incline hacia arriba o abajo
            direction.y = 0f;

            if (direction.sqrMagnitude > 0.001f)
            {
                Quaternion rotacionObjetivo = Quaternion.LookRotation(direction);

                // Giro suave incluso mientras el jugador se mueve donde sea
                transform.rotation = Quaternion.Slerp(transform.rotation, rotacionObjetivo,rotationSpeed * Time.deltaTime);
            }
        }
    }

    private void Shooting()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray fireRay = new Ray(transform.position + Vector3.up,transform.forward);

            Debug.DrawRay(fireRay.origin, fireRay.direction * 10f, Color.green, 2f);

            if (Physics.Raycast(fireRay, out RaycastHit enemyInfo, 100f, enemyLayer))
            {
                Debug.Log(enemyInfo.transform.gameObject.name);
            }
            else
            {
                Debug.Log("No le diste al enemigo");
            }
        }
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;

        Gizmos.DrawSphere(transform.position + transform.forward, interactableRadius);
    }

}
