using System;
using System.Collections;
using UnityEngine;

public class FoodSpawner : MonoBehaviour
{
    // FIELDS & PROPERTIES
    [SerializeField] private SpawnEntry[] _spawnEntries;
    [SerializeField] private Transform _player;

    [Header("Spawn Area")]
    [Tooltip("Margin from the edges of the screen where food will not spawn. This prevents food from spawning too close to the edges.")]
    [Range(0f, 0.4f)] [SerializeField] private float _spawnAreaPadding = 0.1f;

    [Tooltip("Radius where mobs can't spawn around the player.")]
    [SerializeField] private float _minDistanceFromPlayer = 3f;

    [SerializeField] private int _maxPlacementAttempts = 15;

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
            if (entry.Alive.Count >= entry.MaxAliveCount)
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

        float halfHeight = _camera.orthographicSize * (1f - _spawnAreaPadding);
        float halfWidth = halfHeight * _camera.aspect;
        Vector2 center = _camera.transform.position;

        for (int attempt = 0 ; attempt < _maxPlacementAttempts; attempt++)
        {
            Vector2 candidate = center + new Vector2(
                UnityEngine.Random.Range(-halfWidth, halfWidth),
                UnityEngine.Random.Range(-halfHeight, halfHeight));

            if (_player != null &&
                Vector2.Distance(candidate, _player.position) < _minDistanceFromPlayer)
                continue;

            position = candidate;
            return true;
        }

        return false;
    }
}
