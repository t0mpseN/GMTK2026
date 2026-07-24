using UnityEngine;

public class JunkFood : Food
{
    // FIELDS & PROPERTIES
    protected override int CurrencyReward => ConfigRegistry.Instance.Economy.CurrencyPerJunkFood + UpgradeSystem.Instance.GetValue(UpgradeId.CurrencyPerKill);
    protected override float TimeReward => ConfigRegistry.Instance.Economy.TimeBonusPerJunkFood;


    // METHODS
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;

        OnEatenByPlayer();
    }

    private void OnEatenByPlayer()
    {
        GameData.Instance.RemoveCurrency(CurrencyReward);
        // TODO: Add visual feedback for the junk food being eaten by the player (e.g., play an animation, change color, etc.)
        Destroy(gameObject);
    }

    protected override void OnDeath()
    {
        GameTimer.Instance.AddTime(TimeReward);
        GameData.Instance.AddCurrency(CurrencyReward); 
        Destroy(gameObject);
    }
}
