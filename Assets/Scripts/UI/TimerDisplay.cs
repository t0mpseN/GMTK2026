using UnityEngine;
using UnityEngine.UI;

public class TimerDisplay : MonoBehaviour
{
    // FIELDS & PROPERTIES
    [SerializeField] private Image _fillImage;

    [Header("Colors")]
    [SerializeField] private Color _normalColor = new Color(0.4f, 0.8f, 0.35f);
    [SerializeField] private Color _warningColor = Color.red;

    [Header("Smoothing")]
    [SerializeField] private float _smoothSpeed = 8f;

    private float _targetFill;
    private float WarningThreshold => ConfigRegistry.Instance.Run.WarningThreshold;


    // METHODS
    private void Start()
    {
        GameTimer.Instance.OnTimeChanged += HandleTimeChanged;

        _targetFill = GameTimer.Instance.Progress;
        _fillImage.fillAmount = _targetFill;
    }

    private void OnDestroy()
    {
        if (GameTimer.Instance != null)
            GameTimer.Instance.OnTimeChanged -= HandleTimeChanged;
    }

    private void HandleTimeChanged(float secondsRemaining)
    {
        _targetFill = GameTimer.Instance.Progress;
        _fillImage.color = secondsRemaining <= WarningThreshold ? _warningColor : _normalColor;
    }

    private void Update()
    {
        if (_smoothSpeed <= 0f)
        {
            _fillImage.fillAmount = _targetFill;
            return;
        }

        _fillImage.fillAmount = Mathf.Lerp(_fillImage.fillAmount, _targetFill, _smoothSpeed * Time.deltaTime);
    }
}
