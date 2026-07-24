using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class WeaponHitBox : MonoBehaviour
{
    // FIELDS & PROPERTIES
    private WeaponConfig _config;
    private WeaponConfig Config => _config != null ? _config : _config = ConfigRegistry.Instance.Weapon;

    protected virtual int AttackDamage => Config.BaseDamage + UpgradeSystem.Instance.GetValue(UpgradeId.AttackDamage);
    protected virtual float AttackDuration => Config.BaseAttackDuration;
    protected virtual float AttackCooldown => Config.BaseAttackCooldown;
    protected virtual float AttackScaleY => Config.BaseAttackScaleY + UpgradeSystem.Instance.GetValue(UpgradeId.AttackRange);

    private BoxCollider2D _collider;
    private SpriteRenderer _spriteRenderer;
    private PlayerControls _playerControls;
    private InputAction _attackAction;

    [SerializeField] private Color _attackColor = Color.red;
    private Color _idleColor;
    private Vector3 _idleScale;
    private float _lastAttackTime = float.NegativeInfinity;

    private bool _isAttacking;
    public bool IsAttacking => _isAttacking;

    // METHODS
    private void Awake()
    {
        _collider = GetComponent<BoxCollider2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _idleColor = _spriteRenderer.color;
        _idleScale = transform.localScale;
        _playerControls = new PlayerControls();
        _attackAction = _playerControls.Player.Attack;
        _collider.enabled = false;
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

        // Change visuals for attack
        _spriteRenderer.color = _attackColor;
        transform.localScale = new Vector3(_idleScale.x, _idleScale.y * AttackScaleY, _idleScale.z);
        _collider.enabled = true;
        yield return new WaitForSeconds(AttackDuration);

        // Reset visuals after attack
        _collider.enabled = false;
        transform.localScale = _idleScale;
        _spriteRenderer.color = _idleColor;
        _isAttacking = false;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (!_isAttacking)
            return;

        Food food = collider.GetComponent<Food>();
        if (food == null)
            return;

        food.OnHitByWeapon(AttackDamage);
        // TODO: Add visual feedback for the food being hit by the weapon (e.g., play an animation, change color, etc.)
    }
}