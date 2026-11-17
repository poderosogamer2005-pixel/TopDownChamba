using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private GameObject panelInventario;
    [SerializeField] private Transform contenedorObjetos;
    [SerializeField] private GameObject prefabObjeto;

    [Header("Jugador")]
    [SerializeField] private PlayerController5 jugador;

    public bool MenuAbierto => inventarioAbierto;

    private bool inventarioAbierto;

    private void Start()
    {
        panelInventario.SetActive(false);
        inventarioAbierto = false;

        ActualizarInventario();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            AlternarInventario();
        }
    }

    public void AlternarInventario()
    {
        inventarioAbierto = !inventarioAbierto;

        panelInventario.SetActive(inventarioAbierto);

        if (inventarioAbierto)
        {
            ActualizarInventario();
        }
    }

    public void ActualizarInventario()
    {
        if (jugador == null)
            return;

        // Eliminar los objetos visuales anteriores.
        foreach (Transform hijo in contenedorObjetos)
        {
            Destroy(hijo.gameObject);
        }

        // Crear un elemento visual por cada objeto.
        foreach (ItemSO objeto in jugador.Inventario)
        {
            GameObject nuevoObjeto = Instantiate(
                prefabObjeto,
                contenedorObjetos
            );

            InventoryItemUI itemUI =
                nuevoObjeto.GetComponent<InventoryItemUI>();

            if (itemUI != null)
            {
                itemUI.Configurar(objeto, jugador);
            }
        }
    }
}
