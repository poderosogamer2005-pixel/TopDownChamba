using UnityEngine;

public class EnemyReward : MonoBehaviour
{
    [Header("Recompensa")]
    [SerializeField] private GameObject objetoRecogible;
    [SerializeField] private Transform puntoDrop;

    private bool recompensaSoltada;

    public void SoltarRecompensa()
    {
        if (recompensaSoltada)
            return;

        recompensaSoltada = true;

        if (objetoRecogible == null)
            return;

        Vector3 posicion = puntoDrop != null
            ? puntoDrop.position
            : transform.position + Vector3.up * 0.5f;

        Instantiate(objetoRecogible, posicion, Quaternion.identity);
    }
}
