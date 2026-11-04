using UnityEngine;

[RequireComponent(typeof(Health))]
public class EnemyReward : MonoBehaviour
{
    public int scoreValue = 100;
    public ScoreType scoreType = ScoreType.Kill;
    public GameObject chestPrefab;          // leave empty for the boss

    void Start() { GetComponent<Health>().OnDied += Reward; }

    void Reward()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.AddScore(scoreValue, scoreType);

        if (chestPrefab != null)
        {
            Vector3 pos = transform.position;
            if (Physics.Raycast(pos + Vector3.up, Vector3.down, out var hit, 3f))
                pos = hit.point;                       // sit on the floor
            Instantiate(chestPrefab, pos, Quaternion.Euler(0, transform.eulerAngles.y, 0));
        }
    }
}