using UnityEngine;

public class HealthyFood : Food
{
    // FIELDS & PROPERTIES
    protected override int CurrencyReward => ConfigRegistry.Instance.Economy.CurrencyPerHealthyFood + UpgradeSystem.Instance.GetValue(UpgradeId.CurrencyPerKill);
    protected override float TimeReward => ConfigRegistry.Instance.Economy.TimeBonusPerHealthyFood;
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

    protected override void OnDeath()
    {
        GameTimer.Instance.AddTime(TimeReward);
        GameData.Instance.AddCurrency(CurrencyReward);
        Destroy(gameObject);
    }
}
