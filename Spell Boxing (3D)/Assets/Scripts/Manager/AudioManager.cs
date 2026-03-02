using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource sfxSource; // For one-shot sound effects
    [SerializeField] private AudioSource musicSource; // For looping background music

    [Header("Volume Settings")]
    [Range(0f, 1f)]
    [SerializeField] private float musicVolume = 1.0f;
    [Range(0f, 1f)]
    [SerializeField] private float sfxVolume = 1.0f;

    [Header("SFX Clips")]
    [SerializeField] private AudioClip spellQueuedClip;
    [SerializeField] private AudioClip deathClip;
    [SerializeField] private AudioClip fizzleClip;

    [Header("Music")]
    [SerializeField] private AudioClip backgroundMusicClip;

    [Header("Spell Match Clips")]
    [SerializeField] private AudioClip fireMatchClip;
    [SerializeField] private AudioClip waterMatchClip;
    [SerializeField] private AudioClip earthMatchClip;
    [SerializeField] private AudioClip lightningMatchClip;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Make it persistent across scenes
        }
    }

    private void Start()
    {
        if (musicSource != null)
        {
            musicSource.volume = musicVolume;
            if (backgroundMusicClip != null)
            {
                musicSource.clip = backgroundMusicClip;
                musicSource.loop = true;
                musicSource.Play();
            }
        }
        if (sfxSource != null)
        {
            sfxSource.volume = sfxVolume;
        }
    }

    private void Update()
    {
        // This ensures that if you change the volume in the Inspector during runtime,
        // the AudioSource components are updated. This is great for live-tweaking.
        if (musicSource != null && musicSource.volume != musicVolume)
        {
            musicSource.volume = musicVolume;
        }
        if (sfxSource != null && sfxSource.volume != sfxVolume)
        {
            sfxSource.volume = sfxVolume;
        }
    }

    public float MusicVolume
    {
        get { return musicVolume; }
        set
        {
            musicVolume = Mathf.Clamp01(value);
            if (musicSource != null)
            {
                musicSource.volume = musicVolume;
            }
        }
    }

    public float SfxVolume
    {
        get { return sfxVolume; }
        set
        {
            sfxVolume = Mathf.Clamp01(value);
            if (sfxSource != null) sfxSource.volume = sfxVolume;
        }
    }

    public void PlaySpellQueuedSFX()
    {
        if (spellQueuedClip != null) sfxSource.PlayOneShot(spellQueuedClip);
    }

    public void PlayDeathSFX()
    {
        if (deathClip != null) sfxSource.PlayOneShot(deathClip);
    }

    public void PlayFizzleSFX()
    {
        if (fizzleClip != null) sfxSource.PlayOneShot(fizzleClip);
    }

    public void PlaySpellMatchSFX(SpellType spellType)
    {
        AudioClip clipToPlay = spellType switch
        {
            SpellType.Fire => fireMatchClip,
            SpellType.Water => waterMatchClip,
            SpellType.Earth => earthMatchClip,
            SpellType.Lightning => lightningMatchClip,
            _ => null
        };

        if (clipToPlay != null)
        {
            sfxSource.PlayOneShot(clipToPlay);
        }
    }
}