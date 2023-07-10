using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum SpriteType
{
    GRASS,
    DIRT,
    MUSHROOM,
    CONDUIT,
    SMOKES,
    WALL,
}

public class SpriteTile
{
    public Vector3Int position;
    public CustomTileBase tile;
    public SpriteType type;

    internal delegate void BehaviourDelegate();
    internal BehaviourDelegate behaviour = null;

    private int iterations = 0;

    public SpriteTile(Vector3Int position, CustomTileBase tile, SpriteType type, Tilemap tilemap)
    {
        this.position = position;
        this.tile = tile;
        this.type = type;

        switch (this.type)
        {
            case SpriteType.MUSHROOM:
                behaviour = () =>
                {
                    while (this.iterations++ <= 10)
                        ;
                    Debug.Log(this.iterations);
                    behaviour = null;
                };
                break;
            case SpriteType.CONDUIT:
                break;
            case SpriteType.SMOKES:
                break;
            case SpriteType.WALL:
                break;
        }
    }
}

public class GridRework : MonoBehaviour
{
    public Tilemap[] tilemaps = new Tilemap[3]; /* layer 0 and 1 for level, 2 for units */

    private CustomTileBase[] sprites;
    private Vector3Int[] positions;

    private List<SpriteTile> state = new List<SpriteTile>();

    public int gridX = 40;
    public int gridY = 20;

    public Sprite[] spriteSheet;

    public Sprite getSprite(SpriteType type)
    {
        return spriteSheet[(int)type];
    }

    private void Start()
    {
        positions = new Vector3Int[gridX * gridY];
        sprites = new CustomTileBase[gridX * gridY];

        for (int x = 0; x < gridX; x++)
        {
            for (int y = 0; y < gridY; y++)
            {
                int index = (x * gridY) + y;
                positions[index] = new Vector3Int(x, y, 0);
                CustomTileBase tile = ScriptableObject.CreateInstance<CustomTileBase>();
                tile.SetCustomTileBase(getSprite(SpriteType.GRASS));
                sprites[index] = tile;
            }
        }

        tilemaps[0].SetTiles(positions, sprites);
    }

    private void Update()
    {
        Vector3 mouse = Input.mousePosition;
        mouse = Camera.main.ScreenToWorldPoint(mouse);

        Vector3Int position = Vector3Int.FloorToInt(mouse);

        if (position.x < 0 || position.y < 0 || position.x >= gridX || position.y >= gridY)
        {
            return;
        }

        if (Input.GetMouseButton(0))
        {
            int index = (gridX * position.x) + position.y;

            CustomTileBase tile = ScriptableObject.CreateInstance<CustomTileBase>();
            tile.SetCustomTileBase(getSprite(SpriteType.MUSHROOM));

            state.Add(new SpriteTile(position, tile, SpriteType.MUSHROOM, tilemaps[1]));
            tilemaps[1].SetTile(position, tile);
        }

        foreach (SpriteTile spriteTile in state)
        {
            if (spriteTile.behaviour != null)
                spriteTile.behaviour();
        }
    }
}

public class CustomTileBase : TileBase
{
    public Sprite tileSprite;

    public void SetCustomTileBase(Sprite sprite)
    {
        tileSprite = sprite;
    }

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        tileData.sprite = tileSprite;
    }
}
