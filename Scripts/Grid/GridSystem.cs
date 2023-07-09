using UnityEngine;
using UnityEngine.Tilemaps;

public class GridSystem : MonoBehaviour
{
    public Tilemap[] tilemap = new Tilemap[3];
    public CustomTile[] tileset;
    public int sideLength;

    private CustomTile[,] customTiles;
    private Vector3Int previousHoverPosition;
    private Grid grid;

    private void Start()
    {
        grid = GetComponent<Grid>();
        customTiles = new CustomTile[sideLength, sideLength];

        for (int x = 0; x < sideLength; x++)
        {
            for (int y = 0; y < sideLength; y++)
            {
                Identifiers.Identifier identifier = Identifiers.Identifier.GRASS;

                CustomTile tile = GetTileByIdentifier(identifier);
                if (tile != null)
                {
                    Vector3Int position = new Vector3Int(x, y, 0);
                    tilemap[tile.layer].SetTile(position, tile);
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
            tilemap[0].HasTile(cellPosition)
            && tilemap[1].GetTile(cellPosition) == null
            && tileAtPosition != null
            && !tileAtPosition.isFinal
        )
        {
            CustomTile tile = GetTileByIdentifier(Identifiers.Identifier.MUSHROOM);
            if (tile != null)
            {
                tilemap[1].SetTile(cellPosition, tile);
                customTiles[cellPosition.x, cellPosition.y] = tile;
                tile.isFinal = false;
            }
            
            if (Input.GetMouseButton(0) && !tileAtPosition.isFinal)
                tileAtPosition.isFinal = true;
            
            previousHoverPosition = cellPosition;
        }
    }
}
