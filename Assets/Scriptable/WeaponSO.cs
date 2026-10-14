using UnityEngine;

[CreateAssetMenu(fileName = "WeaponSO", menuName = "Inventario/Arma")]
public class WeaponSO : ItemSO
{
    [Header("Configuración de disparo")]
    public ShootingMode shootingMode;
    public int projectileAmount = 1;
    public float shootingSpeed = 0.5f;
    public float scatter;
}

public enum ShootingMode
{
    Automiatic,
    SemiAutomatic
}

