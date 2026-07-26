using UnityEngine;

[CreateAssetMenu(menuName = "Game/Upgrade Definition", fileName = "Upgrade_")]
public class UpgradeDefinition : ScriptableObject
{
    // FIELDS & PROPERTIES
    [SerializeField] private UpgradeId _id;
    public UpgradeId Id => _id;

    [SerializeField] private string _displayName;
    public string DisplayName => _displayName;

    [SerializeField] private Sprite _icon;
    public Sprite Icon => _icon;

    [Tooltip("Index 0 = Level 1. Array size defines max upgrade level")]
    [SerializeField] private UpgradeLevel[] _levels;
    public int MaxLevel => Mathf.Max(0, _levels.Length - 1);

    [SerializeField] private UpgradeRequirement[] _requirements;
    public UpgradeRequirement[] Requirements => _requirements;

    [SerializeField] private float _baseValue;


    // METHODS
    public UpgradeLevel GetLevel(int level)
    {
        if (level < 0 || level >= _levels.Length) return null;
        return _levels[level];
    }

    public float GetValueAtLevel(int level)
    {
        UpgradeLevel data = GetLevel(level);
        return data != null ? data.value : 0f;
    }
}
