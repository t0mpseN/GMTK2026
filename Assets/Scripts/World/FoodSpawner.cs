using System.Collections;
using UnityEngine;

public class FoodSpawner : MonoBehaviour
{
    // FIELDS & PROPERTIES
    [SerializeField] private SpawnEntry[] _spawnEntries;
    [SerializeField] private Transform _player;
    private SpawnConfig Config => ConfigRegistry.Instance.Spawn;
    private Camera _camera;


    // METHODS
    private void Awake()
    {
        _camera = Camera.main;
    }

    private void Start()
    {
        foreach (SpawnEntry entry in _spawnEntries)
        {
            if (entry.Prefab == null)
                continue;

            StartCoroutine(SpawnLoop(entry));
        }
    }

    private IEnumerator SpawnLoop(SpawnEntry entry)
    {
        while (true)
        {
            yield return new WaitForSeconds(entry.SpawnInterval);

            entry.Alive.RemoveAll(food => food == null); // Clean up dead references

            int maxAliveCount = entry.MaxAliveCount;
            if (entry.Prefab is HealthyFood)
                maxAliveCount += Mathf.RoundToInt(UpgradeSystem.Instance.GetValue(UpgradeId.MaxHealthyFoodIncrease));

            if (entry.Alive.Count >= maxAliveCount)
                continue;

            if (TryGetSpawnPosition(out Vector2 position))
            {
                Food foodInstance = Instantiate(entry.Prefab, position, Quaternion.identity);
                entry.Alive.Add(foodInstance);
            }
        }
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

            if (_player != null &&
                Vector2.Distance(candidate, _player.position) < Config.MinDistanceFromPlayer)
                continue;

            position = candidate;
            return true;
        }

        return false;
    }
}
