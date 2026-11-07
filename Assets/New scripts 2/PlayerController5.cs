using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController5 : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private CharacterController characterController;
    [SerializeField] private float speed = 5f;

    [Header("Interacción")]
    [SerializeField] private float interactableRadius = 2f;
    [SerializeField] private LayerMask enemyLayer;

    [Header("Inventario")]
    [SerializeField]
    private List<ItemSO> inventario = new List<ItemSO>();

    [SerializeField] private WeaponSO currentWeapon;
    [SerializeField] private WeaponSO primaryWeapon;
    [SerializeField] private WeaponSO secondaryWeapon;

    [Header("Munición")]
    [SerializeField] private int primaryBullet = 30;

    [Header("Vida y recursos")]
    [SerializeField] private int vidaBase = 100;
    [SerializeField] private int vidaMaxima = 100;
    [SerializeField] private int recursos = 0;

    [Header("Daño recibido")]
    [SerializeField] private bool jugadorDerrotado = false;

    [Header("Disparo")]
    [SerializeField] private float shootTimer;

    private readonly List<WeaponSO> armas = new List<WeaponSO>();

    private int indiceArma = -1;

    public int Recursos => recursos;
    public int VidaBase => vidaBase;
    public int VidaMaxima => vidaMaxima;
    public WeaponSO ArmaActual => currentWeapon;
    public IReadOnlyList<ItemSO> Inventario => inventario;

    private void Awake()
    {
        if (characterController == null)
            characterController = GetComponent<CharacterController>();

        if (primaryWeapon != null)
            AgregarObjeto(primaryWeapon);

        if (secondaryWeapon != null)
            AgregarObjeto(secondaryWeapon);

        if (currentWeapon != null)
            AgregarObjeto(currentWeapon);

        ActualizarArmas();

        if (currentWeapon != null)
            indiceArma = armas.IndexOf(currentWeapon);

        if (indiceArma < 0 && armas.Count > 0)
        {
            indiceArma = 0;
            currentWeapon = armas[0];
        }
    }

    private void Update()
    {
        if (jugadorDerrotado)
            return;

        Movement();
        Looking();

        IInteractable closestInteractable = null;

        List<Collider> interactableColliders =
            Physics.OverlapSphere(
                transform.position + transform.forward,
                interactableRadius
            ).ToList();

        float closestDistance = Mathf.Infinity;

        foreach (Collider collider in interactableColliders)
        {
            if (collider.transform.TryGetComponent<IInteractable>(
                out IInteractable interactable))
            {
                float distance = Vector3.Distance(
                    collider.transform.position,
                    transform.position
                );

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

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            CambiarArma();
        }

        if (shootTimer > 0f)
            shootTimer -= Time.deltaTime;

        Shooting();
    }

    // Recibir daño de los enemigos.
    public void RecibirDanio(int cantidad)
    {
        if (jugadorDerrotado || cantidad <= 0)
            return;

        vidaBase = Mathf.Max(vidaBase - cantidad, 0);

        Debug.Log(
            "El jugador recibió " + cantidad +
            " de daño. Vida restante: " + vidaBase
        );

        if (vidaBase <= 0)
        {
            jugadorDerrotado = true;

            Debug.Log("El jugador ha sido derrotado.");

            Destroy(gameObject);
        }
    }

    private void Shooting()
    {
        if (currentWeapon == null)
            return;

        if (shootTimer > 0f)
            return;

        if (currentWeapon.shootingMode == ShootingMode.Automiatic)
        {
            if (Input.GetMouseButton(0))
                Disparar();
        }
        else if (currentWeapon.shootingMode ==
                 ShootingMode.SemiAutomatic)
        {
            if (Input.GetMouseButtonDown(0))
                Disparar();
        }
    }

    private void Disparar()
    {
        if (currentWeapon == null)
            return;

        if (currentWeapon == primaryWeapon)
        {
            if (primaryBullet <= 0)
            {
                Debug.Log("No tienes munición.");
                return;
            }

            primaryBullet--;
        }

        for (int i = 0; i < currentWeapon.projectileAmount; i++)
        {
            float angle = Random.Range(
                -currentWeapon.scatter,
                currentWeapon.scatter
            );

            Quaternion angleRotation =
                Quaternion.AngleAxis(angle, Vector3.up);

            Vector3 origen = transform.position + Vector3.up;
            Vector3 direccion = angleRotation * transform.forward;

            Ray fireRay = new Ray(origen, direccion);

            RaycastHit enemyInfo;

            Debug.DrawRay(
                origen,
                direccion * 100f,
                Color.green,
                1f
            );

            if (Physics.Raycast(
                fireRay,
                out RaycastHit hit,
                100f,
                ~0,
                QueryTriggerInteraction.Ignore))
            {
                Debug.Log("Impacto contra: " + hit.collider.name);

                Enemy5 enemigo =
                    hit.collider.GetComponentInParent<Enemy5>();

                if (enemigo != null)
                {
                    enemigo.RecibirDanio(
                        Mathf.RoundToInt(currentWeapon.dano)
                    );

                    continue;
                }

                Caja caja = hit.collider.GetComponentInParent<Caja>();

                if (caja != null)
                {
                    caja.RecibirDanio(
                        Mathf.RoundToInt(currentWeapon.dano)
                    );

                    continue;
                }

                if (Physics.Raycast(
                    fireRay.origin,
                    fireRay.direction,
                    out enemyInfo,
                    100f,
                    enemyLayer))
                {
                    if (enemyInfo.transform.gameObject.TryGetComponent<IDamagable>(
                        out IDamagable damagable))
                    {
                        damagable.TryDamage(1, 6);
                    }

                    Debug.Log(enemyInfo.transform.gameObject.name);
                }
                else
                {
                    Debug.Log("Oye bro, no le pegaste");
                }

                Debug.Log("El objeto no tiene un receptor de daño.");
            }
            else
            {
                Debug.Log("El disparo no golpeó ningún objeto.");
            }
        }

        shootTimer = currentWeapon.shootingSpeed;
    }

    private void CambiarArma()
    {
        ActualizarArmas();

        if (armas.Count <= 1)
            return;

        indiceArma = (indiceArma + 1) % armas.Count;
        currentWeapon = armas[indiceArma];

        Debug.Log("Arma equipada: " + currentWeapon.nombre);
    }

    private void ActualizarArmas()
    {
        armas.Clear();

        foreach (ItemSO item in inventario)
        {
            if (item is WeaponSO arma && !armas.Contains(arma))
                armas.Add(arma);
        }

        indiceArma = currentWeapon != null
            ? armas.IndexOf(currentWeapon)
            : -1;
    }

    public void AgregarObjeto(ItemSO objeto)
    {
        if (objeto == null)
            return;

        if (inventario.Contains(objeto))
        {
            Debug.Log("Ya tienes este objeto: " + objeto.nombre);
            return;
        }

        inventario.Add(objeto);

        if (objeto is WeaponSO nuevaArma &&
            objeto.tipo == ItemType.Arma)
        {
            ActualizarArmas();

            if (currentWeapon == null)
            {
                currentWeapon = nuevaArma;
                indiceArma = armas.IndexOf(nuevaArma);
            }
        }
        else if (objeto.tipo == ItemType.Equipamiento)
        {
            Debug.Log("Equipamiento guardado: " + objeto.nombre);
        }

        Debug.Log("Objeto obtenido: " + objeto.nombre);
    }

    public void AgregarRecursos(int cantidad)
    {
        if (cantidad <= 0)
            return;

        recursos += cantidad;

        Debug.Log("Recursos actuales: " + recursos);
    }

    public void GastarRecursos(int cantidad)
    {
        if (cantidad <= 0)
            return;

        if (recursos < cantidad)
        {
            Debug.Log("No tienes suficientes recursos.");
            return;
        }

        recursos -= cantidad;

        Debug.Log("Recursos restantes: " + recursos);
    }

    public void AgregarVidaMaxima(int cantidad)
    {
        if (cantidad <= 0)
            return;

        vidaMaxima += cantidad;
        vidaBase += cantidad;

        Debug.Log("Vida máxima: " + vidaMaxima);
    }

    private void Looking()
    {
        if (Camera.main == null)
            return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hitInfo, 100f))
        {
            Vector3 position = hitInfo.point;

            position.y = transform.position.y;

            transform.LookAt(position);
        }
    }

    private void Movement()
    {
        Vector3 movementVector = Vector3.zero;

        movementVector.x = Input.GetAxisRaw("Horizontal");
        movementVector.z = Input.GetAxisRaw("Vertical");

        movementVector = Vector3.ClampMagnitude(movementVector, 1f);

        characterController.Move(
            movementVector * Time.deltaTime * speed
        );
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
