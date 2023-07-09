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

    private void Update()
    {
        if (tilemap[1].HasTile(previousHoverPosition))
            tilemap[1].SetTile(previousHoverPosition, null);

        Vector3 mousePosition = Input.mousePosition;
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);

        Vector3Int cellPosition = grid.WorldToCell(worldPosition);

        if (tilemap[0].HasTile(cellPosition) && tilemap[1].GetTile(cellPosition) == null)
        {
            CustomTile tile = GetTileByIdentifier(Identifiers.Identifier.MUSHROOM);
            if (tile != null)
            {
                tilemap[1].SetTile(cellPosition, tile);
                customTiles[cellPosition.x, cellPosition.y] = tile;
                previousHoverPosition = cellPosition;
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            CustomTile tile = customTiles[previousHoverPosition.x, previousHoverPosition.y];
            if (tile != null)
                tile.isFinal = true;
            previousHoverPosition = new Vector3Int(-1, -1, -1);
        }
    }
}
