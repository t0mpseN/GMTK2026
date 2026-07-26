using System.Collections;
using UnityEngine;

public class HealthyFood : Food
{
    // FIELDS & PROPERTIES
    protected override float CurrencyReward => ConfigRegistry.Instance.Economy.CurrencyPerHealthyFood;
    protected override float TimeReward => ConfigRegistry.Instance.Economy.TimeBonusPerHealthyFood;
    [SerializeField] private float _fleeDetectionRadius = 5f;
    private static readonly string[] HealthyEatenPhrases =
    {
        "YUM!", "FRESH!", "CRUNCH", "SO GOOD", "GAINS", "NICE", "*happy*"
    };

    protected override string KilledMessage => $"+{CurrencyReward:0} kcal";
    protected override Color KilledMessageColor => FloatingTextSpawner.Instance.CurrencyColor;

    [Header("Color Variants")]
    [SerializeField]
    private Color[] _colorVariants =
    {
        new Color(1f, 0.15f, 0.15f),   
        new Color(0.2f, 0.85f, 0.15f),
        new Color(1f, 0.8f, 0.1f)     
    };

    // METHODS 
    protected override void Awake()
    {
        base.Awake();   // primeiro — pega renderer, define _baseColor = cinza

        Color chosen = _colorVariants[Random.Range(0, _colorVariants.Length)];
        spriteRenderer.color = chosen;
        baseColor = chosen;   // sobrescreve DEPOIS
    }

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

    protected override IEnumerator OnEatenByPlayer(Collider2D eater)
    {
        BeginEaten(eater);

        float currency = RollCurrencyReward();

        //spriteRenderer.color = deathColor;
        yield return new WaitForSeconds(deathDuration);

        FoodSpawner.Instance?.NotifyFoodKilled(this);
        GameTimer.Instance.AddTime(TimeReward);
        GameData.Instance.AddCurrency(currency);

        yield return SuckIntoPlayer(eater.transform);
    }

    protected override EatenFeedback GetEatenFeedback() =>
        RollEatenFeedback(
            HealthyEatenPhrases,
            $"+{CurrencyReward:0} kcal",
            FloatingTextSpawner.Instance.CurrencyColor);
}
