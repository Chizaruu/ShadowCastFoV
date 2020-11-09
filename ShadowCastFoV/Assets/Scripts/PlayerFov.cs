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

    private Tilemap fogMap;

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
        wallMap = gameManager.wallMap;
        fogMap = gameManager.fogMap;
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
            if(_tile.isVisible){
                _tile.isVisible = false;
                fogMap.SetTileFlags(_tile.localPlace, TileFlags.None);
                fogMap.SetColor(_tile.localPlace, grey);
            }
        }
        visibleTiles = new List<Vector3Int>();

        ShadowCastVisibility.FovCompute(lPos, fovDistance, wallMap, fogMap);
    }
}   
