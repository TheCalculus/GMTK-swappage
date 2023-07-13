using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
    private int iterations = 0;
    public bool isCaptured { get; private set; }

    internal delegate void BehaviourDelegate();
    internal BehaviourDelegate behaviour = null;

    public SpriteTile(Vector3Int position, CustomTileBase tile, SpriteType type)
    {
        this.position = position;
        this.tile = tile;
        this.type = type;
        this.isCaptured = false;

        switch (this.type)
        {
            case SpriteType.MUSHROOM:
                behaviour = () =>
                {
                    this.position.y += 1;
                    this.iterations++;
                    GridRework.PlaceTile(this.position, this.tile, this.type, true);
                    if (this.iterations == 10)
                        this.behaviour = null;
                };
                break;
            case SpriteType.CONDUIT:
            case SpriteType.SMOKES:
            case SpriteType.WALL:
                break;
        }
    }
}

public class GridRework : MonoBehaviour
{
    [Header("Grid, Sprites")]
    public Tilemap[] tilemaps = new Tilemap[3]; /* layer 0 and 1 for level, 2 for units */
    public Sprite[] spriteSheet;
    public SpriteType activeType;

    [Header("Grid Dimensions")]
    public int gridX = 40;
    public int gridY = 20;

    [Header("UI")]
    public Button mushroomButton;
    public Button conduitButton;
    public Button smokesButton;
    public Button wallButton;

    private List<SpriteTile> state = new List<SpriteTile>();

    private static GridRework instance;

    public void SetActiveTile(SpriteType type)
    {
        activeType = type;
    }

    public Sprite GetSprite(SpriteType type)
    {
        return spriteSheet[(int)type];
    }

    private void Start()
    {
        instance = this;

        mushroomButton.onClick.AddListener(() => activeType = SpriteType.MUSHROOM);
        conduitButton.onClick.AddListener(() => activeType = SpriteType.CONDUIT);
        smokesButton.onClick.AddListener(() => activeType = SpriteType.SMOKES);
        wallButton.onClick.AddListener(() => activeType = SpriteType.WALL);

        CustomTileBase tile = CreateCustomTile(GetSprite(SpriteType.GRASS)); // all tiles in start are grass
        Vector3Int position = new Vector3Int(0, 0, 0); // position can be reused
        for (int x = 0; x < gridX; x++)
        {
            for (int y = 0; y < gridY; y++)
            {
                position.x = x;
                position.y = y;

                tilemaps[0].SetTile(position, tile);

                if (y >= gridY / 2)
                    tilemaps[0].SetColor(position, Color.red);

                state.Add(new SpriteTile(position, tile, SpriteType.GRASS));
            }
        }
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

            CustomTileBase tile = CreateCustomTile(GetSprite(activeType));
            PlaceTile(position, tile, activeType, false);
        }

        for (int i = state.Count - 1; i >= 0; i--)
        {
            SpriteTile spriteTile = state[i];
            if (spriteTile.behaviour != null)
                spriteTile.behaviour();
            else
                state.RemoveAt(i);
        }
    }

    public static void PlaceTile(
        Vector3Int position,
        CustomTileBase tile,
        SpriteType type,
        bool isGhost
    )
    {
        if (
            instance != null
            && !instance.state.Exists(st => st.position == position && st.isCaptured)
        )
        {
            instance.tilemaps[1].SetTile(position, tile);
            if (!isGhost)
                instance.state[(position.x * instance.gridX + position.y)] = (
                    new SpriteTile(position, tile, type)
                );
        }
    }

    private CustomTileBase CreateCustomTile(Sprite sprite)
    {
        CustomTileBase tile = ScriptableObject.CreateInstance<CustomTileBase>();
        tile.SetCustomTileBase(sprite);
        return tile;
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
