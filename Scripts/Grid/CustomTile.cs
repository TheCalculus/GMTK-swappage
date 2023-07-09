using UnityEngine;
using UnityEngine.Tilemaps;

public class Identifiers
{
    public enum Identifier
    {
        GRASS,
        DIRT,
        MUSHROOM,
        CONDUIT,
        SMOKES,
        WALL,
    }
}

[CreateAssetMenu(fileName = "New CustomTile", menuName = "Tiles/CustomTile")]
public class CustomTile : TileBase
{
    public Sprite sprite;
    public int layer;
    public bool isFinal; // this is definitely exploitable.
    public Identifiers.Identifier identifier;

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        tileData.sprite = sprite;
        tileData.colliderType = Tile.ColliderType.None;
    }

    public void UpdateBehavior(Vector3Int position, GridSystem gridSystem)
    {
        switch (identifier)
        {
            case Identifiers.Identifier.MUSHROOM:
                UpdateMushroomBehavior(position, gridSystem);
                break;
            case Identifiers.Identifier.CONDUIT:
                UpdateConduitBehavior(position, gridSystem);
                break;
            case Identifiers.Identifier.SMOKES:
                UpdateSmokesBehavior(position, gridSystem);
                break;
            case Identifiers.Identifier.WALL:
                UpdateWallBehavior(position, gridSystem);
                break;
        }
    }

    private void UpdateMushroomBehavior(Vector3Int position, GridSystem gridSystem) { }

    private void UpdateConduitBehavior(Vector3Int position, GridSystem gridSystem) { }

    private void UpdateSmokesBehavior(Vector3Int position, GridSystem gridSystem) { }

    private void UpdateWallBehavior(Vector3Int position, GridSystem gridSystem) { }
}
