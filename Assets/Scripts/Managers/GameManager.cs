using System;
using UnityEngine;

public enum ScoreType { Kill, EliteKill, StageClear, Boss }

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public const int TotalStages = 4;

    public int Stage { get; private set; } = 1;
    public bool BossPhase { get; private set; }
    public int Score { get; private set; }

    // for the victory screen breakdown
    public int Kills, EliteKills, StagesCleared;
    public bool BossKilled;
    public int KillScore, EliteScore, StageScore, BossScore;

    public bool smgGiven;      // the first chest gives the SMG, once per run

    public event Action OnChanged;

    void Awake() { Instance = this; }

    public void AddScore(int points, ScoreType type)
    {
        Score += points;
        switch (type)
        {
            case ScoreType.Kill: Kills++; KillScore += points; break;
            case ScoreType.EliteKill: EliteKills++; EliteScore += points; break;
            case ScoreType.StageClear: StagesCleared++; StageScore += points; break;
            case ScoreType.Boss: BossKilled = true; BossScore += points; break;
        }
        OnChanged?.Invoke();
    }

    public void SetStage(int s) { Stage = Mathf.Clamp(s, 1, TotalStages); BossPhase = false; OnChanged?.Invoke(); }
    public void StartBossPhase() { Stage = TotalStages; BossPhase = true; OnChanged?.Invoke(); }

    [ContextMenu("Test: next stage")] void TestNext() { SetStage(Stage + 1); }
    [ContextMenu("Test: boss phase")] void TestBoss() { StartBossPhase(); }
    [ContextMenu("Test: +100 kill")] void TestScore() { AddScore(100, ScoreType.Kill); }
}