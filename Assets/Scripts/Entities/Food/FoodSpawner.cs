using System;
using System.Collections;
using UnityEngine;

public class FoodSpawner : MonoBehaviour
{
    // FIELDS & PROPERTIES
    public static FoodSpawner Instance { get; private set; }

    [SerializeField] private SpawnEntry[] _spawnEntries;
    [SerializeField] private Transform _player;
    private SpawnConfig Config => ConfigRegistry.Instance.Spawn;
    private Camera _camera;

    private float SpawnRateBonus => UpgradeSystem.Instance.GetValue(UpgradeId.FoodSpawnPerSecond);
    private int StartingFoodQuantity => Mathf.RoundToInt(UpgradeSystem.Instance.GetValue(UpgradeId.StartingFoodQuantity));
    private float SpawnOnKillChance => UpgradeSystem.Instance.GetValue(UpgradeId.FoodSpawnOnKillChance);
    private float ExtraSpawnChance => UpgradeSystem.Instance.GetValue(UpgradeId.ExtraFoodSpawnChance);


    // METHODS
    private void Awake()
    {
        _camera = Camera.main;
    }

    private void Start()
    {
        SpawnInitialBurst();

        foreach (SpawnEntry entry in _spawnEntries)
        {
            if (entry.Prefab == null)
                continue;

            StartCoroutine(SpawnLoop(entry));
        }
    }

    private void SpawnInitialBurst()
    {
        int amount = StartingFoodQuantity;
        if (amount <= 0 || _spawnEntries.Length == 0)
            return;

        for (int i = 0; i < amount; i++)
        {
            SpawnEntry entry = _spawnEntries[UnityEngine.Random.Range(0, _spawnEntries.Length)];
            if (entry.Prefab != null)
                TrySpawn(entry);
        }
    }

    private IEnumerator SpawnLoop(SpawnEntry entry)
    {
        while (true)
        {
            yield return new WaitForSeconds(GetEffectiveInterval(entry));

            entry.Alive.RemoveAll(food => food == null);

            if (entry.Alive.Count >= entry.MaxAliveCount)
                continue;

            TrySpawn(entry);

            if (Chance.Roll(ExtraSpawnChance) && entry.Alive.Count < entry.MaxAliveCount)
                TrySpawn(entry);
        }
    }

    private float GetEffectiveInterval(SpawnEntry entry)
    {
        float baseRate = 1f / Mathf.Max(entry.SpawnInterval, 0.05f);
        float totalRate = baseRate + SpawnRateBonus;

        return 1f / Mathf.Max(totalRate, 0.01f);
    }

    public void NotifyFoodKilled(Food killed)
    {
        if (!Chance.Roll(SpawnOnKillChance))
            return;

        SpawnEntry entry = FindEntryFor(killed);
        if (entry == null)
            return;

        entry.Alive.RemoveAll(food => food == null);

        if (entry.Alive.Count < entry.MaxAliveCount)
            TrySpawn(entry);
    }

    private SpawnEntry FindEntryFor(Food food)
    {
        foreach (SpawnEntry entry in _spawnEntries)
        {
            if (entry.Prefab != null && entry.Prefab.GetType() == food.GetType())
                return entry;
        }

        return null;
    }

    private void TrySpawn(SpawnEntry entry)
    {
        if (!TryGetSpawnPosition(out Vector2 position))
            return;

        Food instance = Instantiate(entry.Prefab, position, Quaternion.identity);
        entry.Alive.Add(instance);
    }

    private bool TryGetSpawnPosition(out Vector2 position)
    {
        position = default;
        if (_camera == null)
            return false;

        float halfHeight = _camera.orthographicSize * (1f - Config.SpawnAreaPadding);
        float halfWidth = halfHeight * _camera.aspect;
        Vector2 center = _camera.transform.position;

        for (int attempt = 0 ; attempt < Config.MaxPlacementAttempts; attempt++)
        {
            Vector2 candidate = center + new Vector2(
                UnityEngine.Random.Range(-halfWidth, halfWidth),
                UnityEngine.Random.Range(-halfHeight, halfHeight));

            if (_player != null && Vector2.Distance(candidate, _player.position) < Config.MinDistanceFromPlayer)
                continue;

            position = candidate;

            return true;
        }

        return false;
    }
}
