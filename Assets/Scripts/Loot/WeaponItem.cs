using InventoryFramework;
using UnityEngine;

[CreateAssetMenu(menuName = "Loot Lockdown/Weapon Item")]
public class WeaponItem : Item
{
    [Header("Level 1 stats")]
    public float damage = 20f;
    public float fireRate = 4f;      // shots per second
    public float range = 50f;
    public bool automatic = true;    // hold to fire

    [Header("Ammo")]
    public int magSize = 30;         // bullets per magazine
    public bool unlimitedReserve = false;
    public int reserveAmmo = 90;     // ignored if unlimitedReserve is on
    public float reloadTime = 1.5f;
}