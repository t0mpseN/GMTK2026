using UnityEngine;
using System;

[Serializable]
public class UpgradeLevel
{
    // FIELDS
    [Min(0)] public int cost = 10;
    [Tooltip("Value at the CURRENT upgrade level")]
    public float value = 1f;
    public string description;
}
