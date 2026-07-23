using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class WeaponHitBox : MonoBehaviour
{
    // FIELDS & PROPERTIES
    [Header("Attack")]
    [SerializeField] private float _attackDuration = 0.5f;
    [SerializeField] private float _attackCooldown = 1f;

    [Header("Visuals")]
    [SerializeField] private float _attackScaleY = 3f;
    [SerializeField] private Color _attackColor = Color.red;

    private Collider2D _collider;
    private SpriteRenderer _spriteRenderer;
    private PlayerControls _playerControls;
    private InputAction _attackAction;

    private Color _idleColor;
    private Vector3 _idleScale;
    private float _lastAttackTime = float.NegativeInfinity;

    private bool _isAttacking;
    public bool IsAttacking => _isAttacking;

    // METHODS
    private void Awake()
    {
        _collider = GetComponent<Collider2D>();
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

        if (Time.time - _lastAttackTime < _attackCooldown)
            return;

        StartCoroutine(PerformAttack());
    }

    private IEnumerator PerformAttack()
    {
        _isAttacking = true;
        _lastAttackTime = Time.time;

        // Change visuals for attack
        _spriteRenderer.color = _attackColor;
        transform.localScale = new Vector3(_idleScale.x, _idleScale.y * _attackScaleY, _idleScale.z);
        _collider.enabled = true;
        yield return new WaitForSeconds(_attackDuration);

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

        food.OnHitByWeapon();
        // TODO: Add visual feedback for the food being hit by the weapon (e.g., play an animation, change color, etc.)
    }
}