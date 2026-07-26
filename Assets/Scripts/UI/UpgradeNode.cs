using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class UpgradeNode : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    // FIELDS & PROPERTIES
    [Header("Data")]
    [SerializeField] private UpgradeDefinition _definition;

    [Header("References")]
    [SerializeField] private RectTransform _iconTransform;
    [SerializeField] private Image _iconImage;
    [SerializeField] private Image _frameImage;

    [Header("Hover Flair")]
    [SerializeField] private float _hoverScale = 1.15f;
    [SerializeField] private float _hoverRotation = -8f;
    [SerializeField] private float _animationSpeed = 12f;

    [Header("State Colors")]
    [SerializeField] private Color _availableColor = Color.white;
    [SerializeField] private Color _lockedColor = new Color(0.35f, 0.35f, 0.35f);
    [SerializeField] private Color _maxedColor = new Color(1f, 0.85f, 0.3f);

    private RectTransform _rectTransform;
    private bool _isHovered;

    public UpgradeDefinition Definition => _definition;

    // METHODS
    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();

        if (_definition != null && _iconImage != null)
            _iconImage.sprite = _definition.Icon;
    }

    private void OnEnable()
    {
        Refresh();
    }

    public void Refresh()
    {
        if (_definition == null) return;
        if (UpgradeSystem.Instance == null || GameData.Instance == null) return;

        int level = UpgradeSystem.Instance.GetLevel(_definition.Id);
        bool locked = !UpgradeSystem.Instance.ArePrerequisitesMet(_definition.Id);
        bool maxed = UpgradeSystem.Instance.IsMaxed(_definition.Id);

        Color stateColor = locked ? _lockedColor : maxed ? _maxedColor : _availableColor;

        if (_frameImage != null) 
            _frameImage.color = stateColor;

        if (_iconImage != null)
            _iconImage.color = stateColor;
    }

    private void Update()
    {
        if (_iconTransform == null) 
            return;

        float t = _animationSpeed * Time.unscaledDeltaTime;

        Vector3 targetScale = Vector3.one * (_isHovered ? _hoverScale : 1f);
        _iconTransform.localScale = Vector3.Lerp(_iconTransform.localScale, targetScale, t);

        float currentZ = _iconTransform.localEulerAngles.z;
        float targetZ = _isHovered ? _hoverRotation : 0f;
        _iconTransform.localRotation = Quaternion.Euler(0f, 0f, Mathf.LerpAngle(currentZ, targetZ, t));
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_definition == null) return;
        if (!UpgradeSystem.Instance.ArePrerequisitesMet(_definition.Id)) return;

        _isHovered = true;
        UpgradeTooltip.Instance?.Show(_definition, _rectTransform);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _isHovered = false;
        UpgradeTooltip.Instance?.Hide();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (UpgradeSystem.Instance.TryPurchase(_definition.Id))
            UpgradeTooltip.Instance?.Show(_definition, _rectTransform); 
    }
}