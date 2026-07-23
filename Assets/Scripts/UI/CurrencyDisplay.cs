using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class CurrencyText : MonoBehaviour
{
    // FIELDS & PROPERTIES
    [SerializeField] private string _format = "Caloric deficit: {0} kcal";
    private TextMeshProUGUI _label;


    // METHODS
    private void Awake()
    {
        _label = GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        GameData.Instance.OnCurrencyChanged += UpdateLabel;
        UpdateLabel(GameData.Instance.Data.currency);
    }

    private void OnDestroy()
    {
        if (GameData.Instance != null)
            GameData.Instance.OnCurrencyChanged -= UpdateLabel;
    }

    private void UpdateLabel(int value)
    {
        _label.text = string.Format(_format, value);
    }
}
