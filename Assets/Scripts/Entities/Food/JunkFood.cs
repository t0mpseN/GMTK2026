using System.Collections;
using UnityEngine;

public class JunkFood : Food
{
    // FIELDS & PROPERTIES
    protected override float CurrencyReward => ConfigRegistry.Instance.Economy.CurrencyPerJunkFood;
    private float CurrencyLostPerJunkFoodEaten => ConfigRegistry.Instance.Economy.CurrencyLostPerJunkFoodEaten;
    protected override float TimeReward => 0f;


    // METHODS
    protected override IEnumerator OnEatenByPlayer(Collider2D eater)
    {
        BeginEaten(eater);

        //spriteRenderer.color = deathColor;
        yield return new WaitForSeconds(deathDuration);

        GameData.Instance.RemoveCurrency(CurrencyLostPerJunkFoodEaten);
        FoodSpawner.Instance?.NotifyFoodKilled(this);

        yield return SuckIntoPlayer(eater.transform);
    }
}
