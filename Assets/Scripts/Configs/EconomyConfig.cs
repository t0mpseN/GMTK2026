using UnityEngine;

[CreateAssetMenu(menuName = "Game/Config/Economy")]
public class EconomyConfig : ScriptableObject
{
    [SerializeField] private float _timeBonusPerJunkFood = 2f;
    [SerializeField] private float _timeBonusPerHealthyFood = 5f;

    public float CurrencyPerJunkFood => UpgradeSystem.Instance.GetValue(UpgradeId.CurrencyPerJunkFood);
    public float CurrencyPerHealthyFood => UpgradeSystem.Instance.GetValue(UpgradeId.CurrencyPerHealthyFood);
    public float TimeBonusPerJunkFood => _timeBonusPerJunkFood;
    public float TimeBonusPerHealthyFood => _timeBonusPerHealthyFood;
}