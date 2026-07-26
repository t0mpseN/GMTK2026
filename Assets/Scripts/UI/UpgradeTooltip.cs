using TMPro;
using UnityEngine;

public class UpgradeTooltip : MonoBehaviour
{
    public static UpgradeTooltip Instance { get; private set; }

    [SerializeField] private RectTransform _root;
    [SerializeField] private TextMeshProUGUI _titleText;
    [SerializeField] private TextMeshProUGUI _descriptionText;
    [SerializeField] private TextMeshProUGUI _costText;
    [SerializeField] private TextMeshProUGUI _levelText;
    [SerializeField] private float _verticalGap = 15f;

    private void Awake()
    {
        Instance = this;

        if (_root == null || _titleText == null || _descriptionText == null
            || _costText == null || _levelText == null)
        {
            Debug.LogError($"{name}: referências da tooltip não atribuídas.", this);
            enabled = false;
            return;
        }

        Hide();
    }

    public void Show(UpgradeDefinition definition, RectTransform anchor)
    {
        int level = UpgradeSystem.Instance.GetLevel(definition.Id);
        bool maxed = UpgradeSystem.Instance.IsMaxed(definition.Id);
        bool locked = !UpgradeSystem.Instance.ArePrerequisitesMet(definition.Id);

        _titleText.text = definition.DisplayName;
        _levelText.text = $"Lvl {level}";

        if (maxed)
        {
            _levelText.text = "MAX";
            _costText.text = "MAX";

            UpgradeLevel current = definition.GetLevel(level);
            _descriptionText.text = current?.description ?? string.Empty;
        }
        else
        {
            int nextLevel = level + 1;
            UpgradeLevel next = definition.GetLevel(nextLevel);

            _levelText.text = $"Lvl {nextLevel}";
            _costText.text = $"{next.cost} kcal";
            _descriptionText.text = next.description ?? string.Empty;
        }

        float halfWidth = anchor.rect.width * 0.5f * anchor.lossyScale.x;
        float tooltipWidth = _root.rect.width * _root.lossyScale.x;

        bool fitsRight = anchor.position.x + halfWidth + _verticalGap + tooltipWidth < Screen.width;
        float direction = fitsRight ? 1f : -1f;

        _root.pivot = new Vector2(fitsRight ? 0f : 1f, 0.5f);
        _root.position = anchor.position + new Vector3(direction * (halfWidth + _verticalGap), 0f, 0f);

        _root.gameObject.SetActive(true);
        _root.SetAsLastSibling();
    }

    public void Hide()
    {
        _root.gameObject.SetActive(false);
    }
}