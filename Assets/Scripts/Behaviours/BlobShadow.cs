using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class BlobShadow : MonoBehaviour
{
    // FIELDS & PROPERTIES
    [SerializeField] private Material _materialTemplate;
    [SerializeField] private Color _color = new Color(0f, 0f, 0f, 0.3f);
    [SerializeField] private float _width = 0.8f;
    [SerializeField] private float _height = 0.35f;
    [SerializeField] private Vector2 _offset = new Vector2(0f, -0.4f);
    [SerializeField] private int _segments = 24;
    [SerializeField] private int _sortingOrder = -1;

    // METHODS
    private void Awake()
    {
        GetComponent<MeshFilter>().mesh = BuildEllipse(_segments);

        MeshRenderer renderer = GetComponent<MeshRenderer>();
        if (_materialTemplate != null)
            renderer.material = new Material(_materialTemplate);
        renderer.material.color = _color;
        renderer.sortingOrder = _sortingOrder;

        transform.localPosition = new Vector3(_offset.x, _offset.y, 0f);
        transform.localScale = new Vector3(_width, _height, 1f);
    }

    private Mesh BuildEllipse(int segments)
    {
        Mesh mesh = new Mesh { name = "Shadow" };

        Vector3[] vertices = new Vector3[segments + 1];
        vertices[0] = Vector3.zero;

        for (int i = 0; i < segments; i++)
        {
            float angle = (i / (float)segments) * Mathf.PI * 2f;
            vertices[i + 1] = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f) * 0.5f;
        }

        int[] triangles = new int[segments * 3];
        for (int i = 0; i < segments; i++)
        {
            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = (i + 1) % segments + 1;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateBounds();
        return mesh;
    }
}