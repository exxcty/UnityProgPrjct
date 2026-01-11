using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Links")]
    [SerializeField] private Health playerHealth;
    [SerializeField] private PlayerWeaponController playerWeapon;

    [Header("UI Elements")]
    [SerializeField] private Image hpBar;
    [SerializeField] private TextMeshProUGUI hpText;        
    [SerializeField] private TextMeshProUGUI pistolAmmoText; 
    [SerializeField] private TextMeshProUGUI rifleAmmoText; 
    [SerializeField] private TextMeshProUGUI scoreText;    

    private void Start()
    {
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged += UpdateHPUI;
            playerHealth.OnDeath += GameManager.Instance.EndGame;
            UpdateHPUI(1f);
        }

        if (playerWeapon != null)
        {
            playerWeapon.OnAmmoChanged += UpdateAmmoUI;

            UpdateAmmoUI(1, playerWeapon.GetAmmoCount(1)); 
            UpdateAmmoUI(2, playerWeapon.GetAmmoCount(2));
        }
    }

    private void Update()
    {
        if (GameManager.Instance != null && scoreText != null)
        {
            scoreText.text = "Ñ÷¸ò: " + GameManager.Instance.Score;
        }
    }

    private void UpdateHPUI(float pct) 
    {
        if (hpBar != null)
            hpBar.fillAmount = pct;

        if (hpText != null && playerHealth != null)
        {
            float current = playerHealth.CurrentHealth;
            hpText.text = $"{Mathf.CeilToInt(current)}";
        }
    }

    private void UpdateAmmoUI(int weaponID, int amount)
    {
        if (weaponID == 1 && pistolAmmoText != null)
        {
            pistolAmmoText.text = amount.ToString();
        }
        else if (weaponID == 2 && rifleAmmoText != null)
        {
            rifleAmmoText.text = amount.ToString();
        }
    }
}