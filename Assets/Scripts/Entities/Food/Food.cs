using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class Food : MonoBehaviour
{
    // FIELDS & PROPERTIES
    protected Rigidbody2D _rigidBody;
    protected Transform _target;
    public SpriteRenderer spriteRenderer;
    private Color _baseColor;
    private Vector2 _knockbackVelocity;
    private Coroutine _flashRoutine;
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


    // METHODS
    protected virtual void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        _baseColor = spriteRenderer.color;
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

        _rigidBody.MovePosition(_rigidBody.position + delta * Time.fixedDeltaTime);

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

        GameTimer.Instance.AddTime(TimeReward);
        GameData.Instance.AddCurrency(currency); 
        Destroy(gameObject);
    }

    protected abstract IEnumerator OnEatenByPlayer();

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
        spriteRenderer.color = _baseColor;
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

        StartCoroutine(OnEatenByPlayer());
    }
}
