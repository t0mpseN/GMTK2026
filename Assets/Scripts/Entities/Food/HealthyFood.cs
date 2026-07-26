using System.Collections;
using UnityEngine;

public class HealthyFood : Food
{
    // FIELDS & PROPERTIES
    protected override float CurrencyReward => ConfigRegistry.Instance.Economy.CurrencyPerHealthyFood + UpgradeSystem.Instance.GetValue(UpgradeId.CurrencyPerHealthyFood);
    protected override float TimeReward => ConfigRegistry.Instance.Economy.TimeBonusPerHealthyFood + UpgradeSystem.Instance.GetValue(UpgradeId.EnergyRecoveryOnHealthyFoodKill);
    [SerializeField] private float _fleeDetectionRadius = 5f;


    // METHODS
    protected override Vector2 GetMovementDirection()
    {
        if (_fleeDetectionRadius > 0f && _target != null)
        {
            float distanceToTarget = Vector2.Distance(_rigidBody.position, _target.position);
            if (distanceToTarget > _fleeDetectionRadius)
                return Vector2.zero;
        }

        return -DirectionToTarget();
    }

    protected override IEnumerator OnEatenByPlayer()
    {
        float currency = RollCurrencyReward();

        spriteRenderer.color = deathColor;
        yield return new WaitForSeconds(deathDuration);

        GameTimer.Instance.AddTime(TimeReward);
        GameData.Instance.AddCurrency(currency);

        Destroy(gameObject);
    }
}
