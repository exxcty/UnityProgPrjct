using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System; // Нужно для Action

public class PlayerWeaponController : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Animator animator;
    [SerializeField] private PlayerController playerController;

    [Header("Weapons Settings")]
    [SerializeField] private List<Weapon> weapons; 

    public event Action<int, int> OnAmmoChanged;

    private Dictionary<int, int> ammoInventory = new Dictionary<int, int>();
    private int currentWeaponIndex = 0;
    private GameControls controls; 

    private void Awake()
    {
        controls = new GameControls();
    }

    private void OnEnable() => controls.Enable();
    private void OnDisable() => controls.Disable();

    private void Start()
    {
        ammoInventory[0] = 9999; 
        ammoInventory[1] = 0;    
        ammoInventory[2] = 0;   

        foreach (var w in weapons)
        {
            if (w is RangedWeapon rw) rw.Initialize(this);
            w.gameObject.SetActive(false);
        }
        EquipWeapon(0);
    }

    private void Update()
    {
        HandleWeaponSwitch();
        HandleAttack();
        UpdateAnimations();
    }

    private void HandleWeaponSwitch()
    {
        if (controls.Player.Weapon1.triggered) EquipWeapon(0);
        if (controls.Player.Weapon2.triggered) EquipWeapon(1);
        if (controls.Player.Weapon3.triggered) EquipWeapon(2);
    }

    private void HandleAttack()
    {
        Weapon currentWp = weapons[currentWeaponIndex];
        bool attackInput = false;

        if (currentWp.weaponID == 2)
        {
            attackInput = controls.Player.Fire.IsPressed();
        }
        else
        {
            attackInput = controls.Player.Fire.triggered;
        }

        if (attackInput)
        {
            if (currentWp.TryAttack())
            {
                animator.SetTrigger("Attack");
            }
        }
    }

    private void UpdateAnimations()
    {
        bool isMoving = playerController.IsMoving;
        animator.SetBool("IsMoving", isMoving);
    }

    private void EquipWeapon(int index)
    {
        if (index < 0 || index >= weapons.Count) return;

        weapons[currentWeaponIndex].gameObject.SetActive(false);
        currentWeaponIndex = index;
        weapons[currentWeaponIndex].gameObject.SetActive(true);

        animator.SetInteger("WeaponID", weapons[currentWeaponIndex].weaponID);
    }

    public bool TryConsumeAmmo(int weaponID)
    {
        if (ammoInventory.ContainsKey(weaponID) && ammoInventory[weaponID] > 0)
        {
            ammoInventory[weaponID]--;

            OnAmmoChanged?.Invoke(weaponID, ammoInventory[weaponID]);

            return true;
        }
        return false;
    }

    public void AddAmmo(int weaponID, int amount)
    {
        if (ammoInventory.ContainsKey(weaponID))
        {
            ammoInventory[weaponID] += amount;

            OnAmmoChanged?.Invoke(weaponID, ammoInventory[weaponID]);
        }
    }

    public int GetAmmoCount(int weaponID)
    {
        if (ammoInventory.ContainsKey(weaponID)) return ammoInventory[weaponID];
        return 0;
    }
}