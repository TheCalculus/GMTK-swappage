using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TileBehaviour : MonoBehaviour
{
    public Identifiers.Identifier activeTileType;

    private void Start()
    {
        activeTileType = Identifiers.Identifier.MUSHROOM;
    }

    public void SelectTile(int tileIdentifier)
    {
        activeTileType = (Identifiers.Identifier)(tileIdentifier + 2);
    }
}
