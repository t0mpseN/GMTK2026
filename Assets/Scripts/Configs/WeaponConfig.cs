using UnityEngine;

[CreateAssetMenu(menuName = "Game/Config/Weapon")]
public class WeaponConfig : ScriptableObject
{
    [SerializeField] private float _baseLateralRadius = 0.3f ;
    [SerializeField] private float _baseAttackDuration = 0.15f;

    public float BaseDamage => UpgradeSystem.Instance.GetValue(UpgradeId.WeaponDamage);
    public float BaseAttackDuration => _baseAttackDuration;
    public float BaseAttackCooldown => UpgradeSystem.Instance.GetValue(UpgradeId.AttackCooldown);
    public float BaseForwardRadius => UpgradeSystem.Instance.GetValue(UpgradeId.WeaponRange);
    public float BaseLateralRadius => _baseLateralRadius * UpgradeSystem.Instance.GetValue(UpgradeId.WeaponRange);
}