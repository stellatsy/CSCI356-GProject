// Health.cs - shared by the player and all monsters
using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    public float maxHealth = 100f;
    public float current;
    public bool IsDead => current <= 0f;

    public event Action<float, float> OnChanged; // current, max (for health bars)
    public event Action OnDied;

    void Awake() { current = maxHealth; }

    public void TakeDamage(float amount)
    {
        if (IsDead) return;
        current = Mathf.Max(0f, current - amount);
        OnChanged?.Invoke(current, maxHealth);
        if (IsDead) OnDied?.Invoke();
    }

    public bool Heal(float amount)
    {
        if (IsDead || current >= maxHealth) return false;   // nothing to heal
        current = Mathf.Min(maxHealth, current + amount);
        OnChanged?.Invoke(current, maxHealth);               // updates the health bar
        return true;
    }

    

    [ContextMenu("Take 10 Damage")]
    void TestDamage() { TakeDamage(10f); }
}