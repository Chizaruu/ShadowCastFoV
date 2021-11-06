using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace SCF.Map
{
    /// <summary> Manages the map. </summary>
    public class MapManager : MonoBehaviour
    {
        public static MapManager instance; // The instance of the map manager.
        
        public Grid grid; // The grid of the map.
        public Tilemap floorMap; // The floor map of the map.
        public Tilemap obstacleMap; // The obstacle map of the map.
        public Tilemap fogMap; // The fog map of the map.

        public Dictionary<Vector3, WorldTile> fogTiles; // The fog tiles of the map.

        /// <summary> Sets the instance of the map manager. </summary>
        private void Awake() => instance = this;
        /// <summary> Initializes the map manager. </summary>
        private void Start() => GetFogTiles(); 
        /// <summary> Gets the fog tiles of the map. </summary>
        public void GetFogTiles()
        {
            fogTiles = new Dictionary<Vector3, WorldTile>(); // Initializes the fog tiles.

            // Gets the fog tiles.
            foreach (Vector3Int pos in fogMap.cellBounds.allPositionsWithin)
            {
                var lPos = new Vector3Int(pos.x, pos.y, pos.z); // Gets the local position.
    
                if (!fogMap.HasTile(lPos)) continue; // If the tile doesn't exist, continue.

                // Gets the tile.
                WorldTile fogTile = new WorldTile()
                {
                    localPlace = lPos, // Sets the local place.
                    gridLocation = fogMap.CellToWorld(lPos), // Sets the grid location.
                    tileBase = fogMap.GetTile(lPos).name, // Sets the tile base.
                    isVisible = false, // Sets the visibility.
                    isExplored = false, // Sets the exploration.
                };
                fogTiles.Add(fogTile.gridLocation, fogTile); // Adds the tile to the fog tiles.
            }
        }
    }
}