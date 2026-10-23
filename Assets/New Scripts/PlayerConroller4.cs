using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerConroller4 : MonoBehaviour
{
    [SerializeField] CharacterController characterController;
    [SerializeField] private float speed;
    [SerializeField] private float interactableRadius;
    [SerializeField] private LayerMask enemyLayer;

    [Header("Weapon Information")]
    [SerializeField] private WeaponSO currentWeapon;
    [SerializeField] private WeaponSO primaryWeapon;
    [SerializeField] private WeaponSO secondaryWeapon;

    [SerializeField] private int primaryBullet;

    [SerializeField] private float shootTimer;

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
        if (Input.GetKey(KeyCode.Tab))
        {

            if (currentWeapon == primaryWeapon)
            {
                currentWeapon = secondaryWeapon;
            }
            else if (currentWeapon == secondaryWeapon)
            {
                currentWeapon = primaryWeapon;
            }
        }
        if (shootTimer > 0)
        {
            shootTimer -= Time.deltaTime;
        }
    }
    public bool canShoot = true;
    private void Shooting()
    {
        if (currentWeapon.shootingMode == ShootingMode.Automiatic)
        {
            if (Input.GetMouseButton(0))
            {
                if (!canShoot) { return; }

                if (currentWeapon == primaryWeapon & primaryBullet <= 0) { return; }
                if (currentWeapon == primaryWeapon)
                {
                    primaryBullet -= 1;
                }
                canShoot = false;
                shootTimer = currentWeapon.shootingSpeed;
            }
        }
        else if (currentWeapon.shootingMode == ShootingMode.SemiAutomatic)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (!canShoot) { return; }

                if (currentWeapon == primaryWeapon & primaryBullet <= 0) { return; }
                if (currentWeapon == primaryWeapon)
                {
                    primaryBullet -= 1;
                }

                for (int i = 0; i < currentWeapon.projectileAmount; i++)
                {
                    float angle = UnityEngine.Random.Range(-currentWeapon.scatter, currentWeapon.scatter);
                    Quaternion angleRotation = Quaternion.AngleAxis(angle, Vector3.up);

                    Ray fireRay = new Ray(transform.position + Vector3.up, angleRotation * transform.forward);
                    RaycastHit enemyInfo;

                    Debug.DrawRay(fireRay.origin, fireRay.direction * 10, Color.green, 10f);

                    if (Physics.Raycast(fireRay.origin, fireRay.direction, out enemyInfo, 100f, enemyLayer))
                    {
                        Debug.Log(enemyInfo.transform.gameObject.name);
                    }
                    else
                    {
                        Debug.Log("Oye bro, no le pegaste");
                    }
                }
                canShoot = false;
                StartCoroutine(ResetAttack());
            }

        }

    }
    private IEnumerator ResetAttack()
    {
        yield return new WaitForSeconds(currentWeapon.shootingSpeed);
        canShoot = true;
    }

    private void Looking()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;

        if (Physics.Raycast(ray, out hitInfo, 100f))
        {
            Vector3 position = hitInfo.point;

            position.y = transform.position.y;

            transform.LookAt(position);
        }
    }

    private void Movement()
    {
        Vector3 movementVector = Vector2.zero;

        movementVector.x = Input.GetAxisRaw("Horizontal");
        movementVector.y = 0;
        movementVector.z = Input.GetAxisRaw("Vertical");

        characterController.Move(movementVector * Time.deltaTime * speed);
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position + transform.forward, interactableRadius);
    }
}
