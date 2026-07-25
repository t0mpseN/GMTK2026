using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class HitboxVisual : MonoBehaviour
{
    // FIELDS & PROPERTIES
    [SerializeField] private Material _materialTemplate;
    [SerializeField] private Color _color = new Color(1f, 0.25f, 0.25f, 0.35f);
    [SerializeField] private int _segments = 32;
    [SerializeField] private int _sortingOrder = 20;

    private MeshRenderer _meshRenderer;

    // METHODS
    private void Awake()
    {
        GetComponent<MeshFilter>().mesh = BuildHalfDiscMesh(_segments);

        _meshRenderer = GetComponent<MeshRenderer>();

        if (_materialTemplate != null)
            _meshRenderer.material = new Material(_materialTemplate);

        _meshRenderer.material.color = _color;
        _meshRenderer.sortingOrder = _sortingOrder;
        _meshRenderer.enabled = false;
    }

    /// <summary>Mostra a elipse com os raios informados, em unidades de mundo.</summary>
    public void Show(float forwardRadius, float lateralRadius)
    {
        transform.localScale = new Vector3(forwardRadius, lateralRadius, 1f);
        _meshRenderer.enabled = true;
    }

    public void Hide()
    {
        _meshRenderer.enabled = false;
    }

    /// <summary>Meio-disco unitário: X de 0 a 1 (frente), Y de -1 a 1 (lateral).</summary>
    private Mesh BuildHalfDiscMesh(int segments)
    {
        Mesh mesh = new Mesh { name = "HalfDisc" };

        Vector3[] vertices = new Vector3[segments + 2];
        vertices[0] = Vector3.zero;

        for (int i = 0; i <= segments; i++)
        {
            float angle = Mathf.Lerp(-Mathf.PI / 2f, Mathf.PI / 2f, i / (float)segments);
            vertices[i + 1] = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f);
        }

        int[] triangles = new int[segments * 3];
        for (int i = 0; i < segments; i++)
        {
            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = i + 2;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateBounds();

        return mesh;
    }
}