using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using SCF.Map;

namespace SCF.Character
{
    /// <summary> Field of View that can be used to determine what is visible to a character. </summary>
    public class FOV : MonoBehaviour
    {
        [SerializeField]private bool isPlayer; // Is this the player's FOV?
        [SerializeField]private bool isSymmetrical; // Is the FOV symmetrical?

        [SerializeField]private int fovDistance = 7; // The distance the FOV can see
        
        [SerializeField]private List<Vector3Int> visibleTiles = new List<Vector3Int>(); // The tiles that are visible to the character

        public List<Vector3Int> VisibleTiles { get => visibleTiles;} // The tiles that are visible to the character

        /// <summary> Initializes the FOV. </summary>
        private void Start() => Invoke("RefreshFieldOfView", 0.1f);

        /// <summary> Refreshes the FOV. </summary>
        public void RefreshFieldOfView()
        {
            //If it's the player's FOV
            if(isPlayer)
            {
                WorldTile tile; // The tile that is being checked

                if(MapManager.instance.fogTiles == null) return; // If the fog tiles are null, return

                // For each tile in the visible tiles list
                foreach (Vector3Int visibleTile in visibleTiles) {
                    if (!MapManager.instance.fogTiles.TryGetValue(visibleTile, out tile)) return; // If the tile is not in the fog tiles, return

                    // If the tile is visible
                    if(tile.isVisible)
                    {
                        tile.isVisible = false; // Set the tile to not visible
                        tile.isExplored = true; // Set the tile to explored
                        MapManager.instance.fogMap.SetTileFlags(tile.localPlace, TileFlags.None); // Set the tile to not be a blocking tile
                        MapManager.instance.fogMap.SetColor(tile.localPlace, new Color(1.0f, 1.0f, 1.0f, 0.5f)); // Set the tile to be a half transparent tile
                    }
                }
            }
            
            visibleTiles = new List<Vector3Int>(); // Reset the visible tiles list
            Vector3Int position = MapManager.instance.grid.LocalToCell(this.transform.position); // Get the position of the character
            ShadowCastVisibility.FovCompute(position, fovDistance, MapManager.instance.obstacleMap, MapManager.instance.fogMap, isPlayer, this, isSymmetrical); // Compute the FOV
        }
    }
}