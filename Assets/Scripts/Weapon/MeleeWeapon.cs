using UnityEngine;
using System.Collections; 

public class MeleeWeapon : Weapon
{
    [Header("Melee Settings")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRange = 0.8f;
    [SerializeField] private float damageDelay = 0.1f;

    public override bool TryAttack()
    {
        if (Time.time < nextAttackTime) return false;

        nextAttackTime = Time.time + fireRate;

        StartCoroutine(DelayedAttack());

        return true; 
    }

    private IEnumerator DelayedAttack()
    {
        yield return new WaitForSeconds(damageDelay);

        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(attackPoint.position, attackRange);

        foreach (var hit in hitColliders)
        {
            if (hit.CompareTag("Player")) continue;

            if (hit.isTrigger) continue;

            if (hit.TryGetComponent(out IDamageable damageable))
            {
                damageable.TakeDamage(damage);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint) Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}