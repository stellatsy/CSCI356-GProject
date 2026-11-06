using System.Collections.Generic;
using InventoryFramework;
using UnityEngine;

public class WeaponProgress : MonoBehaviour
{
    public static WeaponProgress Instance;

    public int copiesPerLevel = 3;        // total copies: Lv2 = 3, Lv3 = 6
    public int maxLevel = 3;
    public float damageBonusPerLevel = 0.25f;
    public WeaponItem testWeapon;         // only for the test menu below

    readonly Dictionary<WeaponItem, int> copies = new Dictionary<WeaponItem, int>();
    readonly Dictionary<WeaponItem, int> levels = new Dictionary<WeaponItem, int>();

    void Awake() { Instance = this; }

    // Chests call this instead of LootInventory.Give
    public void Give(Item item)
    {
        if (item is WeaponItem w)
        {
            if (copies.ContainsKey(w))
            {
                copies[w]++;                           // extra copy: no new slot
                if (CanUpgrade(w))
                    PickupFeed.Show(w.itemName + " +1 copy (" + Progress(w) + "/" + copiesPerLevel + ") - ready! Press U");
                else if (Level(w) >= maxLevel)
                    PickupFeed.Show(w.itemName + " +1 copy (max level)");
                else
                    PickupFeed.Show(w.itemName + " +1 copy (" + Progress(w) + "/" + copiesPerLevel + ")");
                return;
            }
            copies[w] = 1;                             // first copy takes a slot
            levels[w] = 1;
        }
        PickupFeed.Show("Picked up " + item.itemName);
        LootInventory.Instance.Give(item, 1, true);
    }

    public bool IsTracked(WeaponItem w) => copies.ContainsKey(w);
    public int Copies(WeaponItem w) => copies.TryGetValue(w, out int c) ? c : 0;
    public int Level(WeaponItem w) => levels.TryGetValue(w, out int l) ? l : 1;
    public bool CanUpgrade(WeaponItem w) =>
        IsTracked(w) && Level(w) < maxLevel && Copies(w) >= Level(w) * copiesPerLevel;
    public int Progress(WeaponItem w) =>
        Mathf.Clamp(Copies(w) - (Level(w) - 1) * copiesPerLevel, 0, copiesPerLevel);
    public float Damage(WeaponItem w) => w.damage * (1f + damageBonusPerLevel * (Level(w) - 1));

    public bool Upgrade(WeaponItem w)
    {
        if (!CanUpgrade(w)) return false;
        levels[w] = Level(w) + 1;
        return true;
    }

    [ContextMenu("Test: give a copy of Test Weapon")]
    void TestGive() { if (testWeapon != null) Give(testWeapon); }
}