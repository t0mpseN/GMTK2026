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

    private float TickInterval => Config.TickInterval;
    private float SpawnRateBonus => UpgradeSystem.Instance.GetValue(UpgradeId.FoodSpawnPerSecond);
    private int StartingFoodQuantity => Mathf.RoundToInt(UpgradeSystem.Instance.GetValue(UpgradeId.StartingFoodQuantity));
    private float SpawnOnKillChance => UpgradeSystem.Instance.GetValue(UpgradeId.FoodSpawnOnKillChance);
    private float ExtraSpawnChance => UpgradeSystem.Instance.GetValue(UpgradeId.ExtraFoodSpawnChance);
    private int TotalUpgradeLevels => UpgradeSystem.Instance.TotalUpgradeLevels;


    // METHODS
    private void Awake()
    {
        Instance = this;
        _camera = Camera.main;
    }

    private void Start()
    {
        SpawnInitialBurst();
        StartCoroutine(SpawnLoop());
    }

    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(GetEffectiveTickInterval());

            foreach (SpawnEntry entry in _spawnEntries)
            {
                if (entry.Prefab == null)
                    continue;

                entry.Alive.RemoveAll(food => food == null);

                if (!Chance.Roll(GetEffectiveChance(entry)))
                    continue;

                TrySpawn(entry);

                if (Chance.Roll(ExtraSpawnChance))
                    TrySpawn(entry);
            }
        }
    }

    private float GetEffectiveChance(SpawnEntry entry)
    {
        return entry.SpawnChance + entry.ChancePerUpgradeLevel * TotalUpgradeLevels;
    }

    private float GetEffectiveTickInterval()
    {
        float baseRate = 1f / TickInterval;
        float totalRate = baseRate + SpawnRateBonus;

        return 1f / Mathf.Max(totalRate, 0.01f);
    }

    private void SpawnInitialBurst()
    {
        for (int i = 0; i < StartingFoodQuantity; i++)
        {
            SpawnEntry entry = PickRandomEntry();
            if (entry != null)
                TrySpawn(entry);
        }
    }

    private SpawnEntry PickRandomEntry()
    {
        float totalChance = 0f;
        foreach (SpawnEntry entry in _spawnEntries)
        {
            if (entry.Prefab != null)
                totalChance += GetEffectiveChance(entry);
        }

        if (totalChance <= 0f)
            return null;

        float roll = Random.Range(0f, totalChance);
        float accumulated = 0f;

        foreach (SpawnEntry entry in _spawnEntries)
        {
            if (entry.Prefab == null) continue;

            accumulated += GetEffectiveChance(entry);
            if (roll < accumulated)
                return entry;
        }

        return null;
    }

    public void NotifyFoodKilled(Food killed)
    {
        if (!Chance.Roll(SpawnOnKillChance))
            return;

        SpawnEntry entry = FindEntryFor(killed);
        if (entry == null)
            return;

        entry.Alive.RemoveAll(food => food == null);
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
        entry.Alive.RemoveAll(food => food == null);

        if (entry.Alive.Count >= entry.MaxAliveCount)
            return;

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

        for (int attempt = 0; attempt < Config.MaxPlacementAttempts; attempt++)
        {
            Vector2 candidate = center + new Vector2(
                Random.Range(-halfWidth, halfWidth),
                Random.Range(-halfHeight, halfHeight));

            if (_player != null && Vector2.Distance(candidate, _player.position) < Config.MinDistanceFromPlayer)
                continue;

            position = candidate;
            return true;
        }

        return false;
    }
}