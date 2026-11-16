using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StageHUD : MonoBehaviour
{
    public TMP_Text stageLabel, scoreLabel, bannerText;
    public CanvasGroup banner;
    public Image[] pips;                     // 4 dots, in order

    public Color doneColor = new Color(0.31f, 0.78f, 0.47f);
    public Color currentColor = new Color(1f, 0.75f, 0.16f);
    public Color todoColor = new Color(0.25f, 0.25f, 0.30f);
    public Color bossColor = new Color(0.55f, 0.12f, 0.12f);
    int lastKey = -1;

    void Start()
    {
        GameManager.Instance.OnChanged += Refresh;
        banner.alpha = 0f;
        Refresh();
    }

    void OnDestroy()
    {
        if (GameManager.Instance != null) GameManager.Instance.OnChanged -= Refresh;
    }

    void Refresh()
    {
        var gm = GameManager.Instance;
        stageLabel.text = gm.BossPhase ? "BOSS FIGHT"
                                       : $"STAGE {gm.Stage} / {GameManager.TotalStages}";
        scoreLabel.text = $"SCORE  {gm.Score}";

        for (int i = 0; i < pips.Length; i++)
        {
            int n = i + 1;
            pips[i].color = n < gm.Stage ? doneColor
                          : n == gm.Stage ? (gm.BossPhase ? new Color(0.9f, 0.15f, 0.15f) : currentColor)
                          : (n == GameManager.TotalStages ? bossColor : todoColor);
        }

        int key = gm.Stage * 10 + (gm.BossPhase ? 1 : 0);
        if (key != lastKey)
        {
            lastKey = key;
            if (!gm.BossPhase)                                   // the warning panel handles the boss intro
            {
                StopAllCoroutines();
                StartCoroutine(ShowBanner($"STAGE {gm.Stage}"));
            }
        }
    }

    IEnumerator ShowBanner(string text)
    {
        bannerText.text = text;
        yield return Fade(0f, 1f, 0.3f);
        yield return new WaitForSecondsRealtime(1.5f);
        yield return Fade(1f, 0f, 0.6f);
    }

    IEnumerator Fade(float from, float to, float time)
    {
        for (float t = 0; t < time; t += Time.unscaledDeltaTime)
        {
            banner.alpha = Mathf.Lerp(from, to, t / time);
            yield return null;
        }
        banner.alpha = to;
    }
}