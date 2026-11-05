// EnemyAI.cs - decision making
// Finite state machine (Idle -> Chase -> Attack -> Dead), based on the State pattern
// from Nystrom, Game Programming Patterns. Walking is done by EnemyMovement (Unity NavMesh).
using UnityEngine;

[RequireComponent(typeof(EnemyMovement), typeof(Health))]
public class EnemyAI : MonoBehaviour
{
    public enum State { Idle, Chase, Attack, Dead }

    public float detectRange = 12f;
    public float attackRange = 1.8f;
    public float damage = 10f;
    public float attackCooldown = 1.2f;
    public float hitDelay = 0.5f;        // seconds into the swing when the hit lands
    public float corpseTime = 4f;        // how long the body stays before it is removed
    public State current = State.Idle;

    public bool useAnimationEvent = true;

    EnemyMovement movement;
    Animator animator;
    Health health;
    Health playerHealth;
    Transform player;
    float nextAttackTime;

    float lastHitTime = -10f;

    void Start()
    {
        movement = GetComponent<EnemyMovement>();
        animator = GetComponentInChildren<Animator>();
        health = GetComponent<Health>();
        health.OnDied += () => SetState(State.Dead);

        GameObject p = GameObject.FindWithTag("Player");
        if (p != null) { player = p.transform; playerHealth = p.GetComponent<Health>(); }
    }

    void Update()
    {
        if (current == State.Dead || player == null) return;
        float dist = Vector3.Distance(transform.position, player.position);

        switch (current)
        {
            case State.Idle:
                if (dist <= detectRange) SetState(State.Chase);
                break;

            case State.Chase:
                movement.MoveTo(player.position);
                if (dist <= attackRange) SetState(State.Attack);
                break;

            case State.Attack:
                movement.FaceTarget(player.position);
                if (dist > attackRange + 0.5f) { SetState(State.Chase); break; }
                if (Time.time >= nextAttackTime)
                {
                    nextAttackTime = Time.time + attackCooldown;
                    Trigger("Attack");
                    if (!useAnimationEvent) Invoke(nameof(DealDamage), hitDelay);
                }
                break;
        }
    }

    // Called partway through the attack animation.
    public void DealDamage()
    {
        if (current == State.Dead || playerHealth == null || playerHealth.IsDead) return;
        if (Time.time - lastHitTime < attackCooldown * 0.5f) return;   // one hit per swing
        lastHitTime = Time.time;

        float d = Vector3.Distance(transform.position, player.position);
        if (d <= attackRange + 0.5f)
        {
            
            playerHealth.TakeDamage(damage);
            
        }
    }

    void SetState(State next)
    {
        current = next;
        if (next != State.Chase) movement.Stop();
        if (next == State.Dead)
        {
            CancelInvoke(nameof(DealDamage));
            foreach (var c in GetComponentsInChildren<Collider>()) c.enabled = false;   // bullets and player pass through the corpse
            Trigger("Die");
            Invoke(nameof(RemoveCorpse), corpseTime);
        }
    }

    void RemoveCorpse()
    {
        gameObject.SetActive(false);   // hide instead of Destroy, so the Inspector never loses it
    }


    void Trigger(string triggerName) // ignores animator triggers that don't exist yet
    {
        if (animator == null) return;
        foreach (var p in animator.parameters)
            if (p.name == triggerName) { animator.SetTrigger(triggerName); return; }
    }
}