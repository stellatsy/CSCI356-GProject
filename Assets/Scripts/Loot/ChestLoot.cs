using System.Collections;
using System.Collections.Generic;
using InventoryFramework;
using UnityEngine;

public class ChestLoot : MonoBehaviour
{
    public static readonly HashSet<ChestLoot> Unopened = new();

    public Item[] lootPool;                  // a random item is picked from here
    public int itemCount = 1;                // ordinary = 1, elite = 2
    public Item guaranteedFirst;             // the SMG: only the very first chest gives it
    public bool guaranteeWeapon;             // elite chest: the first drop is always a weapon
    public string openParameter = "Open";    // Animator trigger name
    public float interactRange = 2.5f;
    public float giveDelay = 0.8f;           // wait for the lid to open
    public float disappearAfter = 3f;        // seconds the open chest stays
    public float shrinkTime = 0.5f;          // how long it takes to shrink away

    bool opened;
    Animator animator;
    Transform player;

    void OnEnable() { Unopened.Add(this); }
    void OnDisable() { Unopened.Remove(this); InteractPrompt.Hide(this); }

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        var p = GameObject.FindWithTag("Player");
        if (p != null) player = p.transform;
    }

    void Update()
    {
        if (opened || player == null) return;

        bool near = Vector3.Distance(transform.position, player.position) <= interactRange;
        if (near && !BackpackToggle.IsOpen)
        {
            InteractPrompt.Show(this, "Press E to open");
            if (Input.GetKeyDown(KeyCode.E)) StartCoroutine(Open());
        }
        else InteractPrompt.Hide(this);
    }

    IEnumerator Open()
    {
        opened = true;
        AudioManager.Chest();
        Unopened.Remove(this);
        InteractPrompt.Hide(this);

        if (animator != null)
            foreach (var p in animator.parameters)
                if (p.name == openParameter) { animator.SetTrigger(openParameter); break; }

        yield return new WaitForSeconds(giveDelay);

        var gm = GameManager.Instance;
        bool gaveFirst = false;
        
        if (gm != null && !gm.smgGiven && guaranteedFirst != null)
        {
            gm.smgGiven = true;
            WeaponProgress.Instance.Give(guaranteedFirst);
            gaveFirst = true;
        }

        int remaining = gaveFirst ? itemCount - 1 : itemCount;
        for (int i = 0; i < remaining && lootPool.Length > 0; i++)
        {
            Item drop = null;
            if (i == 0 && guaranteeWeapon) drop = PickWeapon();
            if (drop == null) drop = PickLoot();
            if (drop != null) WeaponProgress.Instance.Give(drop);
        }

        // the chest stays open for a while, then shrinks away
        yield return new WaitForSeconds(disappearAfter);
        yield return Shrink();
        gameObject.SetActive(false);
    }

    // Picks a random item, skipping weapons already at max level
    Item PickLoot()
    {
        var wp = WeaponProgress.Instance;
        for (int tries = 0; tries < 10; tries++)
        {
            var item = lootPool[Random.Range(0, lootPool.Length)];
            if (item is WeaponItem w && wp != null && wp.IsMaxed(w)) continue;
            return item;
        }
        // everything rolled was a maxed weapon: fall back to the first non-weapon item
        foreach (var item in lootPool)
            if (!(item is WeaponItem)) return item;
        return null;
    }

    Item PickWeapon()
    {
        var wp = WeaponProgress.Instance;
        var options = new List<Item>();
        foreach (var item in lootPool)
            if (item is WeaponItem w && (wp == null || !wp.IsMaxed(w))) options.Add(item);
        return options.Count > 0 ? options[Random.Range(0, options.Count)] : null;
    }

    IEnumerator Shrink()
    {
        Vector3 start = transform.localScale;
        for (float t = 0; t < shrinkTime; t += Time.deltaTime)
        {
            transform.localScale = Vector3.Lerp(start, Vector3.zero, t / shrinkTime);
            yield return null;
        }
    }
}