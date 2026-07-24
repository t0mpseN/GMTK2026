using UnityEngine;

[CreateAssetMenu(menuName = "Game/Upgrade Definition", fileName = "Upgrade_")]
public class UpgradeDefinition : ScriptableObject
{
    // FIELDS & PROPERTIES
    [SerializeField] private UpgradeId _id;
    [SerializeField] private string _displayName;
    [SerializeField] private Sprite _icon;

    [Tooltip("Index 0 = Level 1. Array size defines max upgrade level")]
    [SerializeField] private UpgradeLevel[] _levels;

    public UpgradeId Id => _id;
    public string DisplayName => _displayName;
    public Sprite Icon => _icon;
    public int MaxLevel => _levels.Length;


    // METHODS
    public UpgradeLevel GetLevel(int level)
    {
        if (level < 1 || level > _levels.Length)
            return null;

        return _levels[level - 1];
    }

    public int GetValueAtLevel(int level)
    {
        UpgradeLevel data = GetLevel(level);
        return data != null ? data.value : 0;
    }
}
