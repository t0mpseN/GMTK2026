using UnityEngine;

[System.Serializable]
public class UpgradeRequirement
{
    public UpgradeId upgrade;
    [Min(1)] public int minLevel = 1;
}