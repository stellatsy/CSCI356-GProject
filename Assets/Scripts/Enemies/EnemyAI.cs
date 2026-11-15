// EnemyAI.cs - decision making

using UnityEngine;
using static UnityEngine.SpriteMask;

[RequireComponent(typeof(EnemyMovement), typeof(Health))]
public class EnemyAI : MonoBehaviour
{
    public enum State { Idle, Chase, Attack, Dead }
    public enum EnemyType { Ordinary, Elite, Boss }
    public EnemyType enemyType;

    public float detectRange = 12f;
    public float attackRange = 1.8f;
    public float damage = 10f;
    public float attackCooldown = 1.2f;
    public float hitDelay = 0.5f;        // seconds into the swing when the hit lands
    public float corpseTime = 4f;        // how long the body stays before it is removed
    public State current = State.Idle;

    public bool useAnimationEvent = true;

    [Header("Audio")]
    public AudioClip walkClip;
    [Range(0f, 1f)] public float walkVolume = 0.8f;
    private AudioSource walkSource;
    public AudioClip attackClip, deathClip;

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

        walkSource = gameObject.AddComponent<AudioSource>();
        walkSource.clip = walkClip;
        walkSource.loop = true;
        walkSource.playOnAwake = false;
        walkSource.volume = walkVolume;
        walkSource.spatialBlend = 1f; // 3D sound, so it gets louder as the boss approaches
    }

    void Update()
    {
        // Walking sound: plays only while chasing, stops in every other state (including Dead)
        bool isWalking = current == State.Chase;
        if (isWalking && !walkSource.isPlaying) walkSource.Play();
        else if (!isWalking && walkSource.isPlaying) walkSource.Stop();

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
                    PlaySound(attackClip);
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
            var mgr = GetComponentInParent<StageProgressManager>();
            if (mgr != null)
            {
                if (enemyType == EnemyType.Ordinary) mgr.OrdinaryKilled();
                else if (enemyType == EnemyType.Elite) mgr.EliteKilled();
                else if (enemyType == EnemyType.Boss) mgr.BossKilled();
            }

            CancelInvoke(nameof(DealDamage));
            foreach (var c in GetComponentsInChildren<Collider>()) c.enabled = false;   // bullets and player pass through the corpse
            Trigger("Die");
            PlaySound(deathClip);
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

    void PlaySound(AudioClip c)
    {
        if (c != null) AudioSource.PlayClipAtPoint(c, transform.position, GameSettings.Sfx);
    }
}