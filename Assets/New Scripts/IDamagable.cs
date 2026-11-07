using UnityEngine;

public interface IDamagable
{
    public int Health { get; set; }
    public LayerMask hitBy { get; set; }

    public virtual void TryDamage(int newDamage, LayerMask attacker)
    {
        if ((hitBy.value & attacker.value) != 0) { return; }
        if (Health <= 0) { return; }

        Health -= newDamage;
        if (Health <= 0)
        {
            Die();
        }
    }
    public abstract void Die();
}