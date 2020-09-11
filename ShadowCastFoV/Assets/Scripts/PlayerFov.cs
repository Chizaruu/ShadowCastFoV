using System.Collections;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerFov : MonoBehaviour
{
    public static PlayerFov instance;

    private WorldTile _tile;

    private Tilemap wallMap;
    private TileBase wall;

    public int fovDistance = 7;

    public List<Vector3Int> visibleTiles;

    private GameManager gameManager;

    void Awake()
    {

        if(instance != null)
        {
            Debug.LogWarning("More than 1 instance of PlayerFov");
            return;
        }
        instance = this;
    }
        
    void Start(){
        gameManager = GameManager.instance;
        wallMap = gameManager.wallTilemap;
        wall = gameManager.wall;
        List<Vector3Int> visibleTiles = new List<Vector3Int>();
    }
    //Checks Player FoV
    public void PlayerVisibility(Vector3Int lPos) {
        Color grey = new Color(1.0f, 1.0f, 1.0f, 0.5f);
        WorldTile _tile;
        var tiles = GameTiles.instance.tiles;
        //Removes visibleTiles.
        foreach (Vector3Int visibleTile in visibleTiles) {
            if (!tiles.TryGetValue(visibleTile, out _tile)) return;
            if(_tile.IsVisible){
                _tile.IsVisible = false;
                _tile.TilemapMember.SetTileFlags(_tile.LocalPlace, TileFlags.None);
                _tile.TilemapMember.SetColor(_tile.LocalPlace, grey);
            }
        }
        visibleTiles = new List<Vector3Int>();

        ShadowCastVisibility.FovCompute(lPos, fovDistance, wallMap, wall);
    }
}   
