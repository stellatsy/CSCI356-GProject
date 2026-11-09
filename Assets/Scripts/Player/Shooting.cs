using System.Collections;
using System.Collections.Generic;
using InventoryFramework;
using UnityEngine;

public class Shooting : MonoBehaviour
{
    public Camera cam;                  // FirstPersonCamera
    public HotbarUI hotbarUI;           // auto-found if left empty
    public LayerMask hitMask = ~0;

    public KeyCode reloadKey = KeyCode.R;
    public bool autoReload = true;

    // Current ammo is stored here, not on the item asset, so restarts reset it
    readonly Dictionary<WeaponItem, int> ammo = new();
    readonly Dictionary<WeaponItem, int> reserve = new();

    WeaponItem current;
    float nextFire;
    float nextEmptyClick;
    bool reloading;
    Coroutine reloadRoutine;
    AudioSource shotSource, reloadSource;

    public WeaponItem Current => current;
    public bool Reloading => reloading;
    public int CurrentAmmo => current != null && ammo.TryGetValue(current, out var a) ? a : 0;
    public int Reserve => current != null && reserve.TryGetValue(current, out var r) ? r : 0;
    bool CanReload => current != null && (current.unlimitedReserve || Reserve > 0);

    void Awake()
    {
        if (cam == null) cam = GetComponentInChildren<Camera>();
        if (hotbarUI == null) hotbarUI = FindFirstObjectByType<HotbarUI>();

        shotSource = gameObject.AddComponent<AudioSource>();
        shotSource.playOnAwake = false;
        shotSource.spatialBlend = 0f;
        reloadSource = gameObject.AddComponent<AudioSource>();
        reloadSource.playOnAwake = false;
        reloadSource.spatialBlend = 0f;
    }

    void Update()
    {
        if (Cursor.lockState != CursorLockMode.Locked) return;

        var weapon = hotbarUI != null ? hotbarUI.SelectedItem as WeaponItem : null;
        if (weapon != current) SwitchTo(weapon);
        if (current == null) return;

        if (Input.GetKeyDown(reloadKey)) TryReload();

        bool wantsFire = current.automatic ? Input.GetButton("Fire1")
                                           : Input.GetButtonDown("Fire1");
        if (wantsFire) TryFire();
    }

    void SwitchTo(WeaponItem w)
    {
        if (reloadRoutine != null) StopCoroutine(reloadRoutine);
        reloadSource.Stop();
        reloading = false;
        current = w;
        if (w == null) return;
        if (!ammo.ContainsKey(w)) { ammo[w] = w.magSize; reserve[w] = w.reserveAmmo; }
    }

    void TryFire()
    {
        if (reloading || Time.time < nextFire) return;
        if (CurrentAmmo <= 0)
        {
            if (!CanReload) PlayEmptyClick();   // nothing left to reload: click
            else TryReload();
            return;
        }

        nextFire = Time.time + 1f / current.fireRate;
        ammo[current]--;
        if (current.shotClip != null) shotSource.PlayOneShot(current.shotClip, current.shotVolume);

        if (autoReload && ammo[current] <= 0) StartCoroutine(AutoReloadAfterShot());

        var ray = new Ray(cam.transform.position, cam.transform.forward);
        if (Physics.Raycast(ray, out var hit, current.range, hitMask, QueryTriggerInteraction.Ignore))
        {
            var h = hit.collider.GetComponentInParent<Health>();
            if (h != null && !h.CompareTag("Player") && !h.IsDead)
            {
                h.TakeDamage(WeaponProgress.Instance != null ? WeaponProgress.Instance.Damage(current) : current.damage);
                if (Crosshair.Instance != null) Crosshair.Instance.Hit();
            }
        }
    }

    void PlayEmptyClick()
    {
        if (Time.time < nextEmptyClick) return;
        nextEmptyClick = Time.time + 0.3f;
        if (current.emptyClip != null) shotSource.PlayOneShot(current.emptyClip);
    }

    void TryReload()
    {
        if (reloading || current == null) return;
        if (CurrentAmmo >= current.magSize) return;
        if (!current.unlimitedReserve && Reserve <= 0) return;
        reloadRoutine = StartCoroutine(Reload(current));
    }

    IEnumerator Reload(WeaponItem w)
    {
        reloading = true;
        if (w.reloadClip != null) reloadSource.PlayOneShot(w.reloadClip, w.reloadVolume);
        yield return new WaitForSeconds(w.reloadTime);

        if (w.unlimitedReserve) ammo[w] = w.magSize;
        else
        {
            int take = Mathf.Min(w.magSize - ammo[w], reserve[w]);
            ammo[w] += take;
            reserve[w] -= take;
        }
        reloading = false;
    }

    IEnumerator AutoReloadAfterShot()
    {
        yield return null;      // let this shot finish first
        TryReload();
    }
}