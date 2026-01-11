using UnityEngine;

public class RangedWeapon : Weapon
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float bulletSpeed = 20f;

    private PlayerWeaponController controller;

    public void Initialize(PlayerWeaponController weaponController)
    {
        controller = weaponController;
    }

    public override bool TryAttack()
    {
        if (Time.time < nextAttackTime) return false;

        if (!controller.TryConsumeAmmo(weaponID))
        {
            return false;
        }

        nextAttackTime = Time.time + fireRate;

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();

        rb.linearVelocity = firePoint.right * bulletSpeed;

        bullet.GetComponent<Bullet>().damage = damage;

        return true;
    }
}