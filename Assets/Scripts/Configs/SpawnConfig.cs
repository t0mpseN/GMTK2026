using UnityEngine;

[CreateAssetMenu(menuName = "Game/Config/Spawn")]
public class SpawnConfig : ScriptableObject
{
    // FIELDS & PROPERTIES
    [SerializeField] private float _spawnAreaPadding = 0.1f;
    [SerializeField] private float _minDistanceFromPlayer = 3f;
    [SerializeField] private int _maxPlacementAttempts = 15;
    [SerializeField] private Vector2 _worldHalfExtents = new Vector2(16f, 16f);

    public float SpawnAreaPadding => _spawnAreaPadding;
    public float MinDistanceFromPlayer => _minDistanceFromPlayer;
    public int MaxPlacementAttempts => _maxPlacementAttempts;
    public Vector2 WorldHalfExtents => _worldHalfExtents;
}