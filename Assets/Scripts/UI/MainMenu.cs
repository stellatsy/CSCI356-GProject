using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public string gameScene = "Loot Lockdown";      // must match your game scene's name exactly
    public GameObject instructionsPanel;

    void Start()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        if (instructionsPanel != null) instructionsPanel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && instructionsPanel != null && instructionsPanel.activeSelf)
            instructionsPanel.SetActive(false);
    }

    public void StartGame() { SceneManager.LoadScene(gameScene); }

    public void ShowInstructions(bool show)
    {
        if (instructionsPanel != null) instructionsPanel.SetActive(show);
    }

    public void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;   // lets Quit work in the editor too
#endif
    }
}