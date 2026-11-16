using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryScreen : MonoBehaviour
{
    public CanvasGroup panel;
    public float showDelay = 3f;

    public TMP_Text killsText, elitesText, stagesText;
    public TMP_Text totalText;
    public RectTransform title;          

    bool triggered;

    void Start()
    {
        SetVisible(false);
        if (GameManager.Instance != null) GameManager.Instance.OnChanged += Check;
    }

    void OnDestroy()
    {
        if (GameManager.Instance != null) GameManager.Instance.OnChanged -= Check;
    }

    void Check()
    {
        var gm = GameManager.Instance;
        if (triggered || gm == null || !gm.BossKilled) return;
        triggered = true;
        Invoke(nameof(Show), showDelay);
    }

    void Show()
    {
        foreach (var c in FindObjectsByType<Canvas>(FindObjectsSortMode.None))
            if (!c.transform.IsChildOf(transform)) c.enabled = false;

        var player = GameObject.FindWithTag("Player");
        if (player != null)
            foreach (var b in player.GetComponentsInChildren<MonoBehaviour>())
                b.enabled = false;

        var gm = GameManager.Instance;
        killsText.text = gm.Kills.ToString();
        elitesText.text = gm.EliteKills.ToString();
        stagesText.text = GameManager.TotalStages.ToString();

        AudioManager.StageClear();
        SetVisible(true);
        panel.alpha = 0f;
        StartCoroutine(Sequence(gm.Score));

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0f;
    }

    IEnumerator Sequence(int finalScore)
    {
        totalText.text = "0";
        StartCoroutine(Punch(title, 1.5f, 0.35f));
        yield return Fade(0f, 1f, 0.5f);
        yield return CountUp(finalScore, 1.5f);
    }

    IEnumerator Fade(float from, float to, float time)
    {
        for (float t = 0; t < time; t += Time.unscaledDeltaTime)
        {
            panel.alpha = Mathf.Lerp(from, to, t / time);
            yield return null;
        }
        panel.alpha = to;
    }

    IEnumerator Punch(RectTransform rt, float startScale, float time)
    {
        if (rt == null) yield break;
        for (float t = 0; t < time; t += Time.unscaledDeltaTime)
        {
            float k = 1f - Mathf.Pow(1f - t / time, 3f);
            rt.localScale = Vector3.one * Mathf.Lerp(startScale, 1f, k);
            yield return null;
        }
        rt.localScale = Vector3.one;
    }

    IEnumerator CountUp(int target, float time)
    {
        for (float t = 0; t < time; t += Time.unscaledDeltaTime)
        {
            totalText.text = Mathf.RoundToInt(Mathf.Lerp(0, target, t / time)).ToString("N0");
            yield return null;
        }
        totalText.text = target.ToString("N0");
    }

    void SetVisible(bool v)
    {
        panel.alpha = v ? 1f : 0f;
        panel.interactable = v;
        panel.blocksRaycasts = v;
    }

    public void BackToMenu() { Time.timeScale = 1f; SceneManager.LoadScene("MainMenu"); }

    public void StartAgain()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

}