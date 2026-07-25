using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SpawnEntry
{
    // FIELDS
    public string Label = "Food";
    public Food Prefab;
    [Min(0.05f)] public float SpawnInterval = 3f;
    [Min(1)] public int MaxAliveCount = 10;
    [HideInInspector] public List<Food> Alive = new List<Food>();
}