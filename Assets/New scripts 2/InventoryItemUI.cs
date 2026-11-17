using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItemUI : MonoBehaviour
{
    [SerializeField] private Image imagenObjeto;
    [SerializeField] private TMP_Text nombreObjeto;

    private ItemSO objeto;
    private PlayerController5 jugador;

    public void Configurar(ItemSO nuevoObjeto, PlayerController5 nuevoJugador)
    {
        objeto = nuevoObjeto;
        jugador = nuevoJugador;

        if (imagenObjeto != null)
            imagenObjeto.sprite = objeto.icono;

        if (nombreObjeto != null)
            nombreObjeto.text = objeto.nombre;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("El puntero está sobre: " + objeto.nombre);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Clic detectado: " + eventData.button);

        if (eventData.button == PointerEventData.InputButton.Right)
        {
            Debug.Log("Clic derecho detectado.");

            if (jugador != null && objeto != null)
                jugador.EquiparObjeto(objeto);
        }
    }

    public void ClickDerecho()
    {
        if (jugador != null && objeto != null)
            jugador.EquiparObjeto(objeto);
    }

}
