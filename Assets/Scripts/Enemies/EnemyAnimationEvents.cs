using UnityEngine;

public class EnemyAnimationEvents : MonoBehaviour
{
    EnemyAI ai;
    void Awake() { ai = GetComponentInParent<EnemyAI>(); }
    public void DealDamage() { if (ai != null) ai.DealDamage(); }
}