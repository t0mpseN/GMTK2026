using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SpawnEntry
{
    // FIELDS
    public string Label = "Food";
    public Food Prefab;

    [Tooltip("Spawn chance per spawner tick.")]
    [Range(0f, 1f)] public float SpawnChance = 0.5f;

    [Tooltip("Additional spawn chance per player's upgrade level.")]
    public float ChancePerUpgradeLevel = 0f;

    [Min(1)] public int MaxAliveCount = 10;
    [HideInInspector] public List<Food> Alive = new List<Food>();
}