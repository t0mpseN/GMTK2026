using UnityEngine;

[CreateAssetMenu(menuName = "Game/Config/Weapon")]
public class WeaponConfig : ScriptableObject
{
    [SerializeField] private float _baseLateralRadius = 0.3f ;
    [SerializeField] private float _baseAttackDuration = 0.15f;

    public float Damage => UpgradeSystem.Instance.GetValue(UpgradeId.WeaponDamage);
    public float AttackDuration => _baseAttackDuration;
    public float AttackCooldown => UpgradeSystem.Instance.GetValue(UpgradeId.AttackCooldown);
    public float ForwardRadius => UpgradeSystem.Instance.GetValue(UpgradeId.WeaponRange);
    public float LateralRadius => _baseLateralRadius * UpgradeSystem.Instance.GetValue(UpgradeId.WeaponRange);
}