using System.Collections.Generic;
using InventoryFramework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeHint : MonoBehaviour
{
    public KeyCode upgradeKey = KeyCode.U;
    public Color glowColor = new Color(1f, 0.75f, 0.1f);

    class Marks { public Image glow; public TextMeshProUGUI arrow, label; }

    readonly Dictionary<InventorySlotUI, Marks> marks = new Dictionary<InventorySlotUI, Marks>();
    InventorySlotUI[] slots = new InventorySlotUI[0];
    HotbarUI hotbarUI;
    Health playerHealth;
    float nextFind;

    void Start()
    {
        hotbarUI = FindFirstObjectByType<HotbarUI>();
        var p = GameObject.FindWithTag("Player");
        if (p != null) playerHealth = p.GetComponent<Health>();
    }

    void Update()
    {
        var wp = WeaponProgress.Instance;
        if (wp == null) return;

        if (Time.unscaledTime > nextFind)
        {
            slots = FindObjectsByType<InventorySlotUI>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            nextFind = Time.unscaledTime + 0.5f;
        }

        float pulse = 0.2f + 0.25f * Mathf.PingPong(Time.unscaledTime * 2f, 1f);

        foreach (var slot in slots)
        {
            if (slot == null) continue;

            WeaponItem w = null;
            try
            {
                var s = slot.GetSlot();
                if (s != null && !s.IsEmpty) w = s.item as WeaponItem;
            }
            catch { }

            bool tracked = w != null && wp.IsTracked(w);
            if (!tracked && !marks.ContainsKey(slot)) continue;
            if (tracked && !marks.ContainsKey(slot)) marks[slot] = Create(slot);

            var m = marks[slot];
            bool ready = tracked && wp.CanUpgrade(w);

            m.label.gameObject.SetActive(tracked);
            m.glow.gameObject.SetActive(ready);
            m.arrow.gameObject.SetActive(ready);

            if (tracked)
            {
                bool max = wp.Level(w) >= wp.maxLevel;
                m.label.text = max ? "Lv" + wp.Level(w) + " MAX"
                                   : "Lv" + wp.Level(w) + " " + wp.Progress(w) + "/" + wp.copiesPerLevel;
            }
            if (ready)
            {
                var c = glowColor; c.a = pulse;
                m.glow.color = c;
            }
        }

        // Press U to upgrade the weapon in your hand
        if (hotbarUI == null || (playerHealth != null && playerHealth.IsDead)) return;
        var held = hotbarUI.SelectedItem as WeaponItem;
        bool canUpgrade = held != null && wp.CanUpgrade(held) && !BackpackToggle.IsOpen;

        if (canUpgrade) InteractPrompt.Show(this, "Press " + upgradeKey + " to upgrade " + held.itemName);
        else InteractPrompt.Hide(this);

        if (held != null && Input.GetKeyDown(upgradeKey))
        {
            float before = wp.Damage(held);
            if (wp.Upgrade(held))
            {
                AudioManager.Upgrade();
                PickupFeed.Show(held.itemName + " upgraded to Lv " + wp.Level(held) +
                                "!  Damage " + before.ToString("0.#") + " -> " + wp.Damage(held).ToString("0.#"));
            }
        }
    }

    Marks Create(InventorySlotUI slot)
    {
        var parent = slot.icon.transform.parent;
        var iconRT = slot.icon.rectTransform;
        var bgRT = slot.transform.GetChild(0) as RectTransform;   // the slot's Background image

        RectTransform Make(string n, RectTransform src)
        {
            var go = new GameObject(n, typeof(RectTransform));
            var rt = go.GetComponent<RectTransform>();
            rt.SetParent(parent, false);
            rt.anchorMin = src.anchorMin; rt.anchorMax = src.anchorMax;
            rt.pivot = src.pivot;
            rt.anchoredPosition = src.anchoredPosition;
            rt.sizeDelta = src.sizeDelta;
            return rt;
        }

        var m = new Marks();

        var glowRT = Make("UpgradeGlow", bgRT);
        glowRT.SetSiblingIndex(1);                 // behind the gun icon
        m.glow = glowRT.gameObject.AddComponent<Image>();
        m.glow.raycastTarget = false;

        m.arrow = Make("UpgradeArrow", iconRT).gameObject.AddComponent<TextMeshProUGUI>();
        m.arrow.text = "^";
        m.arrow.fontSize = 36;
        m.arrow.fontStyle = FontStyles.Bold;
        m.arrow.alignment = TextAlignmentOptions.TopRight;
        m.arrow.color = glowColor;
        m.arrow.raycastTarget = false;

        m.label = Make("LevelLabel", iconRT).gameObject.AddComponent<TextMeshProUGUI>();
        m.label.fontSize = 16;
        m.label.alignment = TextAlignmentOptions.TopLeft;
        m.label.color = Color.white;
        m.label.raycastTarget = false;

        return m;
    }
}