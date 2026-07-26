using System.Collections;
using UnityEngine;

public class JunkFood : Food
{
    // FIELDS & PROPERTIES
    protected override float CurrencyReward => ConfigRegistry.Instance.Economy.CurrencyPerJunkFood;
    protected override float TimeReward => 0f;


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
