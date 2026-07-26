using System.Collections;
using UnityEngine;

public class JunkFood : Food
{
    // FIELDS & PROPERTIES
    protected override float CurrencyReward => ConfigRegistry.Instance.Economy.CurrencyPerJunkFood;
    private float CurrencyLostPerJunkFoodEaten => ConfigRegistry.Instance.Economy.CurrencyLostPerJunkFoodEaten;
    protected override float TimeReward => 0f;
    private static readonly string[] JunkEatenPhrases =
    {
        "YUCK!", "TOO SWEET", "GREASY...", "SO BAD", "MY ARTERIES", "WORTH IT?", "OINK"
    };

    protected override string KilledMessage => $"+{CurrencyReward:0} kcal";
    protected override Color KilledMessageColor => FloatingTextSpawner.Instance.CurrencyColor;


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

    protected override EatenFeedback GetEatenFeedback() =>
        RollEatenFeedback(
            JunkEatenPhrases,
            $"-{CurrencyLostPerJunkFoodEaten:0} kcal",
            FloatingTextSpawner.Instance.PenaltyColor);
}
