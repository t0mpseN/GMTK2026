using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponHitBox : MonoBehaviour
{
    // FIELDS & PROPERTIES
    [SerializeField] private SlashVisual _slashVisual; 
    [SerializeField] private LayerMask _foodLayers;
    [SerializeField] private SpriteRenderer _weaponRenderer;
    [SerializeField] private Color _attackColor = Color.red;
    private WeaponAim _weaponAim;

    private Vector2 _attackOrigin;
    private Vector2 _attackForward;
    private Vector2 _attackLateral;

    private PlayerControls _playerControls;
    private InputAction _attackAction;
    private Color _idleColor;
    private float _lastAttackTime = float.NegativeInfinity;

    private WeaponConfig _config;
    private WeaponConfig Config => _config != null ? _config : _config = ConfigRegistry.Instance.Weapon;

    private bool _isAttacking;
    public bool IsAttacking => _isAttacking;

    private float RangeScale => UpgradeSystem.Instance.GetValue(UpgradeId.WeaponRange);
    private float ForwardRadius => Config.ForwardRadius * RangeScale;
    private float LateralRadius => Config.LateralRadius * RangeScale;
    protected virtual float AttackDamage => Config.Damage;
    protected virtual float AttackDuration => Config.AttackDuration;
    protected virtual float AttackCooldown => Config.AttackCooldown;

    // DEBUGGING
    [SerializeField] private HitboxVisual _hitboxVisual;


    // METHODS
    private void Awake()
    {
        _weaponAim = GetComponent<WeaponAim>();

        if (_weaponRenderer != null)
            _idleColor = _weaponRenderer.color;

        _playerControls = new PlayerControls();
        _attackAction = _playerControls.Player.Attack;
    }

    private void OnEnable()
    {
        _attackAction.Enable();
        _attackAction.performed += OnAttackPerformed;
    }

    private void OnDisable()
    {
        _attackAction.performed -= OnAttackPerformed;
        _attackAction.Disable();
    }

    private void OnDestroy()
    {
        _playerControls.Dispose();
    }

    private void OnAttackPerformed(InputAction.CallbackContext context)
    {
        if (AttackDamage <= 0f)
            return;

        if (_isAttacking)
            return;

        if (Time.time - _lastAttackTime < AttackCooldown)
            return;

        StartCoroutine(PerformAttack());
    }

    private IEnumerator PerformAttack()
    {
        _isAttacking = true;
        _lastAttackTime = Time.time;

        _attackOrigin = transform.position;
        _attackForward = transform.right;
        _attackLateral = transform.up;

        ResolveHits();

        //_hitboxVisual?.Show(ForwardRadius, LateralRadius);
        _slashVisual?.Play(ForwardRadius, LateralRadius, AttackDuration);

        yield return new WaitForSeconds(AttackDuration);

        //_hitboxVisual?.Hide();

        _isAttacking = false;
    }

    private void ResolveHits()
    {
        float queryRadius = Mathf.Max(ForwardRadius, LateralRadius);
        Collider2D[] candidates = Physics2D.OverlapCircleAll(_attackOrigin, queryRadius, _foodLayers);

        foreach (Collider2D candidate in candidates)
        {
            if (!IsInsideHitArea(candidate.bounds.center))
                continue;

            Food food = candidate.GetComponent<Food>();
            if (food != null)
                food.OnHitByWeapon(AttackDamage, _attackOrigin);
        }
    }

    private bool IsInsideHitArea(Vector2 point)
    {
        Vector2 toTarget = point - _attackOrigin;

        float forwardDistance = Vector2.Dot(toTarget, _attackForward);
        if (forwardDistance < 0f)
            return false;

        float lateralDistance = Vector2.Dot(toTarget, _attackLateral);

        float normalizedForward = forwardDistance / ForwardRadius;
        float normalizedLateral = lateralDistance / LateralRadius;

        return normalizedForward * normalizedForward + normalizedLateral * normalizedLateral <= 1f;
    }

    // DEBUG HITBOX
    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;
        if (ConfigRegistry.Instance == null || UpgradeSystem.Instance == null) return;

        Gizmos.color = _isAttacking ? Color.red : new Color(1f, 1f, 1f, 0.35f);

        Vector3 origin = transform.position;
        float a = ForwardRadius;
        float b = LateralRadius;

        const int segments = 24;
        Vector3 previous = origin + transform.up * -b;

        for (int i = 1; i <= segments; i++)
        {
            float t = Mathf.Lerp(-Mathf.PI / 2f, Mathf.PI / 2f, i / (float)segments);
            Vector3 current = origin
                + transform.right * (a * Mathf.Cos(t))
                + transform.up * (b * Mathf.Sin(t));

            Gizmos.DrawLine(previous, current);
            previous = current;
        }

        Gizmos.DrawLine(origin + transform.up * b, origin + transform.up * -b); // flat back edge
    }
}