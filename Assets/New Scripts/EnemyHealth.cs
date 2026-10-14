using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int vida = 100;
    [SerializeField] private EnemyReward recompensa;

    public void RecibirDanio(int cantidad)
    {
        if (vida <= 0)
            return;

        vida -= cantidad;

        Debug.Log("Vida del enemigo: " + vida);

        if (vida <= 0)
        {
            Morir();
        }
    }

    private void Morir()
    {
        if (recompensa != null)
            recompensa.SoltarRecompensa();

        Destroy(gameObject);
    }
}
