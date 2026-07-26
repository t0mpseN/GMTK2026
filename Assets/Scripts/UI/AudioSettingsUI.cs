using UnityEngine;
using UnityEngine.UI;

public class AudioSettingsUI : MonoBehaviour
{
    // FIELDS & PROPERTIES
    [SerializeField] private Button _muteButton;
    [SerializeField] private Image _muteIcon;
    [SerializeField] private Slider _volumeSlider;

    [SerializeField] private Sprite _soundOnSprite;
    [SerializeField] private Sprite _soundOffSprite;

    // METHODS
    private void Start()
    {
        _muteButton.onClick.AddListener(OnMutePressed);
        _volumeSlider.onValueChanged.AddListener(OnSliderChanged);

        AudioManager.Instance.OnAudioSettingsChanged += Refresh;

        _volumeSlider.SetValueWithoutNotify(AudioManager.Instance.MusicVolume);
        Refresh();
    }

    private void OnDestroy()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.OnAudioSettingsChanged -= Refresh;
    }

    private void OnMutePressed()
    {
        AudioManager.Instance.ToggleMute();
    }

    private void OnSliderChanged(float value)
    {
        AudioManager.Instance.SetMusicVolume(value);
        AudioManager.Instance.SetSfxVolume(value);
    }

    private void Refresh()
    {
        bool muted = AudioManager.Instance.IsMuted;

        if (_muteIcon != null && _soundOnSprite != null && _soundOffSprite != null)
            _muteIcon.sprite = muted ? _soundOffSprite : _soundOnSprite;

        _volumeSlider.interactable = !muted;
    }
}