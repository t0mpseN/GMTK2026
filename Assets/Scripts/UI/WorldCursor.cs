using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(SpriteRenderer))]
public class WorldCursor : MonoBehaviour
{
    // FIELDS & PROPERTIES
    [SerializeField] private bool _hideSystemCursor = true;

    private Camera _camera;

    // METHODS
    private void Awake()
    {
        _camera = Camera.main;
    }

    private void OnEnable()
    {
        if (_hideSystemCursor)
            Cursor.visible = false;
    }

    private void OnDisable()
    {
        Cursor.visible = true;
    }

    private void LateUpdate()
    {
        Mouse mouse = Mouse.current;
        if (mouse == null || _camera == null)
            return;

        Vector2 worldPosition = _camera.ScreenToWorldPoint(mouse.position.ReadValue());
        transform.position = new Vector3(worldPosition.x, worldPosition.y, transform.position.z);
    }
}