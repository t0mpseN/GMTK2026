using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponAim : MonoBehaviour
{
    // FIELDS & PROPERTIES
    private Camera _camera;


    // METHODS
    private void Awake()
    {
        _camera = Camera.main;
    }

    private void Update()
    {
        Mouse mouse = Mouse.current;
        if (mouse == null || _camera == null)
            return;

        Vector2 screenPosition = mouse.position.ReadValue();
        Vector2 worldPosition = _camera.ScreenToWorldPoint(screenPosition);

        Vector2 direction = worldPosition - (Vector2)transform.position;
        if (direction.sqrMagnitude < 0.0001f) // mouse on top of pivot point
            return;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }
}
