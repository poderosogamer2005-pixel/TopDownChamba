using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController3 : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private CharacterController characterController;
    [SerializeField] private float speed = 5f;

    [Header("Interacción")]
    [SerializeField] private float interactableRadius = 2f;
    [SerializeField] private LayerMask enemyLayer;

    [Header("Inventario")]
    [SerializeField]
    private List<ItemSO> inventario =
        new List<ItemSO>();

    [SerializeField] private WeaponSO currentWeapon;
    [SerializeField] private WeaponSO primaryWeapon;
    [SerializeField] private WeaponSO secondaryWeapon;

    [Header("Munición")]
    [SerializeField] private int primaryBullet = 30;

    [Header("Vida y recursos")]
    [SerializeField] private int vidaBase = 100;
    [SerializeField] private int vidaMaxima = 100;
    [SerializeField] private int recursos = 0;

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
        Movement();
        Looking();

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            CambiarArma();
        }

        if (shootTimer > 0f)
            shootTimer -= Time.deltaTime;

        Shooting();
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
            float angle = Random.Range(-currentWeapon.scatter, currentWeapon.scatter);

            Quaternion angleRotation =
                Quaternion.AngleAxis(angle, Vector3.up);

            Vector3 origen = transform.position + Vector3.up;
            Vector3 direccion =
                angleRotation * transform.forward;

            Ray fireRay = new Ray(origen, direccion);

            Debug.DrawRay( origen,direccion * 100f, Color.green, 1f);

            // Esta parte es enemyLayer para detectar a los enemigos y cajas.
            if (Physics.Raycast(fireRay,out RaycastHit hit, 100f,~0, QueryTriggerInteraction.Ignore))
            {
                Debug.Log("Impacto contra: " + hit.collider.name);

                // Daño a los enemigos que les dispares
                Enemy3 enemigo = hit.collider.GetComponentInParent<Enemy3>();

                if (enemigo != null)
                {
                    enemigo.RecibirDanio(Mathf.RoundToInt(currentWeapon.dano));

                    continue;
                }

                // Daño a cajas destruibles que les dispares
                Caja caja = hit.collider.GetComponentInParent<Caja>();

                if (caja != null)
                {
                    caja.RecibirDanio(
                        Mathf.RoundToInt(currentWeapon.dano));

                    continue;
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

        characterController.Move(movementVector * Time.deltaTime * speed );
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;

        Gizmos.DrawWireSphere(transform.position + transform.forward, interactableRadius);
    }
}