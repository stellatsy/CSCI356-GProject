// Shooting.cs - hitscan weapon using a raycast from the camera through the crosshair
using UnityEngine;

public class Shooting : MonoBehaviour
{
    public float damage = 20f;
    public float range = 50f;
    public float fireRate = 4f;          // shots per second
    public LayerMask hitMask = ~0;       // everything by default
    float nextFire;

    public Camera cam;   // drag FirstPersonCamera here, or leave empty to auto-find

    void Start()
    {
        if (cam == null) cam = GetComponentInChildren<Camera>();
    }

    void Update()
    {
        if (Cursor.lockState != CursorLockMode.Locked) return; // no shooting after death
        if (Input.GetButton("Fire1") && Time.time >= nextFire)
        {
            nextFire = Time.time + 1f / fireRate;
            Shoot();
        }
    }

    void Shoot()
    {
     
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, range, hitMask, QueryTriggerInteraction.Ignore))
        {
            Debug.Log("Shot hit: " + hit.collider.name + " (distance " + hit.distance.ToString("F1") + ")");
            Health h = hit.collider.GetComponentInParent<Health>();
            if (h != null && !h.CompareTag("Player"))
            {
                h.TakeDamage(damage);
                if (Crosshair.Instance != null) Crosshair.Instance.Hit();
            }
            Debug.DrawLine(ray.origin, hit.point, Color.red, 0.5f);
        }
        else Debug.Log("Shot hit nothing");
    }
}