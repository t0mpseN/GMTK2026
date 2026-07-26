using System;
using UnityEngine;

[Serializable]
public class UpgradeRequirement
{
    public UpgradeId upgrade;

    [Tooltip("Necessary level. 1 = base (always met). Use 2+ to require purchase.")]
    [Min(1)] public int minLevel = 1;
}