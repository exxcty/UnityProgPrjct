using UnityEngine;
using System;

public class Health : MonoBehaviour, IDamageable
{
    [Header("Stats")]
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;

    [Header("Effects")]
    [SerializeField] private GameObject hitEffectPrefab; 

    public event Action<float> OnHealthChanged;
    public event Action OnDeath;

    public float CurrentHealth => currentHealth;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        if (currentHealth <= 0) return;

        currentHealth -= amount;

        if (hitEffectPrefab != null)
        {
            Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
        }

        OnHealthChanged?.Invoke(currentHealth / maxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        OnHealthChanged?.Invoke(currentHealth / maxHealth);
    }

    public void SetMaxHealth(float amount)
    {
        maxHealth = amount;
        currentHealth = amount;
    }

    private void Die()
    {
        OnDeath?.Invoke();
    }
}