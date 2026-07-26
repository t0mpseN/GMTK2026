using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SlashVisual : MonoBehaviour
{
    // FIELDS & PROPERTIES
    [SerializeField] private Sprite[] _frames;
    [SerializeField] private int _sortingOrder = 20;

    private SpriteRenderer _renderer;
    private Vector2 _frameSize;
    private Coroutine _routine;

    // METHODS
    private void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _renderer.sortingOrder = _sortingOrder;
        _renderer.enabled = false;

        if (_frames.Length > 0)
            _frameSize = _frames[0].bounds.size;
    }

    public void Play(float forwardRadius, float lateralRadius, float duration)
    {
        if (_frames.Length == 0)
            return;

        if (_routine != null)
            StopCoroutine(_routine);

        _routine = StartCoroutine(SlashRoutine(forwardRadius, lateralRadius, duration));
    }

    private IEnumerator SlashRoutine(float forwardRadius, float lateralRadius, float duration)
    {
        transform.localScale = new Vector3(
            forwardRadius / _frameSize.x,
            (lateralRadius * 2f) / _frameSize.y,
            1f);

        _renderer.enabled = true;

        float frameDuration = duration / _frames.Length;

        foreach (Sprite frame in _frames)
        {
            _renderer.sprite = frame;
            yield return new WaitForSeconds(frameDuration);
        }

        _renderer.enabled = false;
        _routine = null;
    }
}