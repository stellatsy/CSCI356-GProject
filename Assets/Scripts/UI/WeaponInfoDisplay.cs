using InventoryFramework;
using TMPro;
using UnityEngine;

public class WeaponInfoDisplay : MonoBehaviour
{
    TextMeshProUGUI label;
    HotbarUI hotbarUI;

    void Start()
    {
        label = GetComponent<TextMeshProUGUI>();
        hotbarUI = FindFirstObjectByType<HotbarUI>();
    }

    void Update()
    {
        var w = hotbarUI != null ? hotbarUI.SelectedItem as WeaponItem : null;
        var wp = WeaponProgress.Instance;
        if (w == null || wp == null) { label.text = ""; return; }

        label.text = wp.IsTracked(w)
            ? w.itemName + "  Lv " + wp.Level(w) + "   DMG " + wp.Damage(w).ToString("0.#")
            : w.itemName + "   DMG " + w.damage.ToString("0.#");
    }
}