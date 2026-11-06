using InventoryFramework;
using UnityEngine;

public class PotionUse : MonoBehaviour
{
    public Item potionItem;          // drag the Potion asset here
    public float healAmount = 25f;
    public float cooldown = 1f;
    public HotbarUI hotbarUI;        // auto-found if left empty

    Health health;
    float nextUse;

    void Start()
    {
        health = GetComponent<Health>();
        if (hotbarUI == null) hotbarUI = FindFirstObjectByType<HotbarUI>();
    }

    void Update()
    {
        if (health == null || health.IsDead || BackpackToggle.IsOpen) return;
        if (Cursor.lockState != CursorLockMode.Locked) return;
        if (!Input.GetMouseButtonDown(0) || Time.time < nextUse) return;
        if (hotbarUI.SelectedItem != potionItem) return;

        if (health.Heal(healAmount))      // false if already at full health
        {
            hotbarUI.ConsumeSelected();
            nextUse = Time.time + cooldown;
        }
    }
}