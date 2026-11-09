using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Clips")]
    public AudioClip chestOpen;
    public AudioClip pickup;
    public AudioClip stageClear;
    public AudioClip potion;        // optional
    public AudioClip music;         // optional, loops

    public AudioClip upgrade;       // played when you press U and the weapon levels up
    public AudioClip upgradeReady;  // optional chime when 3/3 copies are collected

    [Range(0f, 1f)] public float sfxVolume = 1f;
    [Range(0f, 1f)] public float musicVolume = 0.35f;


    AudioSource sfx, musicSource;

    void Awake()
    {
        Instance = this;

        sfx = gameObject.AddComponent<AudioSource>();
        sfx.playOnAwake = false;
        sfx.spatialBlend = 0f;

        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.playOnAwake = false;
        musicSource.loop = true;
        musicSource.spatialBlend = 0f;
        if (music != null)
        {
            musicSource.clip = music;
            musicSource.volume = musicVolume;
            musicSource.Play();
        }
    }

    public static void Play(AudioClip clip, float volume = 1f)
    {
        if (Instance != null && clip != null)
            Instance.sfx.PlayOneShot(clip, volume * Instance.sfxVolume);
    }

    public static void Chest() { if (Instance != null) Play(Instance.chestOpen); }
    public static void Pickup() { if (Instance != null) Play(Instance.pickup); }
    public static void StageClear() { if (Instance != null) Play(Instance.stageClear); }
    public static void Potion() { if (Instance != null) Play(Instance.potion); }
    public static void Upgrade()
    {
        
        if (Instance != null) Play(Instance.upgrade);
    }

    public static void UpgradeReady()
    {
        
        if (Instance != null) Play(Instance.upgradeReady);
    }
}