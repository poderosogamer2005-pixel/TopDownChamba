using UnityEngine;

public class Enemy3 : MonoBehaviour
{
    [SerializeField] private int health = 100;

    public void RecibirDanio(int danio)
    {
        health -= danio;

        Debug.Log("Vida restante del enemigo: " + health);

        if (health <= 0)
        {
            EnemyItemDrop drop = GetComponent<EnemyItemDrop>();

            if (drop != null)
            {
                drop.Morir();
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
