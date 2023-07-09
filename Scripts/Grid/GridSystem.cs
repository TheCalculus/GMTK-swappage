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

    // List to store placed tiles for behavior updates
    private List<Vector3Int> placedTiles = new List<Vector3Int>();

    private void Start()
    {
        grid = GetComponent<Grid>();
        Camera.main.transform.position = new Vector3(gridX / 2, 0.175f * gridY, -10);

        customTiles = new CustomTile[gridX, gridY];

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
                    customTiles[x, y] = tile;
                }

                if (y >= gridY / 2)
                {
                    Vector3Int position = new Vector3Int(x, y, 0);
                    tilemap[0].SetColor(position, Color.red);
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
                tile.ApplyBehavior(cellPosition, this);

                tilemap[1].SetTile(cellPosition, tile);
                tile.isFinal = Input.GetMouseButton(0);
                customTiles[cellPosition.x, cellPosition.y] = tile;

                placedTiles.Add(cellPosition);
            }

            previousHoverPosition = cellPosition;
        }

        foreach (Vector3Int tilePosition in placedTiles)
        {
            CustomTile tile = customTiles[tilePosition.x, tilePosition.y];
            tile.UpdateBehavior(tilePosition, this);

            // if (shouldRemoveTile)
            // {
            //     placedTiles.Remove(tilePosition);
            // }
        }
    }
}
