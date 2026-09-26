using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private int health = 100;

    //el enemigo recibe daño
    public void TakeDamage(int damage)
    {
        health -= damage;

        Debug.Log("Enemigo recibió " + damage + " de daño.");

        if (health <= 0)
        {
            Die();
        }
    }
    private void Die()
    {
        Destroy(gameObject);
    }
}
