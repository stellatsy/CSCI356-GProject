// EnemyMovement.cs - movement module (NavMesh pathfinding + animation speed)
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyMovement : MonoBehaviour
{
    NavMeshAgent agent;
    Animator animator;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
    }

    public void MoveTo(Vector3 target)
    {
        agent.isStopped = false;
        agent.SetDestination(target);
    }

    public void Stop()
    {
        agent.isStopped = true;
        agent.velocity = Vector3.zero;   // stop instantly, no coasting
    }

    public void FaceTarget(Vector3 target)
    {
        Vector3 dir = target - transform.position;
        dir.y = 0f;
        if (dir.sqrMagnitude > 0.01f)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 8f * Time.deltaTime);
    }

    void Update()
    {
        if (animator == null) return;
        // 0 whenever the agent is stopped, so Attack/Idle never show the walk animation
        float speed = agent.isStopped ? 0f : agent.velocity.magnitude;
        animator.SetFloat("Speed", speed, 0.1f, Time.deltaTime); // 0.1 = smooth blend
    }
}