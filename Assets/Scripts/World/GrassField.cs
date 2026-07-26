using UnityEngine;
using UnityEngine.Tilemaps;

public class GrassField : MonoBehaviour
{
    // FIELDS & PROPERTIES
    [SerializeField] private Tilemap _tilemap;
    [SerializeField] private Sprite[] _grassSprites;
    [SerializeField] private Vector2Int _size = new Vector2Int(100, 100);
    [SerializeField] private int _seed = 12345;
    [SerializeField] private Sprite _emptySprite;

    [Range(0f, 1f)]
    [Tooltip("Chance de um tile ter grama. 0.15 = 15% com folha, 85% verde puro.")]
    [SerializeField] private float _decoratedChance = 0.15f;

    private void Start()
    {
        if (_tilemap == null || _emptySprite == null || _grassSprites == null || _grassSprites.Length == 0)
        {
            Debug.LogError($"{name}: sprites não atribuídos.", this);
            return;
        }

        Random.InitState(_seed);

        TileBase emptyTile = BuildTile(_emptySprite);
        TileBase[] grassTiles = BuildTiles();

        for (int x = -_size.x / 2; x < _size.x / 2; x++)
            for (int y = -_size.y / 2; y < _size.y / 2; y++)
            {
                TileBase chosen = Random.value < _decoratedChance
                    ? grassTiles[Random.Range(0, grassTiles.Length)]
                    : emptyTile;

                _tilemap.SetTile(new Vector3Int(x, y, 0), chosen);
            }
    }

    private TileBase BuildTile(Sprite sprite)
    {
        Tile tile = ScriptableObject.CreateInstance<Tile>();
        tile.sprite = sprite;
        return tile;
    }

    private TileBase[] BuildTiles()
    {
        TileBase[] tiles = new TileBase[_grassSprites.Length];
        for (int i = 0; i < _grassSprites.Length; i++)
            tiles[i] = BuildTile(_grassSprites[i]);
        return tiles;
    }
}