using System.Collections;
using InventoryFramework;
using UnityEngine;

public class LootInventory : MonoBehaviour
{
    public static LootInventory Instance;
    public Item startingWeapon;   // Pistol, later
    public Item testItem;         // any item, for testing

    public bool lockStartingWeapon = true;

    Inventory inventory; Hotbar hotbar;
    InventoryUI inventoryUI; HotbarUI hotbarUI;

    void Awake()
    {
        Instance = this;
        inventory = GetComponent<Inventory>();
        hotbar = GetComponent<Hotbar>();
        inventoryUI = GetComponent<InventoryUI>();
        hotbarUI = GetComponent<HotbarUI>();
    }

    IEnumerator Start()
    {
        yield return null;                      // wait for the UI to build
        if (startingWeapon != null)
        {
            var s = hotbar.slots[0];            // hotbar slot 1, always
            s.item = startingWeapon;
            s.count = 1;
            inventoryUI.RefreshUI();
            hotbarUI.RefreshUI();
        }
    }

    public bool IsLocked(Item item)
    {
        return lockStartingWeapon && item != null && item == startingWeapon;
    }

    public bool Give(Item item, int amount = 1, bool preferHotbar = false)
    {
        bool ok = preferHotbar
            ? (hotbar.AddItem(item, amount) || inventory.AddItem(item, amount))
            : (inventory.AddItem(item, amount) || hotbar.AddItem(item, amount));
        inventoryUI.RefreshUI();
        hotbarUI.RefreshUI();
        return ok;
    }

    [ContextMenu("Give test item")]
    void TestGive() { if (testItem != null) Give(testItem, 1); }
}