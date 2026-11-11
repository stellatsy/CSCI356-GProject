using UnityEngine;

public static class GameSettings
{
    public static float Music = 1f;
    public static float Sfx = 1f;
    public static float Sensitivity = 1f;   // multiplier on your existing look speed
    public static float Fov = 85f;         
    public static bool Fullscreen = true;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Load()
    {
        Music = PlayerPrefs.GetFloat("set_music", 1f);
        Sfx = PlayerPrefs.GetFloat("set_sfx", 1f);
        Sensitivity = PlayerPrefs.GetFloat("set_sens", 1f);
        Fov = PlayerPrefs.GetFloat("set_fov", 85f);
        Fullscreen = PlayerPrefs.GetInt("set_full", 1) == 1;
    }

    public static void Save()
    {
        PlayerPrefs.SetFloat("set_music", Music);
        PlayerPrefs.SetFloat("set_sfx", Sfx);
        PlayerPrefs.SetFloat("set_sens", Sensitivity);
        PlayerPrefs.SetFloat("set_fov", Fov);
        PlayerPrefs.SetInt("set_full", Fullscreen ? 1 : 0);
        PlayerPrefs.Save();
    }
}