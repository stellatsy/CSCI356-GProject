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

    public void Heal(float amount)
    {
        if (IsDead) return;
        current = Mathf.Min(maxHealth, current + amount);
        OnChanged?.Invoke(current, maxHealth);
    }

    [ContextMenu("Take 10 Damage")]
    void TestDamage() { TakeDamage(10f); }
}