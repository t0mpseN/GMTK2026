using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(WeaponHitBox))]
public class WeaponAim : MonoBehaviour
{
    // FIELDS & PROPERTIES
    private Camera _camera;
    private WeaponHitBox _hitBox;

    // METHODS
    private void Awake()
    {
        _camera = Camera.main;
        _hitBox = GetComponent<WeaponHitBox>();
    }

    private void Update()
    {
        if (_hitBox.IsAttacking)
            return;

        Mouse mouse = Mouse.current;
        if (mouse == null || _camera == null)
            return;

        Vector2 screenPosition = mouse.position.ReadValue();
        Vector2 worldPosition = _camera.ScreenToWorldPoint(screenPosition);

        Vector2 direction = worldPosition - (Vector2)transform.position;
        if (direction.sqrMagnitude < 0.0001f)
            return;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }
}