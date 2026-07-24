using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeRow : MonoBehaviour
{
    // FIELDS & PROPERTIES
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _levelText;
    [SerializeField] private TextMeshProUGUI _costText;
    [SerializeField] private Button _buyButton;

    [Header("Button colors")]
    [SerializeField] private Color _affordableColor = Color.white;
    [SerializeField] private Color _blockedColor = new Color(0.5f, 0.5f, 0.5f);

    private UpgradeDefinition _definition;


    // METHODS
    public void Bind(UpgradeDefinition definition)
    {
        _definition = definition;
        _nameText.text = definition.DisplayName;

        _buyButton.onClick.RemoveAllListeners();
        _buyButton.onClick.AddListener(OnBuyPressed);

        Refresh();
    }

    public void Refresh()
    {
        if (_definition == null)
            return;

        UpgradeId id = _definition.Id;
        int level = UpgradeSystem.Instance.GetLevel(id);
        bool maxed = UpgradeSystem.Instance.IsMaxed(id);

        _levelText.text = $"Lvl. {level}/{_definition.MaxLevel}";

        if (maxed)
        {
            _costText.text = "MAX";
            _buyButton.interactable = false;
        }
        else
        {
            int cost = UpgradeSystem.Instance.GetNextLevelCost(id);
            bool canAfford = UpgradeSystem.Instance.CanPurchase(id);

            _costText.text = cost.ToString();
            _costText.color = canAfford ? _affordableColor : _blockedColor;
            _buyButton.interactable = canAfford;
        }
    }

    private void OnBuyPressed()
    {
        UpgradeSystem.Instance.TryPurchase(_definition.Id);
    }
}