using UnityEngine;

[CreateAssetMenu(menuName = "Game/Config/Run")]
public class RunConfig : ScriptableObject
{
    [SerializeField] private float _startingTime = 60f;
    [SerializeField] private float _maxTime = 60f;
    [SerializeField] private float _warningThreshold = 10f;

    public float StartingTime => _startingTime;
    public float MaxTime => _maxTime;
    public float WarningThreshold => _warningThreshold;
}