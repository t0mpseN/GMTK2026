using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(PlayerMovement))]
public class PlayerAnimator : MonoBehaviour
{
    // FIELDS & PROPERTIES
    private static readonly int IsMovingHash = Animator.StringToHash("IsMoving");

    private Animator _animator;
    private PlayerMovement _movement;

    // METHODS
    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _movement = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        _animator.SetBool(IsMovingHash, _movement.IsMoving);
    }
}