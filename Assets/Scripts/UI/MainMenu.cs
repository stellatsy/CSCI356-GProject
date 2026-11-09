using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public string gameScene = "Loot Lockdown";      // must match your game scene's name exactly
    public GameObject instructionsPanel;
    public AudioSource music;                       // the menu music
    public float fadeTime = 0.6f;

    bool leaving;

    void Start()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        if (instructionsPanel != null) instructionsPanel.SetActive(false);
        if (music == null) music = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && instructionsPanel != null && instructionsPanel.activeSelf)
            instructionsPanel.SetActive(false);
    }

    public void StartGame()
    {
        if (!leaving) StartCoroutine(FadeThen(() => SceneManager.LoadScene(gameScene)));
    }

    public void ShowInstructions(bool show)
    {
        if (instructionsPanel != null) instructionsPanel.SetActive(show);
    }

    public void QuitGame()
    {
        if (!leaving) StartCoroutine(FadeThen(QuitNow));
    }

    void QuitNow()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;   // lets Quit work in the editor too
#endif
    }

    IEnumerator FadeThen(System.Action after)
    {
        leaving = true;
        if (music != null)
        {
            float start = music.volume;
            for (float t = 0f; t < fadeTime; t += Time.unscaledDeltaTime)
            {
                music.volume = Mathf.Lerp(start, 0f, t / fadeTime);
                yield return null;
            }
            music.volume = 0f;
            music.Stop();
        }
        after();
    }
}