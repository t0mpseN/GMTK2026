using System;
using System.Collections.Generic;

[Serializable]
public class PlayerData
{
    // FIELDS
    public float currency;
    public List<UpgradeProgress> upgrades = new List<UpgradeProgress>();
    public const int BaseLevel = 0;

    // METHODS
    public int GetUpgradeLevel(UpgradeId id)
    {
        foreach (UpgradeProgress upgrade in upgrades)
            if (upgrade.id == id)
                return upgrade.level;

        return BaseLevel;
    }

    public void SetUpgradeLevel(UpgradeId id, int level)
    {
        foreach (UpgradeProgress upgrade in upgrades)
        {
            if (upgrade.id == id)
            {
                upgrade.level = level;
                return;
            }
        }

        upgrades.Add(new UpgradeProgress { id = id, level = level });
    }

    public void ClearUpgrades()
    {
        upgrades.Clear();
    }

    public int GetTotalUpgradeLevels()
    {
        int total = 0;

        foreach (UpgradeProgress upgrade in upgrades)
            total += upgrade.level - BaseLevel;   

        return total;
    }
}
