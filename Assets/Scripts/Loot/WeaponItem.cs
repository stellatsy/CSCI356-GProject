using InventoryFramework;
using UnityEngine;

[CreateAssetMenu(menuName = "Loot Lockdown/Weapon Item")]
public class WeaponItem : Item
{
    public float damage = 20f, fireRate = 4f, range = 50f;
    public bool automatic = true;
    public int magSize = 30;
    public bool unlimitedReserve = false;
    public int reserveAmmo = 90;
    public float reloadTime = 1.5f;

    [Header("Audio")]
    public AudioClip shotClip, reloadClip, emptyClip;
    [Range(0f, 1f)] public float shotVolume = 1f;
    [Range(0f, 1f)] public float reloadVolume = 1f;
}