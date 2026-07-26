using UnityEngine;

[CreateAssetMenu(menuName = "Game/Config/Run")]
public class RunConfig : ScriptableObject
{
    [SerializeField] private float _warningThreshold = 10f;

    public float StartingTime => UpgradeSystem.Instance.GetValue(UpgradeId.EnergyPerRun);
    public float MaxTime => UpgradeSystem.Instance.GetValue(UpgradeId.EnergyPerRun);
    public float WarningThreshold => _warningThreshold;
}