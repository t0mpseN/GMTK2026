using UnityEngine;

[CreateAssetMenu(menuName = "Game/Upgrade Catalog")]
public class UpgradeCatalog : ScriptableObject
{
    // FIELDS & PROPERTIES
    [SerializeField] private UpgradeDefinition[] _upgrades;
    public UpgradeDefinition[] Upgrades => _upgrades;


    // METHODS
    public UpgradeDefinition Get(UpgradeId id)
    {
        foreach (UpgradeDefinition upgrade in _upgrades)
        {
            if (upgrade != null && upgrade.Id == id)
                return upgrade;
        }

        return null;
    }
}