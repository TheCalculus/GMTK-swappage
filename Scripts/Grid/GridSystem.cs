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

    public CustomTile[,] customTiles;
    private Vector3Int previousHoverPosition;
    private Grid grid;

    [SerializeField]
    private TileBehaviour tileBehaviour;
    private TileBehaviourManager behaviourManager;

    private void Start()
    {
        grid = GetComponent<Grid>();
        Camera.main.transform.position = new Vector3(gridX / 2, 0.175f * gridY, -10);

        customTiles = new CustomTile[gridX, gridY];
        behaviourManager = new TileBehaviourManager();

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

                    if (y >= gridY / 2)
                    {
                        tile.isCaptured = true;
                        tilemap[0].SetColor(position, Color.red);
                    }
                    else
                    {
                        tile.isCaptured = false;
                    }
                }
            }
        }
    }

    public CustomTile GetTileByIdentifier(Identifiers.Identifier identifier)
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
            && tileAtPosition != null
            && tileAtPosition.isCaptured
            && tilemap[1].GetTile(cellPosition) == null
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
                    behaviourManager.ApplyBehaviour(tile, cellPosition, this);
            }

            previousHoverPosition = cellPosition;
        }

        behaviourManager.UpdateAllBehaviours();
    }
}

public struct Behaviour
{
    public ITileBehaviour behaviour;
    public CustomTile tile;
    public Vector3Int position;
    public GridSystem gridSystem;
}

public class TileBehaviourManager
{
    private Dictionary<Identifiers.Identifier, ITileBehaviour> behaviourDictionary;
    private List<Behaviour> behaviours;

    public TileBehaviourManager()
    {
        behaviourDictionary = new Dictionary<Identifiers.Identifier, ITileBehaviour>();

        behaviourDictionary.Add(Identifiers.Identifier.MUSHROOM, new MushroomTileBehaviour());
        behaviourDictionary.Add(Identifiers.Identifier.CONDUIT, new ConduitTileBehaviour());
        behaviourDictionary.Add(Identifiers.Identifier.SMOKES, new SmokesTileBehaviour());
        behaviourDictionary.Add(Identifiers.Identifier.WALL, new WallTileBehaviour());

        behaviours = new List<Behaviour>();
    }

    public void UpdateAllBehaviours()
    {
        for (int i = behaviours.Count - 1; i >= 0; i--)
        {
            Behaviour behaviour = behaviours[i];
            behaviour.behaviour.Update(behaviour.tile, behaviour.position, behaviour.gridSystem);

            if (behaviour.tile.isFinal)
                behaviours.RemoveAt(i);
        }
    }

    public void ApplyBehaviour(CustomTile tile, Vector3Int position, GridSystem gridSystem)
    {
        if (behaviourDictionary.TryGetValue(tile.identifier, out ITileBehaviour behaviour))
        {
            Behaviour behaviourStruct = new Behaviour();

            behaviourStruct.behaviour = behaviour;
            behaviourStruct.tile = tile;
            behaviourStruct.position = position;
            behaviourStruct.gridSystem = gridSystem;

            behaviours.Add(behaviourStruct);

            Debug.Log("added behaviour to list");
        }
    }
}

public interface ITileBehaviour
{
    void Update(CustomTile tile, Vector3Int position, GridSystem gridSystem);
}

public class MushroomTileBehaviour : ITileBehaviour
{
    int iterations = 0;
    Vector3Int pos;

    public void Update(CustomTile tile, Vector3Int position, GridSystem gridSystem)
    {
        // slowly advance captures vertically with some horizontal offsetting
        if (iterations == 0)
        {
            pos = position;
            Debug.Log("mushroom iteration 1");
        }

        if (pos.y < gridSystem.gridY)
        {
            gridSystem.customTiles[pos.x, pos.y].isCaptured = true;
            gridSystem.tilemap[0].SetColor(pos, Color.white);
            pos.y += 1;
            iterations++;
        }
    }
}

public class ConduitTileBehaviour : ITileBehaviour
{
    public void Update(CustomTile tile, Vector3Int position, GridSystem gridSystem) { }
}

public class SmokesTileBehaviour : ITileBehaviour
{
    public void Update(CustomTile tile, Vector3Int position, GridSystem gridSystem) { }
}

public class WallTileBehaviour : ITileBehaviour
{
    public void Update(CustomTile tile, Vector3Int position, GridSystem gridSystem) { }
}
