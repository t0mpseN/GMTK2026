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