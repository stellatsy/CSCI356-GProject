using System.Collections;
using UnityEngine;

public class StageProgressManager : MonoBehaviour
{

    public enum Trigger { ChestsCollected, OrdinariesKilled, ElitesKilled }

    Health playerHealth;

    int running;
    bool playerDead;

    [System.Serializable]
    public class Alert
    {
        public CanvasGroup group;   // alpha = 0 at start (leave empty to skip)
        public AudioClip clip;
        public float fadeIn = 1f;
        public float hold = 2f;
        public float fadeOut = 1f;
        public float blinkSpeed = 3f;       // blinks per second (0 = no blinking)
        public float blinkMinAlpha = 0.15f; // how dim it gets between blinks
    }

    [System.Serializable]
    public class Step
    {
        public string name;
        public Trigger waitFor;
        public int amountNeeded = 1;        // e.g. 3 chests, 8 ordinaries, 4 elites
        public bool requireChestsOpened;    // tick for the boss step
        public float chestSpawnWait = 2f;   // chests appear ~1.5s after the last kill
        public GameObject[] spawn;          // placed in scene, INACTIVE
        public Alert alert;
        public AudioClip battleMusic;       // leave empty for no battle music
        public float musicFadeIn = 1f;
        public float musicFadeOut = 3f;

        [HideInInspector] public int bossesLeft;
        [HideInInspector] public int count;
        [HideInInspector] public bool done;
    }

    public Step[] steps;

    [Header("Audio")]
    public AudioSource alertSource; // 2D (Spatial Blend = 0)
    public AudioSource musicSource;     // 2D (Spatial Blend = 0), Loop ticked, Play On Awake unticked
    [Range(0f, 1f)] public float musicVolume = 0.4f;
    Coroutine musicRoutine;

    void Awake()
    {
        foreach (var s in steps)
            if (s.alert != null && s.alert.group != null)
            {
                s.alert.group.alpha = 0f;
                s.alert.group.gameObject.SetActive(false);
            }
    }

    // Call these from other scripts
    public void ChestCollected() { Report(Trigger.ChestsCollected); }
    public void OrdinaryKilled() { Report(Trigger.OrdinariesKilled); }
    public void EliteKilled() { Report(Trigger.ElitesKilled); }

    void Report(Trigger t)
    {
        foreach (var s in steps)
        {
            if (s.done || s.waitFor != t) continue;
            s.count++;
            if (s.count >= s.amountNeeded)
            {
                s.done = true;
                StartCoroutine(RunStep(s));
            }
        }
    }

    public bool HasPendingSteps
    {
        get
        {
            if (running > 0) return true;
            foreach (var s in steps)
            {
                if (!s.done) return true;
                if (s.bossesLeft > 0) return true;   // boss spawned but not dead yet
            }
            return false;
        }
    }

    IEnumerator RunStep(Step s)
    {
        running++;

        // Wait until all chests are opened (needs ChestLoot.Unopened from your StageManager)
        if (s.requireChestsOpened)
        {
            yield return new WaitForSeconds(s.chestSpawnWait);
            while (ChestLoot.Unopened.Count > 0 && !playerDead)
                yield return null;
        }

        if (playerDead) { running--; yield break; }

        yield return PlayAlert(s.alert);

        if (playerDead) { running--; yield break; }

        foreach (var g in s.spawn)
            if (g != null) g.SetActive(true);

        s.bossesLeft = 0;
        foreach (var g in s.spawn)
        {
            if (g == null) continue;
            var ai = g.GetComponentInChildren<EnemyAI>(true);
            if (ai != null && ai.enemyType == EnemyAI.EnemyType.Boss) s.bossesLeft++;
        }

        if (s.battleMusic != null && musicSource != null)
            StartMusic(s.battleMusic, s.musicFadeIn);

        running--;
    }

    IEnumerator PlayAlert(Alert a)
    {
        if (a == null) yield break;
        if (alertSource != null && a.clip != null) alertSource.PlayOneShot(a.clip);
        if (a.group == null) yield break;

        a.group.gameObject.SetActive(true);

        yield return Fade(a.group, 0f, 1f, a.fadeIn);

        // Blink during the hold time
        float t = 0f;
        while (t < a.hold && !playerDead)
        {
            t += Time.deltaTime;
            if (a.blinkSpeed > 0f)
            {
                float k = (Mathf.Cos(t * a.blinkSpeed * 2f * Mathf.PI) + 1f) * 0.5f;
                a.group.alpha = Mathf.Lerp(a.blinkMinAlpha, 1f, k);
            }
            yield return null;
        }

        yield return Fade(a.group, a.group.alpha, 0f, a.fadeOut);
        a.group.gameObject.SetActive(false);
    }

    IEnumerator Fade(CanvasGroup g, float from, float to, float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            g.alpha = Mathf.Lerp(from, to, t / duration);
            yield return null;
        }
        g.alpha = to;
    }

    public void BossKilled()
    {
        foreach (var s in steps)
        {
            if (s.bossesLeft <= 0) continue;
            s.bossesLeft--;
            if (s.bossesLeft == 0 && s.battleMusic != null && musicSource != null)
                StopMusic(s.musicFadeOut);
        }
    }

    void StartMusic(AudioClip clip, float fadeIn)
    {
        if (musicRoutine != null) StopCoroutine(musicRoutine);
        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.volume = 0f;
        musicSource.Play();
        musicRoutine = StartCoroutine(FadeMusic(0f, musicVolume, fadeIn, false));
    }

    void StopMusic(float fadeOut)
    {
        if (musicRoutine != null) StopCoroutine(musicRoutine);
        musicRoutine = StartCoroutine(FadeMusic(musicSource.volume, 0f, fadeOut, true));
    }

    IEnumerator FadeMusic(float from, float to, float duration, bool stopAtEnd)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(from, to, t / duration);
            yield return null;
        }
        musicSource.volume = to;
        if (stopAtEnd) musicSource.Stop();
    }

    void OnEnable()
    {
        playerDead = false;

        var p = GameObject.FindWithTag("Player");
        if (p != null)
        {
            playerHealth = p.GetComponent<Health>();
            if (playerHealth != null) playerHealth.OnDied += OnPlayerDied;
        }
    }

    void OnDisable()
    {
        if (playerHealth != null) playerHealth.OnDied -= OnPlayerDied;
        if (musicSource != null) musicSource.Stop();
    }

    void OnPlayerDied()
    {
        playerDead = true;
        if (alertSource != null) alertSource.Stop();
        if (musicSource != null && musicSource.isPlaying) StopMusic(1.5f);
    }
}