using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(SpriteRenderer))]
public class PlayerSpriteFlip : MonoBehaviour
{
    // FIELDS & PROPERTIES
    [Tooltip("Uncheck if the source sprite is drawn facing left.")]
    [SerializeField] private bool _spriteFacesRight = true;

    [Tooltip("Dead zone around the player to avoid flicker when the cursor is centered.")]
    [SerializeField] private float _flipDeadZone = 0.05f;

    private SpriteRenderer _spriteRenderer;
    private Camera _camera;

    // METHODS
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _camera = Camera.main;
    }

    private void LateUpdate()
    {
        Mouse mouse = Mouse.current;
        if (mouse == null || _camera == null)
            return;

        Vector2 worldPosition = _camera.ScreenToWorldPoint(mouse.position.ReadValue());
        float horizontalDelta = worldPosition.x - transform.position.x;

        if (Mathf.Abs(horizontalDelta) < _flipDeadZone)
            return;

        bool cursorIsToTheRight = horizontalDelta > 0f;
        _spriteRenderer.flipX = _spriteFacesRight ? !cursorIsToTheRight : cursorIsToTheRight;
    }
}