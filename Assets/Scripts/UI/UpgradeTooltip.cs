using TMPro;
using UnityEngine;

public class UpgradeTooltip : MonoBehaviour
{
    public static UpgradeTooltip Instance { get; private set; }

    [SerializeField] private RectTransform _root;
    [SerializeField] private TextMeshProUGUI _titleText;
    [SerializeField] private TextMeshProUGUI _descriptionText;
    [SerializeField] private TextMeshProUGUI _costText;
    [SerializeField] private float _verticalGap = 15f;

    private void Awake()
    {
        Instance = this;
        Hide();
    }

    public void Show(UpgradeDefinition definition, RectTransform anchor)
    {
        int level = UpgradeSystem.Instance.GetLevel(definition.Id);
        bool maxed = UpgradeSystem.Instance.IsMaxed(definition.Id);
        bool locked = !UpgradeSystem.Instance.ArePrerequisitesMet(definition.Id);

        _titleText.text = $"{definition.DisplayName}";

        UpgradeLevel next = definition.GetLevel(level + 1);
        _descriptionText.text = next != null && !string.IsNullOrEmpty(next.description)
            ? next.description
            : string.Empty;

        if (locked)
            _costText.text = "LOCKED";
        else if (maxed)
            _costText.text = "MAX";
        else
            _costText.text = $"{UpgradeSystem.Instance.GetNextLevelCost(definition.Id)} $";

        float halfHeight = anchor.rect.height * 0.5f * anchor.lossyScale.y;
        bool fitsAbove = anchor.position.y + halfHeight + _verticalGap + _root.rect.height * _root.lossyScale.y < Screen.height;
        float direction = fitsAbove ? 1f : -1f;
        _root.pivot = new Vector2(0.5f, fitsAbove ? 0f : 1f);
        _root.position = anchor.position + new Vector3(0f, direction * (halfHeight + _verticalGap), 0f);

        _root.gameObject.SetActive(true);
        _root.SetAsLastSibling(); // garante que fica por cima dos nós
    }

    public void Hide()
    {
        _root.gameObject.SetActive(false);
    }
}