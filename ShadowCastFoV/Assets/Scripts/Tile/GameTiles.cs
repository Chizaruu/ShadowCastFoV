using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameTiles : MonoBehaviour {
	public static GameTiles instance;
	public Tilemap fogMap;

	public Dictionary<Vector3, WorldTile> tiles;

	private void Awake() 
	{
		if (instance == null) 
		{
			instance = this;
		}
		else if (instance != this)
		{
			Destroy(gameObject);
		}
        
		Invoke("GetWorldTiles", 0.1f);
	}

	// Use this for initialization
	private void GetWorldTiles () 
	{
        tiles = new Dictionary<Vector3, WorldTile>();

        foreach (Vector3Int pos in fogMap.cellBounds.allPositionsWithin)
		{
			var lPos = new Vector3Int(pos.x, pos.y, pos.z);

                if (!fogMap.HasTile(lPos)) continue;
                var tile = new WorldTile
                {
                    LocalPlace = lPos,
                    GridLocation = fogMap.CellToWorld(lPos),
                    TileBase = fogMap.GetTile(lPos),
                    TilemapMember = fogMap,
                    Name = lPos.x + "," + lPos.y,
					IsVisible = false,
					IsExplored = false,
			    };
                tiles.Add(tile.GridLocation, tile);
		}        
	}
}
