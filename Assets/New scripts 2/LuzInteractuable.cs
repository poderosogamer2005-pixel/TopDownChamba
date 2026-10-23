using UnityEngine;

public class LuzInteractuable : MonoBehaviour, IInteractable
{
    [Header("Luz que se va a controlar")]
    [SerializeField] private Light luz;

    [Header("Estado inicial")]
    [SerializeField] private bool luzEncendida = false;

    private void Start()
    {
        if (luz != null)
        {
            luz.enabled = luzEncendida;
        }
    }

    public void Interact()
    {
        if (luz == null)
        {
            Debug.LogWarning("No se ha asignado una luz al objeto.");
            return;
        }

        luzEncendida = !luzEncendida;
        luz.enabled = luzEncendida;

        Debug.Log(luzEncendida? "La luz se ha encendido.": "La luz se ha apagado.");
    }
}
