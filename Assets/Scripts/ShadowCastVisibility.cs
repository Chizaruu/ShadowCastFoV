using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary> Shadow Cast Visibility Algorithm. </summary>
sealed class ShadowCastVisibility : MonoBehaviour
{
    //The original code is public domain, and is available at:
    //http://www.adammil.net/blog/v125_Roguelike_Vision_Algorithms.html#shadowcast

    //The original code is modified by ChizaruuGCO to work with Unity Tilemaps and RLSKTD.

    /// <summary> Field of View is computed using a shadow cast algorithm. </summary>
    /// <param name="lPos"> The local position of the character. </param>
    /// <param name="rangeLimit"> The range limit for the field of view. </param>
    /// <param name="obstacleMap"> The obstacle map. </param>
    /// <param name="fogMap"> The fog map. </param>
    /// <param name="isPlayer"> Is the character the player? </param>
    /// <param name="fov"> The field of view. </param>
    /// <param name="isSymmetrical"> Is the field of view symmetrical? </param>
    /// <returns> The field of view. </returns>
    static public void FovCompute(Vector3Int lPos, int rangeLimit, Tilemap obstacleMap, Tilemap fogMap, bool isPlayer, FOV fov, bool isSymmetrical)
    {
        TileCompute(lPos, fogMap, isPlayer, fov);
        for(uint octant=0; octant<=7; octant++) Compute(octant, lPos, rangeLimit, 1, new Slope(1, 1), new Slope(0, 1), obstacleMap, fogMap, isPlayer, fov, isSymmetrical);
    }

    /// <summary> Slope is used to represent the slope Y/X as a rational number. </summary>
    struct Slope
    {
        public Slope(int y, int x) { Y=y; X=x; }
        public readonly int Y, X;
    }

    /// <summary> Compute the field of view for a given octant. </summary>
    /// <param name="octant"> The octant. </param>
    /// <param name="lPos"> The local position of the character. </param>
    /// <param name="rangeLimit"> The range limit for the field of view. </param>
    /// <param name="top"> The top of the slope. </param>
    /// <param name="bottom"> The bottom of the slope. </param>
    /// <param name="obstacleMap"> The obstacle map. </param>
    /// <param name="fogMap"> The fog map. </param>
    /// <param name="isPlayer"> Is the character the player? </param>
    /// <param name="fov"> The field of view. </param>
    /// <param name="isSymmetrical"> Is the field of view symmetrical? </param>
    /// <returns> The field of view of a given octant. </returns>
    static void Compute(uint octant, Vector3Int lPos, int rangeLimit, int x, Slope top, Slope bottom, Tilemap obstacleMap, Tilemap fogMap, bool isPlayer, FOV fov, bool isSymmetrical)
    {
        for(; (uint)x <= (uint)rangeLimit; x++) // rangeLimit < 0 || x <= rangeLimit
        {
            // compute the Y coordinates where the top vector leaves the column (on the right) and where the bottom vector
            // enters the column (on the left). this equals (x+0.5)*top+0.5 and (x-0.5)*bottom+0.5 respectively, which can
            // be computed like (x+0.5)*top+0.5 = (2(x+0.5)*top+1)/2 = ((2x+1)*top+1)/2 to avoid floating point math
            int topY = top.X == 1 ? x : ((x*2+1) * top.Y + top.X - 1) / (top.X*2); // the rounding is a bit tricky, though
            int bottomY = bottom.Y == 0 ? 0 : ((x*2-1) * bottom.Y + bottom.X) / (bottom.X*2);
            
            int wasOpaque = -1; // 0:false, 1:true, -1:not applicable
            // compute the top and bottom vectors for the next column
            for(int y=topY; y >= bottomY; y--)
            {   
                int tx = lPos.x, ty = lPos.y;
                // compute the coordinates of the tile at the top of the column
                switch(octant)
                {
                    case 0: tx += x; ty -= y; break;
                    case 1: tx += y; ty -= x; break;
                    case 2: tx -= y; ty -= x; break;
                    case 3: tx -= x; ty -= y; break;
                    case 4: tx -= x; ty += y; break;
                    case 5: tx -= y; ty += x; break;
                    case 6: tx += y; ty += x; break;
                    case 7: tx += x; ty += y; break;
                }

                Vector3Int pos = new Vector3Int(tx, ty, 0); // the position of the tile at the top of the column
                TileCompute(pos, fogMap, isPlayer, fov);
                // NOTE: use the next line instead if you want the algorithm to be symmetrical
                if(isSymmetrical)
                {
                    if((y != topY || top.Y*x >= top.X*y) && (y != bottomY || bottom.Y*x <= bottom.X*y)) TileCompute(pos, fogMap, isPlayer, fov);
                }

                bool isOpaque = obstacleMap.GetTile(pos) != null;
                
                if(x != rangeLimit)
                {

                    if(isOpaque) 
                    {            
                        if(wasOpaque == 0) // if we found a transition from clear to opaque, this sector is done in this column, so
                        {                  // adjust the bottom vector upwards and continue processing it in the next column.
                        Slope newBottom = new Slope(y*2+1, x*2-1); // (x*2-1, y*2+1) is a vector to the top-left of the opaque tile
                        if(y == bottomY) { bottom = newBottom; break; } // don't recurse unless we have to
                        else Compute(octant, lPos, rangeLimit, x+1, top, newBottom, obstacleMap, fogMap, isPlayer, fov, isSymmetrical);
                        }
                        wasOpaque = 1;
                    }
                    else // adjust top vector downwards and continue if we found a transition from opaque to clear
                    {    // (x*2+1, y*2+1) is the top-right corner of the clear tile (i.e. the bottom-right of the opaque tile)
                        if(wasOpaque > 0) top = new Slope(y*2+1, x*2+1);
                            wasOpaque = 0;
                    }
                }
            }

            if(wasOpaque != 0) break; // if the column ended in a clear tile, continue processing the current sector
        }
    }

    /// <summary> Set the fog of war for a given tile. </summary>
    static public void TileCompute(Vector3Int pos, Tilemap fogMap, bool isPlayer, FOV fov)
    {
        WorldTile tile;

        if (!MapManager.instance.fogTiles.TryGetValue(pos, out tile)) return;

            if(isPlayer)
            {
                fogMap.SetTileFlags(tile.localPlace, TileFlags.None);
                fogMap.SetColor(tile.localPlace, new Color(1.0f, 1.0f, 1.0f, 0f));
                tile.isVisible = true;
                tile.isExplored = true;
            }

            if(!fov.VisibleTiles.Contains(pos)) fov.VisibleTiles.Add(pos);
    }
}
