using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    // FIELDS & PROPERTIES
    private Rigidbody2D _rigidBody;
    private PlayerControls _controls;
    private InputAction _moveAction;
    private Vector2 _input;
    [SerializeField] private WeaponHitBox _weaponHitBox;
    public float MoveSpeed => UpgradeSystem.Instance.GetValue(UpgradeId.MoveSpeed);
    public bool IsMoving => _input.sqrMagnitude > 0.01f;

    // METHODS
    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _controls = new PlayerControls();
        _moveAction = _controls.Player.Move;
    }

    private void OnEnable()
    {
        _controls.Enable();
    }

    private void OnDisable()
    {
        _controls.Disable();
    }

    private void OnDestroy()
    {
        _controls.Dispose();
    }

    private void Update()
    {
        _input = _moveAction.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        if (_weaponHitBox != null && _weaponHitBox.IsAttacking)
            return;

        _rigidBody.MovePosition(_rigidBody.position + _input * MoveSpeed * Time.fixedDeltaTime);
    }
}