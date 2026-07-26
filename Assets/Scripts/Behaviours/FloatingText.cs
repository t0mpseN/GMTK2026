using System.Collections;
using TMPro;
using UnityEngine;

public class FloatingText : MonoBehaviour
{
    // FIELDS & PROPERTIES
    [SerializeField] private TextMeshPro _text;
    [SerializeField] private float _lifetime = 0.8f;
    [SerializeField] private float _riseDistance = 1.2f;
    [SerializeField] private AnimationCurve _alphaCurve = AnimationCurve.Linear(0f, 1f, 1f, 0f);
    [SerializeField] private AnimationCurve _scaleCurve = AnimationCurve.EaseInOut(0f, 0.6f, 0.2f, 1f);

    // METHODS
    public void Show(string message, Color color)
    {
        _text.text = message;
        _text.color = color;
        StartCoroutine(Animate());
    }

    private IEnumerator Animate()
    {
        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + Vector3.up * _riseDistance;

        // pequeno desvio horizontal pra números empilhados não se sobreporem
        endPos.x += Random.Range(-0.3f, 0.3f);

        float elapsed = 0f;
        Color baseColor = _text.color;

        while (elapsed < _lifetime)
        {
            float t = elapsed / _lifetime;

            transform.position = Vector3.Lerp(startPos, endPos, t);
            transform.localScale = Vector3.one * _scaleCurve.Evaluate(t);

            Color c = baseColor;
            c.a = _alphaCurve.Evaluate(t);
            _text.color = c;

            elapsed += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }
}