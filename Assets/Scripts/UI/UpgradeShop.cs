using System.Collections.Generic;
using UnityEngine;

public class UpgradeShop : MonoBehaviour
{
    // FIELDS & PROPERTIES
    [SerializeField] private UpgradeRow _rowPrefab;
    [SerializeField] private Transform _rowContainer;

    private readonly List<UpgradeRow> _rows = new List<UpgradeRow>();

    // METHODS
    private void Start()
    {
        BuildRows();

        UpgradeSystem.Instance.OnUpgradePurchased += HandleUpgradePurchased;
        GameData.Instance.OnCurrencyChanged += HandleCurrencyChanged;
    }

    private void OnDestroy()
    {
        if (UpgradeSystem.Instance != null)
            UpgradeSystem.Instance.OnUpgradePurchased -= HandleUpgradePurchased;

        if (GameData.Instance != null)
            GameData.Instance.OnCurrencyChanged -= HandleCurrencyChanged;
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