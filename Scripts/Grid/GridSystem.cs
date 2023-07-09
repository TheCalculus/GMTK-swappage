using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class GridSystem : MonoBehaviour
{
    public Tilemap[] tilemap = new Tilemap[2];
    public CustomTile[] tileset;

    public int gridX = 50;
    public int gridY = 200;

    private CustomTile[,] customTiles;
    private Vector3Int previousHoverPosition;
    private Grid grid;

    [SerializeField]
    private TileBehaviour tileBehaviour;

    private TileBehaviorManager behaviorManager;

    private void Start()
    {
        grid = GetComponent<Grid>();
        Camera.main.transform.position = new Vector3(gridX / 2, 0.175f * gridY, -10);

        customTiles = new CustomTile[gridX, gridY];

        behaviorManager = new TileBehaviorManager();

        for (int x = 0; x < gridX; x++)
        {
            for (int y = 0; y < gridY; y++)
            {
                Identifiers.Identifier identifier = Identifiers.Identifier.GRASS;
                CustomTile tile = GetTileByIdentifier(identifier);

                if (tile != null)
                {
                    Vector3Int position = new Vector3Int(x, y, 0);
                    tilemap[0].SetTile(position, tile);

                    if (y >= gridY / 2)
                    {
                        tilemap[0].SetColor(position, Color.red);
                    }
                    else
                    {
                        tile.isCaptured = true;
                    }

                    customTiles[x, y] = tile;
                }
            }
        }
    }

    private CustomTile GetTileByIdentifier(Identifiers.Identifier identifier)
    {
        foreach (CustomTile tile in tileset)
        {
            if (tile.identifier == identifier)
            {
                tile.isFinal = false;
                tile.isCaptured = false;
                return tile;
            }
        }

        return null;
    }

    private CustomTile GetTile(Vector3Int position)
    {
        int x = position.x;
        int y = position.y;

        int dimX = customTiles.GetLength(0);
        int dimY = customTiles.GetLength(1);

        if (x >= 0 && y >= 0 && x < dimX && y < dimY)
            return customTiles[x, y];

        return null;
    }

    private void Update()
    {
        if (tilemap[1].HasTile(previousHoverPosition) && !GetTile(previousHoverPosition).isFinal)
            tilemap[1].SetTile(previousHoverPosition, null);

        Vector3 mousePosition = Input.mousePosition;
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);

        Vector3Int cellPosition = grid.WorldToCell(worldPosition);
        CustomTile tileAtPosition = GetTile(cellPosition);

        if (
            !EventSystem.current.IsPointerOverGameObject()
            && worldPosition.y <= gridY / 2
            && tilemap[0].HasTile(cellPosition)
            && tilemap[1].GetTile(cellPosition) == null
            && tileAtPosition != null
            && !tileAtPosition.isFinal
        )
        {
            CustomTile tile = GetTileByIdentifier(tileBehaviour.activeTileType);

            if (tile != null)
            {
                tilemap[1].SetTile(cellPosition, tile);
                tile.isFinal = Input.GetMouseButton(0);
                customTiles[cellPosition.x, cellPosition.y] = tile;

                if (tile.isFinal)
                    behaviorManager.ApplyBehavior(tile, cellPosition, this);
            }

            previousHoverPosition = cellPosition;
        }
    }
}

public class TileBehaviorManager
{
    private Dictionary<Identifiers.Identifier, ITileBehavior> behaviorDictionary;

    public TileBehaviorManager()
    {
        behaviorDictionary = new Dictionary<Identifiers.Identifier, ITileBehavior>();

        behaviorDictionary.Add(Identifiers.Identifier.MUSHROOM, new MushroomTileBehavior());
        behaviorDictionary.Add(Identifiers.Identifier.CONDUIT, new ConduitTileBehavior());
        behaviorDictionary.Add(Identifiers.Identifier.SMOKES, new SmokesTileBehavior());
        behaviorDictionary.Add(Identifiers.Identifier.WALL, new WallTileBehavior());
    }

    public void ApplyBehavior(CustomTile tile, Vector3Int position, GridSystem gridSystem)
    {
        if (behaviorDictionary.TryGetValue(tile.identifier, out ITileBehavior behavior))
            behavior.Apply(tile, position, gridSystem);
    }
}

public interface ITileBehavior
{
    void Apply(CustomTile tile, Vector3Int position, GridSystem gridSystem);
}

public class MushroomTileBehavior : ITileBehavior
{
    public void Apply(CustomTile tile, Vector3Int position, GridSystem gridSystem)
    {
        Debug.Log("mushroomin'");
    }
}

public class ConduitTileBehavior : ITileBehavior
{
    public void Apply(CustomTile tile, Vector3Int position, GridSystem gridSystem)
    {
        Debug.Log("harnessing essence");
    }
}

public class SmokesTileBehavior : ITileBehavior
{
    public void Apply(CustomTile tile, Vector3Int position, GridSystem gridSystem)
    {
        Debug.Log("laying smokes");
    }
}

public class WallTileBehavior : ITileBehavior
{
    public void Apply(CustomTile tile, Vector3Int position, GridSystem gridSystem)
    {
        Debug.Log("blockin' attacks (durability)");
    }
}
