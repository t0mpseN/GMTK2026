using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TimerDisplay : MonoBehaviour
{
    [SerializeField] private Color _warningColor = Color.red;
    private float _warningThreshold => ConfigRegistry.Instance.Run.WarningThreshold;

    private TextMeshProUGUI _label;
    private Color _normalColor;

    private void Awake()
    {
        _label = GetComponent<TextMeshProUGUI>();
        _normalColor = _label.color;
    }

    private void Start()
    {
        GameTimer.Instance.OnTimeChanged += UpdateLabel;
        UpdateLabel(GameTimer.Instance.TimeRemaining);
    }

    private void OnDestroy()
    {
        if (GameTimer.Instance != null)
            GameTimer.Instance.OnTimeChanged -= UpdateLabel;
    }

    private void UpdateLabel(float seconds)
    {
        int minutes = Mathf.FloorToInt(seconds / 60f);
        int secs = Mathf.FloorToInt(seconds % 60f);
        _label.text = $"{minutes:00}:{secs:00}";

        _label.color = seconds <= _warningThreshold ? _warningColor : _normalColor;
    }
}
