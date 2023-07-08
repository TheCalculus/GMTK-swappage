using UnityEngine;
using UnityEngine.Tilemaps;

public class Identifiers {
    public enum Identifier {
        GRASS,
        DIRT,
        MUSHROOM,
    }
}

[CreateAssetMenu(fileName = "New CustomTile", menuName = "Tiles/CustomTile")]
public class CustomTile : TileBase
{
    public Sprite sprite;
    public Identifiers.Identifier identifier;

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        tileData.sprite = sprite;
        tileData.colliderType = Tile.ColliderType.None;
    }
}