using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace SCF.Map
{
    public class MapManager : MonoBehaviour
    {
        public static MapManager instance;

        public Grid grid;
        public Tilemap floorMap;
        public Tilemap obstacleMap;
        public Tilemap fogMap;

        public Dictionary<Vector3, WorldTile> fogTiles;

        private void Awake() => instance = this;
        private void Start() => GetFogTiles(); 

        public void GetFogTiles()
        {
            fogTiles = new Dictionary<Vector3, WorldTile>();

            foreach (Vector3Int pos in fogMap.cellBounds.allPositionsWithin)
            {
                var lPos = new Vector3Int(pos.x, pos.y, pos.z);

                if (!fogMap.HasTile(lPos)) continue;

                WorldTile fogTile = new WorldTile()
                {
                    localPlace = lPos,
                    gridLocation = fogMap.CellToWorld(lPos),
                    tileBase = fogMap.GetTile(lPos).name,
                    isVisible = false,
                    isExplored = false,
                };
                fogTiles.Add(fogTile.gridLocation, fogTile); 
            }
        }
    }
}