using UnityEngine;

public class Items: MonoBehaviour
{
    public enum ItemType
    {
        Health,
        SpeedBoost,
        PistolAmmo,
        RifleAmmo
    }

    [Header("Settings")]
    [SerializeField] private ItemType type;
    [SerializeField] private float amount;   
    [SerializeField] private float duration; 

    [Header("Visuals")]
    [SerializeField] private GameObject pickupEffect; 

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            ApplyEffect(other.gameObject);
        }
    }

    private void ApplyEffect(GameObject player)
    {
        bool success = false;

        switch (type)
        {
            case ItemType.Health:
                if (player.TryGetComponent(out Health health))
                {
                    health.Heal(amount);
                    success = true;
                }
                break;

            case ItemType.SpeedBoost:
                if (player.TryGetComponent(out PlayerController controller))
                {
                    controller.ApplySpeedBoost(amount, duration);
                    success = true;
                }
                break;

            case ItemType.PistolAmmo:
                {  
                    if (player.TryGetComponent(out PlayerWeaponController weapons))
                    {
                        weapons.AddAmmo(1, (int)amount);
                        success = true;
                    }
                    break;
                }  

            case ItemType.RifleAmmo:
                {  
                    if (player.TryGetComponent(out PlayerWeaponController weapons))
                    {
                        weapons.AddAmmo(2, (int)amount);
                        success = true;
                    }
                    break;
                } 
        }

        if (success)
        {
            if (pickupEffect != null)
            {
                Instantiate(pickupEffect, transform.position, Quaternion.identity);
            }
            Destroy(gameObject);
        }
    }
}