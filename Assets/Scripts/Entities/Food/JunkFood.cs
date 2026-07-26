using System.Collections;
using UnityEngine;

public class JunkFood : Food
{
    // FIELDS & PROPERTIES
    protected override float CurrencyReward => ConfigRegistry.Instance.Economy.CurrencyPerJunkFood 
        + UpgradeSystem.Instance.GetValue(UpgradeId.CurrencyPerJunkFood);
    protected override float TimeReward => ConfigRegistry.Instance.Economy.TimeBonusPerJunkFood;


    // METHODS
    protected override IEnumerator OnEatenByPlayer()
    {
        isDying = true;

        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
            collider.enabled = false;

        spriteRenderer.color = deathColor;
        yield return new WaitForSeconds(deathDuration);

        GameData.Instance.RemoveCurrency(CurrencyReward);
        FoodSpawner.Instance?.NotifyFoodKilled(this);

        Destroy(gameObject);
    }
}
