using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    public string weaponName;
    public int weaponID; 
    public float fireRate = 0.5f;
    public int damage = 10;

    protected float nextAttackTime;

    public abstract bool TryAttack();
}