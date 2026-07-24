using UnityEngine;

[CreateAssetMenu(menuName = "Game/Config/Economy")]
public class EconomyConfig : ScriptableObject
{
    [SerializeField] private int _currencyPerJunkFood = 1;
    [SerializeField] private int _currencyPerHealthyFood = 3;
    [SerializeField] private float _timeBonusPerJunkFood = 2f;
    [SerializeField] private float _timeBonusPerHealthyFood = 5f;

    public int CurrencyPerJunkFood => _currencyPerJunkFood;
    public int CurrencyPerHealthyFood => _currencyPerHealthyFood;
    public float TimeBonusPerJunkFood => _timeBonusPerJunkFood;
    public float TimeBonusPerHealthyFood => _timeBonusPerHealthyFood;
}