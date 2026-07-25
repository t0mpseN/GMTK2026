using System;
using UnityEngine;

public class UpgradeSystem : MonoBehaviour
{
    // FIELDS & PROPERTIES
    public static UpgradeSystem Instance { get; private set; }
    [SerializeField] private UpgradeCatalog _catalog;
    public UpgradeCatalog Catalog => _catalog;


    // EVENTS 
    public event Action<UpgradeId, int> OnUpgradePurchased;
    public event Action OnUpgradesReset;


    // METHODS
    private void Awake()
    {
        if (Instance != null && Instance != this)
        { 
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public int GetLevel(UpgradeId id)
    {
        return GameData.Instance.Data.GetUpgradeLevel(id);
    }

    public bool IsMaxed(UpgradeId id)
    {
        UpgradeDefinition upgrade = _catalog.Get(id);
        return upgrade != null && GetLevel(id) >= upgrade.MaxLevel; 
    }

    public int GetNextLevelCost(UpgradeId id)
    {
        UpgradeDefinition upgrade = _catalog.Get(id);
        if (upgrade == null)
            return -1;

        UpgradeLevel nextLevel = upgrade.GetLevel(GetLevel(id) + 1);
        return nextLevel != null ? nextLevel.cost : -1;
    }

    public bool CanPurchase(UpgradeId id)
    {
        int cost = GetNextLevelCost(id);
        if (cost < 0)
            return false;

        return GameData.Instance.Data.currency >= cost;
    }

    public bool TryPurchase(UpgradeId id)
    {
        if (!CanPurchase(id))
            return false;

        int cost = GetNextLevelCost(id);
        if (!GameData.Instance.TrySpendCurrency(cost))
            return false;

        int newLevel = GetLevel(id) + 1;
        GameData.Instance.Data.SetUpgradeLevel(id, newLevel);

        SaveSystem.Save(GameData.Instance.Data);
        OnUpgradePurchased?.Invoke(id, newLevel);
        return true;
    }

    public float GetValue(UpgradeId id)
    {
        UpgradeDefinition upgrade = _catalog.Get(id);
        return upgrade != null ? upgrade.GetValueAtLevel(GetLevel(id)) : 0f;
    }

    public void ResetUpgrades(bool refundCurrency)
    {
        if (refundCurrency)
        {
            int refund = CalculateTotalSpent();   // ANTES de limpar
            GameData.Instance.AddCurrency(refund);
        }

        GameData.Instance.Data.ClearUpgrades();
        SaveSystem.Save(GameData.Instance.Data);

        OnUpgradesReset?.Invoke();
    }

    private int CalculateTotalSpent()
    {
        int total = 0;

        foreach (UpgradeDefinition definition in _catalog.Upgrades)
        {
            if (definition == null) continue;

            int level = GetLevel(definition.Id);
            for (int i = 1; i <= level; i++)
            {
                UpgradeLevel data = definition.GetLevel(i);
                if (data != null) total += data.cost;
            }
        }

        return total;
    }
}
