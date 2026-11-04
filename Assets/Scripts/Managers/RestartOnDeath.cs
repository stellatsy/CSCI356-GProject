using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartOnDeath : MonoBehaviour
{
    public float delay = 2.5f;
    public CanvasGroup deathScreen;      // optional "YOU DIED" panel

    void Start()
    {
        if (deathScreen != null) deathScreen.alpha = 0f;

        var player = GameObject.FindWithTag("Player");
        if (player != null)
            player.GetComponent<Health>().OnDied += () => StartCoroutine(Restart());
    }

    IEnumerator Restart()
    {
        for (float t = 0; t < delay; t += Time.unscaledDeltaTime)
        {
            if (deathScreen != null)
                deathScreen.alpha = Mathf.Clamp01(t / (delay * 0.5f));   // fade in over the first half
            yield return null;
        }
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}