using UnityEngine;

[CreateAssetMenu(menuName = "Game/Config/Spawn")]
public class SpawnConfig : ScriptableObject
{
    // FIELDS & PROPERTIES
    [SerializeField] private float _spawnAreaPadding = 0.1f;
    public float SpawnAreaPadding => _spawnAreaPadding;

    [SerializeField] private float _minDistanceFromPlayer = 3f;
    public float MinDistanceFromPlayer => _minDistanceFromPlayer;

    [SerializeField] private int _maxPlacementAttempts = 15;
    public int MaxPlacementAttempts => _maxPlacementAttempts;
}