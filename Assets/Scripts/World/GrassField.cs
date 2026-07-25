using UnityEngine;
using UnityEngine.Tilemaps;

public class GrassField : MonoBehaviour
{
    // FIELDS & PROPERTIES
    [SerializeField] private Tilemap _tilemap;
    [SerializeField] private Sprite[] _grassSprites;
    [SerializeField] private Vector2Int _size = new Vector2Int(100, 100);
    [SerializeField] private int _seed = 12345;

    // METHODS
    private void Start()
    {
        if (_tilemap == null || _grassSprites == null || _grassSprites.Length == 0)
        {
            Debug.LogError($"{name}: tilemap ou sprites não atribuídos.", this);
            return;
        }

        Random.InitState(_seed); // mesmo layout em toda run

        TileBase[] tiles = BuildTiles();

        for (int x = -_size.x / 2; x < _size.x / 2; x++)
            for (int y = -_size.y / 2; y < _size.y / 2; y++)
                _tilemap.SetTile(new Vector3Int(x, y, 0), tiles[Random.Range(0, tiles.Length)]);
    }

    private TileBase[] BuildTiles()
    {
        TileBase[] tiles = new TileBase[_grassSprites.Length];

        for (int i = 0; i < _grassSprites.Length; i++)
        {
            Tile tile = ScriptableObject.CreateInstance<Tile>();
            tile.sprite = _grassSprites[i];
            tiles[i] = tile;
        }

        return tiles;
    }
}