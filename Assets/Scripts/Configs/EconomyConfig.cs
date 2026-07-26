using UnityEngine;

[CreateAssetMenu(menuName = "Game/Config/Economy")]
public class EconomyConfig : ScriptableObject
{
    [SerializeField] private float _timeBonusPerJunkFood = 2f;
    public float TimeBonusPerJunkFood => _timeBonusPerJunkFood;

    public float CurrencyPerJunkFood => UpgradeSystem.Instance.GetValue(UpgradeId.CurrencyPerJunkFood);
    public float CurrencyPerHealthyFood => UpgradeSystem.Instance.GetValue(UpgradeId.CurrencyPerHealthyFood);
    public float TimeBonusPerHealthyFood => UpgradeSystem.Instance.GetValue(UpgradeId.EnergyRecoveryOnHealthyFoodKill);
}