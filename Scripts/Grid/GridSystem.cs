using UnityEngine;
using UnityEngine.Tilemaps;

public class GridSystem : MonoBehaviour
{
    public Tilemap tilemap;
    public CustomTile[] tileset;
    public int sideLength;

    private void Start()
    {
        for (int x = 0; x < sideLength; x++)
        {
            for (int y = 0; y < sideLength; y++)
            {
                Identifiers.Identifier identifier = Identifiers.Identifier.GRASS;
                // identifier = Identifiers.Identifier.DIRT;

                CustomTile tile = GetTileByIdentifier(identifier);
                if (tile != null)
                    tilemap.SetTile(new Vector3Int(x, y, 0), tile);
            }
        }
    }

    private CustomTile GetTileByIdentifier(Identifiers.Identifier identifier)
    {
        foreach (CustomTile tile in tileset) {
            if (tile.identifier == identifier)
                return tile;
        }

        return null;
    }
}
