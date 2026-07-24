using UnityEngine;

[CreateAssetMenu(menuName = "Game/Config/Weapon")]
public class WeaponConfig : ScriptableObject
{
    [SerializeField] private int _baseDamage = 1;
    [SerializeField] private float _baseAttackDuration = 0.15f;
    [SerializeField] private float _baseAttackCooldown = 0.35f;
    [SerializeField] private float _baseAttackScaleY = 3f;

    public int BaseDamage => _baseDamage;
    public float BaseAttackDuration => _baseAttackDuration;
    public float BaseAttackCooldown => _baseAttackCooldown;
    public float BaseAttackScaleY => _baseAttackScaleY;
}