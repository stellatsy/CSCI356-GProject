using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    public AudioClip hurtClip, deathClip;

    [Range(0f, 1f)] public float hurtVolume = 0.5f;
    [Range(0f, 1f)] public float deathVolume = 0.7f;

    public float startHealth = 100f;     // match the player's max health

    Health health;
    AudioSource src;
    float lastHp;

    void Start()
    {
        health = GetComponent<Health>();
        src = gameObject.AddComponent<AudioSource>();
        src.playOnAwake = false;
        src.spatialBlend = 0f;
        lastHp = startHealth;
        if (health == null) return;
        health.OnChanged += Changed;
        health.OnDied += Died;
    }

    void OnDestroy()
    {
        if (health == null) return;
        health.OnChanged -= Changed;
        health.OnDied -= Died;
    }

    void Changed(float cur, float max)
    {
        if (cur < lastHp && cur > 0f && hurtClip != null) src.PlayOneShot(hurtClip, hurtVolume * GameSettings.Sfx);
        lastHp = cur;
    }

    void Died()
    {
        if (deathClip != null) src.PlayOneShot(deathClip, deathVolume * GameSettings.Sfx);
    }
}