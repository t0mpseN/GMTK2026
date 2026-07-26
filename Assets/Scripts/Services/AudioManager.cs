using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class AudioManager : MonoBehaviour
{
    private class MusicChannel
    {
        public AudioSource Source;
        public float Fade;       
        public float TargetFade; 
        public float Speed;       
    }

    // FIELDS & PROPERTIES
    public static AudioManager Instance { get; private set; }

    [Header("Sources")]
    [FormerlySerializedAs("_musicSource")]
    [SerializeField] private AudioSource _musicSourceA;
    [Tooltip("Segundo canal de musica. Necessario para crossfade real. Se ficar vazio, a transicao vira fade-out + fade-in.")]
    [SerializeField] private AudioSource _musicSourceB;
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

    [Header("Music Transitions")]
    [Tooltip("Duracao usada quando PlayMusic e chamado sem especificar fade.")]
    [SerializeField] private float _defaultFadeDuration = 1.5f;
    [Tooltip("Retoma cada faixa do ponto onde parou, em vez de reiniciar do zero.")]
    [SerializeField] private bool _resumeMusicPosition = true;

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

    // Posicao (em segundos) em que cada faixa estava quando saiu de cena.
    private readonly Dictionary<AudioClip, float> _musicPositions = new Dictionary<AudioClip, float>();

    private MusicChannel[] _channels;
    private AudioClip _requestedClip;
    private AudioClip _pendingClip;         
    private float _pendingFadeDuration;
    private bool _isFading;

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

        _channels = new[]
        {
            new MusicChannel { Source = _musicSourceA },
            new MusicChannel { Source = _musicSourceB },
        };

        MusicVolume = PlayerPrefs.GetFloat(MusicVolumeKey, 0.5f);
        SfxVolume = PlayerPrefs.GetFloat(SfxVolumeKey, 1f);
        IsMuted = PlayerPrefs.GetInt(MutedKey, 0) == 1;
        ApplyVolumes();
    }

    private void Update()
    {
        if (!_isFading)
            return;

        bool stillFading = false;

        foreach (MusicChannel ch in _channels)
        {
            if (ch.Source == null)
                continue;

            if (ch.Fade != ch.TargetFade)
            {
                ch.Fade = Mathf.MoveTowards(ch.Fade, ch.TargetFade, ch.Speed * Time.unscaledDeltaTime);
                if (ch.Fade != ch.TargetFade)
                    stillFading = true;
            }

            StopChannelIfSilent(ch);
        }

        ApplyMusicVolumes();

        if (stillFading)
            return;

        _isFading = false;

        if (_pendingClip != null)
        {
            AudioClip clip = _pendingClip;
            _pendingClip = null;

            MusicChannel free = PickFreeChannel();
            if (free != null)
            {
                StartChannel(free, clip, false);
                SetTarget(free, 1f, _pendingFadeDuration);
                _isFading = true;
            }
        }
    }

    public void PlaySfx(AudioClip clip, float volume = 1f)
    {
        if (clip == null || _sfxSource == null)
            return;

        _sfxSource.pitch = 1f + Random.Range(-_pitchVariation, _pitchVariation);
        _sfxSource.PlayOneShot(clip, volume);
    }

    public void PlayMusic(AudioClip clip) => PlayMusic(clip, -1f);

    public void PlayMusic(AudioClip clip, float fadeDuration, bool restartFromStart = false)
    {
        if (clip == null)
            return;

        if (_requestedClip == clip && FindPlayingChannel(clip) != null)
            return;

        _requestedClip = clip;
        _pendingClip = null;

        float duration = fadeDuration < 0f ? _defaultFadeDuration : fadeDuration;
        MusicChannel target = FindPlayingChannel(clip);

        if (target == null)
        {
            MusicChannel free = PickFreeChannel();
            if (free == null)
                return;   // nenhuma AudioSource atribuida no Inspector

            if (free.Source.isPlaying && free.Source.clip != clip)
            {
                SavePosition(free);
                SetTarget(free, 0f, duration);
                _pendingClip = clip;
                _pendingFadeDuration = duration;
                _isFading = true;
                return;
            }

            StartChannel(free, clip, restartFromStart);
            target = free;
        }

        foreach (MusicChannel ch in _channels)
        {
            if (ch.Source == null)
                continue;

            if (ch == target)
            {
                SetTarget(ch, 1f, duration);
            }
            else
            {
                SavePosition(ch);
                SetTarget(ch, 0f, duration);
            }
        }

        _isFading = true;
    }

    public void StopMusic(float fadeDuration = -1f)
    {
        float duration = fadeDuration < 0f ? _defaultFadeDuration : fadeDuration;

        _requestedClip = null;
        _pendingClip = null;

        foreach (MusicChannel ch in _channels)
        {
            if (ch.Source == null)
                continue;

            SavePosition(ch);
            SetTarget(ch, 0f, duration);
        }

        _isFading = true;
    }

    private void SetTarget(MusicChannel ch, float target, float duration)
    {
        ch.TargetFade = Mathf.Clamp01(target);

        if (duration <= 0f)
        {
            ch.Fade = ch.TargetFade;
            ch.Speed = 0f;
            StopChannelIfSilent(ch);
            ApplyMusicVolumes();
            return;
        }

        ch.Speed = 1f / duration;
    }

    private void StartChannel(MusicChannel ch, AudioClip clip, bool restartFromStart)
    {
        ch.Fade = 0f;
        ch.Source.clip = clip;
        ch.Source.loop = true;
        ch.Source.volume = 0f; 

        float startTime = 0f;
        if (_resumeMusicPosition && !restartFromStart &&
            _musicPositions.TryGetValue(clip, out float savedTime))
        {
            startTime = Mathf.Clamp(savedTime, 0f, Mathf.Max(0f, clip.length - 0.05f));
        }

        ch.Source.Play();
        ch.Source.time = startTime; 
    }

    private void StopChannelIfSilent(MusicChannel ch)
    {
        if (ch.Fade <= 0f && ch.TargetFade <= 0f && ch.Source.isPlaying)
            ch.Source.Stop();
    }

    private void SavePosition(MusicChannel ch)
    {
        if (ch.Source == null || ch.Source.clip == null || !ch.Source.isPlaying)
            return;

        _musicPositions[ch.Source.clip] = ch.Source.time;
    }

    private MusicChannel FindPlayingChannel(AudioClip clip)
    {
        foreach (MusicChannel ch in _channels)
        {
            if (ch.Source != null && ch.Source.clip == clip && ch.Source.isPlaying)
                return ch;
        }
        return null;
    }

    private MusicChannel PickFreeChannel()
    {
        MusicChannel best = null;
        foreach (MusicChannel ch in _channels)
        {
            if (ch.Source == null)
                continue;
            if (!ch.Source.isPlaying)
                return ch;
            if (best == null || ch.Fade < best.Fade)
                best = ch;   
        }
        return best;
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

    private void ApplyMusicVolumes()
    {
        if (_channels == null)
            return;

        foreach (MusicChannel ch in _channels)
        {
            if (ch.Source == null)
                continue;

            float curved = Mathf.Sin(Mathf.Clamp01(ch.Fade) * Mathf.PI * 0.5f);
            ch.Source.volume = IsMuted ? 0f : MusicVolume * curved;
        }
    }

    private void ApplyVolumes()
    {
        ApplyMusicVolumes();
        if (_sfxSource != null) _sfxSource.volume = IsMuted ? 0f : SfxVolume;
        OnAudioSettingsChanged?.Invoke();
    }
}