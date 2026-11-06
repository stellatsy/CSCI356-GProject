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
            WeaponProgress.Instance.Give(lootPool[Random.Range(0, lootPool.Length)]);

        // the chest stays open for a while, then shrinks away
        yield return new WaitForSeconds(disappearAfter);
        yield return Shrink();
        gameObject.SetActive(false);
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