using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class Food : MonoBehaviour
{
    // FIELDS & PROPERTIES
    [SerializeField] protected float _moveSpeed = 3f;
    public float MoveSpeed => _moveSpeed;

    protected Rigidbody2D _rigidBody;
    protected Transform _target;


    // METHODS
    protected virtual void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
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
        _rigidBody.MovePosition(_rigidBody.position + direction * _moveSpeed * Time.fixedDeltaTime);
    }

    protected Vector2 DirectionToTarget()
    {
        return ((Vector2)_target.position - _rigidBody.position).normalized;
    }

    protected virtual Vector2 GetMovementDirection()
    {
        return DirectionToTarget();
    }
}
