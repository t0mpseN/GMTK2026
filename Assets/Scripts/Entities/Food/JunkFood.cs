using System.Collections;
using UnityEngine;

public class JunkFood : Food
{
    // FIELDS & PROPERTIES
    protected override int CurrencyReward => ConfigRegistry.Instance.Economy.CurrencyPerJunkFood + (int)UpgradeSystem.Instance.GetValue(UpgradeId.CurrencyPerKill);
    protected override float TimeReward => ConfigRegistry.Instance.Economy.TimeBonusPerJunkFood;


    // METHODS
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isDying || !collision.CompareTag("Player"))
            return;

        StartCoroutine(OnEatenByPlayer());
    }

    private IEnumerator OnEatenByPlayer()
    {
        spriteRenderer.color = deathColor;
        yield return new WaitForSeconds(deathDuration);

        GameData.Instance.RemoveCurrency(CurrencyReward);
        // TODO: Add visual feedback for the junk food being eaten by the player (e.g., play an animation, change color, etc.)
        Destroy(gameObject);
    }
}
