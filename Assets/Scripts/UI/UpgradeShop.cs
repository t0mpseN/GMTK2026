using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UpgradeShop : MonoBehaviour
{
    // FIELDS & PROPERTIES
    [SerializeField] private UpgradeRow _rowPrefab;
    [SerializeField] private Transform _rowContainer;
    private readonly List<UpgradeRow> _rows = new List<UpgradeRow>();

    [SerializeField] private bool _refundOnReset = true;
    [SerializeField] private TextMeshProUGUI _resetButtonLabel;
    private bool _resetArmed;

    // METHODS
    private void Start()
    {
        BuildRows();

        UpgradeSystem.Instance.OnUpgradePurchased += HandleUpgradePurchased;
        UpgradeSystem.Instance.OnUpgradesReset += RefreshAll;
        GameData.Instance.OnCurrencyChanged += HandleCurrencyChanged;
    }

    private void OnDestroy()
    {
        if (UpgradeSystem.Instance != null)
        {
            UpgradeSystem.Instance.OnUpgradePurchased -= HandleUpgradePurchased;
            UpgradeSystem.Instance.OnUpgradesReset -= RefreshAll;
        }

        if (GameData.Instance != null)
            GameData.Instance.OnCurrencyChanged -= HandleCurrencyChanged;
    }

    public void OnResetPressed()
    {
        if (!_resetArmed)
        {
            _resetArmed = true;
            _resetButtonLabel.text = "SURE?";
            return;
        }

        UpgradeSystem.Instance.ResetUpgrades(_refundOnReset);
        _resetArmed = false;
        _resetButtonLabel.text = "RESET UPGRADES";
        _resetButtonLabel.fontSize = 16;
    }

    private void BuildRows()
    {
        foreach (UpgradeDefinition definition in UpgradeSystem.Instance.Catalog.Upgrades)
        {
            if (definition == null) 
                continue;

            UpgradeRow row = Instantiate(_rowPrefab, _rowContainer);
            row.Bind(definition);
            _rows.Add(row);
        }
    }

    private void RefreshAll()
    {
        foreach (UpgradeRow row in _rows)
            row.Refresh();
    }

    private void HandleUpgradePurchased(UpgradeId id, int newLevel) => RefreshAll();
    private void HandleCurrencyChanged(int currency) => RefreshAll();
}