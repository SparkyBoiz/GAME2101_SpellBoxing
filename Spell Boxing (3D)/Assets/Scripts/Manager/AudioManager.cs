using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource musicSource;

    [Range(0f, 1f)]
    [SerializeField] private float musicVolume = 1.0f;
    [Range(0f, 1f)]
    [SerializeField] private float sfxVolume = 1.0f;

    [SerializeField] private AudioClip spellQueuedClip;
    [SerializeField] private AudioClip deathClip;
    [SerializeField] private AudioClip fizzleClip;

    [SerializeField] private AudioClip backgroundMusicClip;

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
            DontDestroyOnLoad(gameObject);
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