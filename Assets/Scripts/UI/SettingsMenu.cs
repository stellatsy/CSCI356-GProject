using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class SettingsMenu : MonoBehaviour
{
    public static bool IsOpen;

    [Header("Mode")]
    public bool isMainMenu;
    public string mainMenuScene = "MainMenu";

    [Header("Audio")]
    public AudioMixer mixer;
    public string musicParam = "MusicVol";

    [Header("Controls")]
    public Slider musicSlider;
    public Slider sfxSlider;
    public Slider sensitivitySlider;
    public Slider fovSlider;
    public Toggle fullscreenToggle;
    public Button resumeButton;
    public Button mainMenuButton;
    public Button quitButton;

    [Header("Game only")]
    public Health playerHealth;
    public Camera playerCamera;   // drag FirstPersonCamera here
    Camera Cam => playerCamera != null ? playerCamera : Camera.main;

    CanvasGroup group;
    bool dead;
    

    void Awake()
    {
        group = GetComponent<CanvasGroup>();
        SetVisible(false);
        IsOpen = false;
        Time.timeScale = 1f;
    }

    void Start()
    {
        Camera cam = Cam;
        if (!isMainMenu && cam != null) cam.fieldOfView = GameSettings.Fov;

        if (Screen.fullScreen != GameSettings.Fullscreen) Screen.fullScreen = GameSettings.Fullscreen;
        ApplyMusic();

        Setup(musicSlider, 0f, 1f, false, GameSettings.Music, OnMusic);
        Setup(sfxSlider, 0f, 1f, false, GameSettings.Sfx, OnSfx);
        Setup(sensitivitySlider, 0.2f, 3f, false, GameSettings.Sensitivity, v => GameSettings.Sensitivity = v);
        Setup(fovSlider, 60f, 110f, true, GameSettings.Fov > 0f ? GameSettings.Fov : 60f, OnFov);

        if (fullscreenToggle != null)
        {
            fullscreenToggle.SetIsOnWithoutNotify(GameSettings.Fullscreen);
            fullscreenToggle.onValueChanged.AddListener(v => { GameSettings.Fullscreen = v; Screen.fullScreen = v; });
        }

        if (resumeButton != null) resumeButton.onClick.AddListener(Close);
        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.AddListener(ToMainMenu);
            if (isMainMenu) mainMenuButton.gameObject.SetActive(false);
        }
        if (quitButton != null)
        {
            quitButton.onClick.AddListener(QuitGame);
            if (isMainMenu) quitButton.gameObject.SetActive(false);
        }

        if (playerHealth != null) playerHealth.OnDied += OnPlayerDied;
    }

    void OnDestroy()
    {
        if (playerHealth != null) playerHealth.OnDied -= OnPlayerDied;
    }

    void OnPlayerDied() { dead = true; }

    void Update()
    {
        if (!Input.GetKeyDown(KeyCode.Escape) && !Input.GetKeyDown(KeyCode.P)) return;
        if (IsOpen) Close();
        else if (!isMainMenu && !dead && Time.timeScale != 0f) Open();
    }

    public void Open()
    {
        SetVisible(true);
        IsOpen = true;
        if (!isMainMenu)
        {
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void Close()
    {
        SetVisible(false);
        IsOpen = false;
        GameSettings.Save();
        if (!isMainMenu)
        {
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public void ToMainMenu()
    {
        GameSettings.Save();
        IsOpen = false;
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene(mainMenuScene);
    }

    public void QuitGame()
    {
        GameSettings.Save();
        Time.timeScale = 1f;
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    void SetVisible(bool on)
    {
        group.alpha = on ? 1f : 0f;
        group.interactable = on;
        group.blocksRaycasts = on;
    }

    void Setup(Slider s, float min, float max, bool whole, float value, UnityAction<float> cb)
    {
        if (s == null) return;
        s.minValue = min; s.maxValue = max; s.wholeNumbers = whole;
        s.SetValueWithoutNotify(value);
        s.onValueChanged.AddListener(cb);
    }

    void OnMusic(float v) { GameSettings.Music = v; ApplyMusic(); }

    void OnSfx(float v) { GameSettings.Sfx = v; }

    void OnFov(float v)
    {
        GameSettings.Fov = v;
        if (!isMainMenu && Cam != null) Cam.fieldOfView = v;
    }

    void ApplyMusic()
    {
        if (mixer != null)
            mixer.SetFloat(musicParam, Mathf.Log10(Mathf.Max(GameSettings.Music, 0.0001f)) * 20f);
    }
}