using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WorldTile {
    public Vector3Int localPlace;

    public Vector3 gridLocation;

    public TileBase tileBase;

    public bool isExplored;

    public bool isVisible;
}
