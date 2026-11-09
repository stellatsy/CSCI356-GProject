using System.Collections;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    [System.Serializable]
    public class StageEntry
    {
        public GameObject root;        // Stage1, Stage2, ...
        public Transform playerStart;  // where the player appears
    }

    public StageEntry[] stages;        // one element per finished stage
    public int stageClearScore = 500;
    public float clearHoldTime = 2.5f; // must stay cleared this long (chests spawn ~1.5s after a kill)
    public float nextStageDelay = 3f;

    int current;                       // 0 = Stage 1
    float clearTimer;
    bool seenMonster, transitioning;
    Transform player;

    void Start()
    {
        var p = GameObject.FindWithTag("Player");
        if (p != null) player = p.transform;
    }

    void Update()
    {
        if (transitioning || current >= stages.Length) return;
        var root = stages[current].root;
        if (root == null) return;

        int alive = 0;
        foreach (var e in root.GetComponentsInChildren<EnemyAI>())   // active ones only
        {
            var h = e.GetComponent<Health>();
            if (h == null || !h.IsDead) alive++;
        }
        if (alive > 0) seenMonster = true;

        bool cleared = seenMonster && alive == 0 && ChestLoot.Unopened.Count == 0;
        clearTimer = cleared ? clearTimer + Time.deltaTime : 0f;
        if (clearTimer >= clearHoldTime) StartCoroutine(ClearStage());
    }

    IEnumerator ClearStage()
    {
        transitioning = true;
        var gm = GameManager.Instance;
        if (gm != null) gm.AddScore(stageClearScore, ScoreType.StageClear);
        PickupFeed.Show("STAGE " + (current + 1) + " CLEARED  +" + stageClearScore);

        AudioManager.StageClear();

        yield return new WaitForSeconds(nextStageDelay);

        int next = current + 1;
        if (next >= stages.Length || stages[next].root == null)
        {
            PickupFeed.Show("More stages coming soon...");   // placeholder until Stage 2+ exist
            yield break;
        }

        stages[current].root.SetActive(false);
        stages[next].root.SetActive(true);
        current = next;
        seenMonster = false;
        clearTimer = 0f;
        MovePlayer(stages[next].playerStart);
        if (gm != null) gm.SetStage(next + 1);
        transitioning = false;
    }

    void MovePlayer(Transform target)
    {
        if (player == null || target == null) return;
        var cc = player.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;               // otherwise it snaps back
        player.SetPositionAndRotation(target.position, target.rotation);
        if (cc != null) cc.enabled = true;
    }
}