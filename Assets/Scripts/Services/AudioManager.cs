using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // FIELDS & PROPERTIES
    public static AudioManager Instance { get; private set; }

    [Header("Sources")]
    [SerializeField] private AudioSource _musicSource;
    [SerializeField] private AudioSource _sfxSource;

    [Header("Music")]
    [SerializeField] private AudioClip _menuMusic;
    [SerializeField] private AudioClip _gameMusic;

    [Header("SFX")]
    [SerializeField] private AudioClip _attackClip;
    [SerializeField] private AudioClip _foodHitClip;
    [SerializeField] private AudioClip _foodEatenClip;
    [SerializeField] private AudioClip _purchaseClip;
    [SerializeField] private AudioClip _deniedClip;

    [Header("Variation")]
    [Range(0f, 0.5f)][SerializeField] private float _pitchVariation = 0.1f;

    public AudioClip MenuMusic => _menuMusic;
    public AudioClip GameMusic => _gameMusic;
    public AudioClip AttackClip => _attackClip;
    public AudioClip FoodHitClip => _foodHitClip;
    public AudioClip FoodEatenClip => _foodEatenClip;
    public AudioClip PurchaseClip => _purchaseClip;
    public AudioClip DeniedClip => _deniedClip;

    private const string MusicVolumeKey = "music_volume";
    private const string SfxVolumeKey = "sfx_volume";
    private const string MutedKey = "audio_muted";

    public float MusicVolume { get; private set; } = 0.5f;
    public float SfxVolume { get; private set; } = 1f;
    public bool IsMuted { get; private set; }

    public event System.Action OnAudioSettingsChanged;


    // METHODS
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        MusicVolume = PlayerPrefs.GetFloat(MusicVolumeKey, 0.5f);
        SfxVolume = PlayerPrefs.GetFloat(SfxVolumeKey, 1f);
        IsMuted = PlayerPrefs.GetInt(MutedKey, 0) == 1;

        ApplyVolumes();
    }

    public void PlaySfx(AudioClip clip, float volume = 1f)
    {
        if (clip == null || _sfxSource == null)
            return;

        _sfxSource.pitch = 1f + Random.Range(-_pitchVariation, _pitchVariation);
        _sfxSource.PlayOneShot(clip, volume);
    }

    public void PlayMusic(AudioClip clip)
    {
        if (clip == null || _musicSource == null)
            return;

        if (_musicSource.clip == clip && _musicSource.isPlaying)
            return;   // já tocando: não reinicia

        _musicSource.clip = clip;
        _musicSource.loop = true;
        _musicSource.Play();
    }

    public void SetMusicVolume(float volume)
    {
        MusicVolume = Mathf.Clamp01(volume);
        PlayerPrefs.SetFloat(MusicVolumeKey, MusicVolume);
        ApplyVolumes();
    }

    public void SetSfxVolume(float volume)
    {
        SfxVolume = Mathf.Clamp01(volume);
        PlayerPrefs.SetFloat(SfxVolumeKey, SfxVolume);
        ApplyVolumes();
    }

    public void ToggleMute()
    {
        IsMuted = !IsMuted;
        PlayerPrefs.SetInt(MutedKey, IsMuted ? 1 : 0);
        ApplyVolumes();
    }

    private void ApplyVolumes()
    {
        if (_musicSource != null) _musicSource.volume = IsMuted ? 0f : MusicVolume;
        if (_sfxSource != null) _sfxSource.volume = IsMuted ? 0f : SfxVolume;

        OnAudioSettingsChanged?.Invoke();
    }
}