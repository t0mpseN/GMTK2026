using System;
using System.Collections.Generic;

[Serializable]
public class PlayerData
{
    // FIELDS
    public int currency;
    public List<UpgradeProgress> upgrades = new List<UpgradeProgress>();
    
    
    // METHODS
    public int GetUpgradeLevel(UpgradeId id)
    {
        foreach (UpgradeProgress upgrade in upgrades)
        {
            if (upgrade.id == id)
                return upgrade.level;
        }

        return 0;
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
}
