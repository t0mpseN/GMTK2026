using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class Food : MonoBehaviour
{
    // FIELDS & PROPERTIES
    protected Rigidbody2D _rigidBody;
    protected Transform _target;
    private Vector2 _knockbackVelocity;
    private Coroutine _flashRoutine;
    public SpriteRenderer spriteRenderer;
    public Color baseColor;
    public bool isDying;

    [Header("Basic Properties")]
    [SerializeField] protected float _health = 1;
    public float Health => _health;
    [SerializeField] protected float _moveSpeed = 3f;
    public float MoveSpeed => _moveSpeed;
    protected abstract float CurrencyReward { get; }
    protected abstract float TimeReward { get; }

    [Header("Hit Feedback")]
    [SerializeField] private float _knockbackForce = 5f;
    [SerializeField] private float _knockbackDecay = 25f;
    [SerializeField] private float _flashDuration = 0.08f;
    [SerializeField] private Color _flashColor = new Color(3f, 3f, 3f, 1f);
    [SerializeField] public float deathDuration = 0.3f;
    [SerializeField] public Color deathColor = new Color(3f, 0.25f, 0.25f, 1f);

    public float DoubleCurrencyChance => UpgradeSystem.Instance.GetValue(UpgradeId.DoubleCurrencyChance);

    private Vector2 Bounds => ConfigRegistry.Instance.Spawn.WorldHalfExtents;

    [SerializeField] private float _suckDuration = 0.25f;
    [SerializeField] private AnimationCurve _suckCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    protected abstract string KilledMessage { get; }
    protected abstract Color KilledMessageColor { get; }
    [Range(0f, 1f)][SerializeField] private float _flavorTextChance = 0.3f;
    [SerializeField] private float _textOffset = 0.6f;


    // METHODS
    protected virtual void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        baseColor = spriteRenderer.color;
    }

    protected virtual void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            _target = player.transform;
        else
            Debug.LogWarning($"{name}: no player object was found");
    }

    protected virtual void FixedUpdate()
    {
        if (_target == null)
            return;

        Vector2 direction = GetMovementDirection();
        Vector2 delta = direction * _moveSpeed + _knockbackVelocity;
        Vector2 next = _rigidBody.position + delta * Time.fixedDeltaTime;
        Vector2 bounds = Bounds;
        next.x = Mathf.Clamp(next.x, -bounds.x, bounds.x);
        next.y = Mathf.Clamp(next.y, -bounds.y, bounds.y); 
        _rigidBody.MovePosition(next);

        _knockbackVelocity = Vector2.MoveTowards(
            _knockbackVelocity, Vector2.zero, _knockbackDecay * Time.fixedDeltaTime);
    }

    protected Vector2 DirectionToTarget()
    {
        return ((Vector2)_target.position - _rigidBody.position).normalized;
    }

    protected virtual Vector2 GetMovementDirection()
    {
        return DirectionToTarget();
    }

    public virtual void OnHitByWeapon(float damage, Vector2 hitSource)
    {
        AudioManager.Instance?.PlaySfx(AudioManager.Instance.FoodHitClip);
        FloatingTextSpawner.Instance?.Spawn(
            damage.ToString(),
            transform.position + Vector3.up * _textOffset, 
            FloatingTextSpawner.Instance.DamageColor);

        if (isDying)
            return;

        _health -= damage;

        if (_health <= 0)
        {
            StartCoroutine(OnKilled()); 
            return;
        }

        ApplyKnockback(hitSource);
        StartFlash();
    }
    
    protected virtual IEnumerator OnKilled()
    {
        isDying = true;

        float currency = RollCurrencyReward();

        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
            collider.enabled = false;

        if (_flashRoutine != null)
            StopCoroutine(_flashRoutine);

        spriteRenderer.color = deathColor;
        yield return new WaitForSeconds(deathDuration);

        if (TimeReward != 0f)
            GameTimer.Instance.AddTime(TimeReward);

        GameData.Instance.AddCurrency(currency);
        FoodSpawner.Instance?.NotifyFoodKilled(this);
        FloatingTextSpawner.Instance?.Spawn(
            KilledMessage,
            transform.position + Vector3.up * _textOffset,
            KilledMessageColor);

        Destroy(gameObject);
    }

    protected abstract IEnumerator OnEatenByPlayer(Collider2D eater);

    protected void BeginEaten(Collider2D eater)
    {
        AudioManager.Instance?.PlaySfx(AudioManager.Instance.FoodEatenClip);
        isDying = true;

        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
            collider.enabled = false;

        PlayerAnimator animator = eater.GetComponentInParent<PlayerAnimator>();
        animator?.PlayEat();
        EatenFeedback feedback = GetEatenFeedback();
        FloatingTextSpawner.Instance?.Spawn(feedback.message, transform.position, feedback.color);
    }

    private void ApplyKnockback(Vector2 hitSource)
    {
        Vector2 away = ((Vector2)transform.position - hitSource).normalized;

        if (away.sqrMagnitude < 0.0001f)
            away = Random.insideUnitCircle.normalized; // acertado exatamente no centro

        _knockbackVelocity = away * _knockbackForce;
    }

    private void StartFlash()
    {
        if (_flashRoutine != null)
            StopCoroutine(_flashRoutine);

        _flashRoutine = StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        spriteRenderer.color = _flashColor;
        yield return new WaitForSeconds(_flashDuration);
        spriteRenderer.color = baseColor;
        _flashRoutine = null;
    }

    protected float RollCurrencyReward()
    {
        float reward = CurrencyReward;

        if (Chance.Roll(DoubleCurrencyChance))
            reward *= 2;

        return reward;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isDying || !collision.CompareTag("Player"))
            return;

        StartCoroutine(OnEatenByPlayer(collision));
    }


    public IEnumerator SuckIntoPlayer(Transform target)
    {
        spriteRenderer.sortingOrder = 1000;

        Vector3 startPos = transform.position;
        Vector3 startScale = transform.localScale;
        float elapsed = 0f;

        while (elapsed < _suckDuration)
        {
            float t = _suckCurve.Evaluate(elapsed / _suckDuration);
            transform.position = Vector3.Lerp(startPos, target.position, t);
            transform.localScale = Vector3.Lerp(startScale, Vector3.zero, t);

            elapsed += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }

    protected struct EatenFeedback
    {
        public string message;
        public Color color;
    }

    protected EatenFeedback RollEatenFeedback(string[] phrases, string fallback, Color fallbackColor)
    {
        if (Chance.Roll(_flavorTextChance) && phrases.Length > 0)
        {
            Color color;
            if (TimeReward > 0f)
                color = FloatingTextSpawner.Instance.EatenSpecialColor;
            else
                color = fallbackColor;

            return new EatenFeedback
            {
                message = phrases[Random.Range(0, phrases.Length)],
                color = color
            };
        }

        return new EatenFeedback { message = fallback, color = fallbackColor };
    }

    protected abstract EatenFeedback GetEatenFeedback();
}
