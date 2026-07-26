using UnityEngine;

public class FloatingTextSpawner : MonoBehaviour
{
    public static FloatingTextSpawner Instance { get; private set; }

    [SerializeField] private FloatingText _prefab;
    [SerializeField] private int _sortingOrder = 500;

    [Header("Colors")]
    [SerializeField] private Color _damageColor = Color.white;
    [SerializeField] private Color _currencyColor = new Color(1f, 0.85f, 0.2f);
    [SerializeField] private Color _penaltyColor = new Color(1f, 0.3f, 0.3f);
    [SerializeField] private Color _eatenSpecialColor = new Color(0.4f, 0.9f, 1f);

    public Color DamageColor => _damageColor;
    public Color CurrencyColor => _currencyColor;
    public Color PenaltyColor => _penaltyColor;
    public Color EatenSpecialColor => _eatenSpecialColor;

    private void Awake()
    {
        Instance = this;
    }

    public void Spawn(string message, Vector3 worldPosition, Color color)
    {
        if (_prefab == null) return;

        FloatingText instance = Instantiate(_prefab, worldPosition, Quaternion.identity);
        instance.Show(message, color);
    }
}