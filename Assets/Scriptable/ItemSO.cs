using UnityEngine;


public enum ItemType
{
    Arma,
    Equipamiento
}

[CreateAssetMenu(fileName = "NuevoItem", menuName = "Inventario/Item")]
public class ItemSO : ScriptableObject
{
    [Header("Información del objeto")]
    public string nombre;
    public ItemType tipo;

    [Header("Estadísticas")]
    public float dano;
    public float velocidadAtaque;
    public int vidaExtra;
}
