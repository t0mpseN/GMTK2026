using UnityEngine;

public class JunkFood : Food
{
    // METHODS
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;

        OnEatenByPlayer();
    }

    private void OnEatenByPlayer()
    {
        GameData.Instance.RemoveCurrency(Currency);
        // TODO: Add visual feedback for the junk food being eaten by the player (e.g., play an animation, change color, etc.)
        Destroy(gameObject);
    }

    protected override void OnDeath()
    {
        GameTimer.Instance.AddTime(BonusTime);
        GameData.Instance.AddCurrency(Currency); 
        Destroy(gameObject);
    }
}
