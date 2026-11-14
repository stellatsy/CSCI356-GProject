using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Clips")]
    public AudioClip chestOpen;
    public AudioClip pickup;
    public AudioClip stageClear;
    public AudioClip potion;
    public AudioClip upgrade;
    public AudioClip upgradeReady;

    public UnityEngine.Audio.AudioMixerGroup musicGroup;

    [Header("Music (one entry per stage: Element 0 = Stage 1)")]
    public AudioClip[] stageMusic;
    public float musicFadeTime = 1.5f;

    [Header("Volumes")]
    [Range(0f, 1f)] public float sfxVolume = 1f;
    [Range(0f, 1f)] public float musicVolume = 0.35f;
    [Range(0f, 1f)] public float pickupVolume = 0.5f;
    [Range(0f, 1f)] public float stageClearVolume = 0.4f;

    [Range(0f, 1f)] public float chestVolume = 0.5f;
    [Range(0f, 1f)] public float upgradeVolume = 0.6f;
    [Range(0f, 1f)] public float upgradeReadyVolume = 0.3f;

    [Range(0f, 1f)] public float potionVolume = 0.5f;

    AudioSource sfx, musicSource;
    Coroutine musicRoutine;

    void Awake()
    {
        Instance = this;

        sfx = gameObject.AddComponent<AudioSource>();
        sfx.playOnAwake = false;
        sfx.spatialBlend = 0f;

        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.outputAudioMixerGroup = musicGroup;
        musicSource.playOnAwake = false;
        musicSource.loop = true;
        musicSource.spatialBlend = 0f;
        musicSource.volume = musicVolume;

        if (stageMusic != null && stageMusic.Length > 0 && stageMusic[0] != null)
        {
            musicSource.clip = stageMusic[0];
            musicSource.Play();
        }
    }

    // Called by StageManager when a new stage starts (0 = Stage 1)
    public static void StageMusic(int index)
    {
        if (Instance != null) Instance.SwitchMusic(index);
    }

    void SwitchMusic(int index)
    {
        AudioClip next = (stageMusic != null && index >= 0 && index < stageMusic.Length) ? stageMusic[index] : null;
        if (musicRoutine != null) StopCoroutine(musicRoutine);
        musicRoutine = StartCoroutine(FadeToClip(next));
    }

    IEnumerator FadeToClip(AudioClip next)
    {
        // fade the current track out
        float start = musicSource.volume;
        for (float t = 0f; t < musicFadeTime; t += Time.unscaledDeltaTime)
        {
            musicSource.volume = Mathf.Lerp(start, 0f, t / musicFadeTime);
            yield return null;
        }
        musicSource.Stop();

        // nothing for this stage: stay silent
        if (next == null) yield break;

        // fade the new track in
        musicSource.clip = next;
        musicSource.Play();
        for (float t = 0f; t < musicFadeTime; t += Time.unscaledDeltaTime)
        {
            musicSource.volume = Mathf.Lerp(0f, musicVolume, t / musicFadeTime);
            yield return null;
        }
        musicSource.volume = musicVolume;
    }

    public static void Play(AudioClip clip, float volume = 1f)
    {
        if (Instance != null && clip != null)
            Instance.sfx.PlayOneShot(clip, volume * Instance.sfxVolume * GameSettings.Sfx);
    }

    public static void Chest() { if (Instance != null) Play(Instance.chestOpen, Instance.chestVolume); }
    public static void Pickup() { if (Instance != null) Play(Instance.pickup, Instance.pickupVolume); }
    public static void StageClear() { if (Instance != null) Play(Instance.stageClear, Instance.stageClearVolume); }
    public static void Potion() { if (Instance != null) Play(Instance.potion, Instance.potionVolume); }
    public static void Upgrade() { if (Instance != null) Play(Instance.upgrade, Instance.upgradeVolume); }
    public static void UpgradeReady() { if (Instance != null) Play(Instance.upgradeReady, Instance.upgradeReadyVolume); }
}