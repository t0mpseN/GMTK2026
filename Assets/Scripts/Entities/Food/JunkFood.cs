using System.Collections;
using UnityEngine;

public class JunkFood : Food
{
    // FIELDS & PROPERTIES
    protected override float CurrencyReward => ConfigRegistry.Instance.Economy.CurrencyPerJunkFood + UpgradeSystem.Instance.GetValue(UpgradeId.CurrencyPerHealthyFood);
    protected override float TimeReward => ConfigRegistry.Instance.Economy.TimeBonusPerJunkFood;


    // METHODS
    protected override IEnumerator OnEatenByPlayer()
    {
        spriteRenderer.color = deathColor;

        yield return new WaitForSeconds(deathDuration);

        GameData.Instance.RemoveCurrency(CurrencyReward);

        Destroy(gameObject);
    }
}
