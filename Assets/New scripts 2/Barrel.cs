using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Barrel : MonoBehaviour, IDamagable
{
    [field: SerializeField] public int Health { get; set; }
    [field: SerializeField] public LayerMask hitBy { get; set; }
    [SerializeField] private ParticleSystem explosion;

    public void Die()
    {
        Instantiate(explosion, transform.position, Quaternion.identity);
        List<Collider> explosionHit = Physics.OverlapSphere(transform.position, 3f).ToList();
        foreach (Collider colliders in explosionHit)
        {
            if (colliders.TryGetComponent<IDamagable>(out IDamagable damagable))
            {
                damagable.TryDamage(1, 6);
                damagable.TryDamage(1, 6);
            }
        }
        gameObject.SetActive(false);
    }
}
