using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

sealed class ShadowCastVisibility : MonoBehaviour
{
    //Based on code provided by Adam Milazzo from http://www.adammil.net/blog/v125_Roguelike_Vision_Algorithms.html#shadowcast
    //Modified for Unity Tilemaps by ChizaruuGCO.

    static public void FovCompute(Vector3Int lPos, int rangeLimit, Tilemap wallMap, TileBase wall)
    {
        TileCompute(lPos);
        for(uint octant=0; octant<=7; octant++) Compute(octant, lPos, rangeLimit, 1, new Slope(1, 1), new Slope(0, 1), wallMap, wall);
    }

    struct Slope // represents the slope Y/X as a rational number
    {
        public Slope(int y, int x) { Y=y; X=x; }
        public readonly int Y, X;
    }

    static void Compute(uint octant, Vector3Int lPos, int rangeLimit, int x, Slope top, Slope bottom, Tilemap wallMap, TileBase wall)
    {
        for(; (uint)x <= (uint)rangeLimit; x++) // rangeLimit < 0 || x <= rangeLimit
        {
            // compute the Y coordinates where the top vector leaves the column (on the right) and where the bottom vector
            // enters the column (on the left). this equals (x+0.5)*top+0.5 and (x-0.5)*bottom+0.5 respectively, which can
            // be computed like (x+0.5)*top+0.5 = (2(x+0.5)*top+1)/2 = ((2x+1)*top+1)/2 to avoid floating point math
            int topY = top.X == 1 ? x : ((x*2+1) * top.Y + top.X - 1) / (top.X*2); // the rounding is a bit tricky, though
            int bottomY = bottom.Y == 0 ? 0 : ((x*2-1) * bottom.Y + bottom.X) / (bottom.X*2);
            
            int wasOpaque = -1; // 0:false, 1:true, -1:not applicable
            for(int y=topY; y >= bottomY; y--)
            {
                int tx = lPos.x, ty = lPos.y;
                switch(octant) // translate local coordinates to map coordinates
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

                Vector3Int pos = new Vector3Int(tx, ty, 0);
                TileCompute(pos);
                // NOTE: use the next line instead if you want the algorithm to be symmetrical
                //if((y != topY || top.Y*x >= top.X*y) && (y != bottomY || bottom.Y*x <= bottom.X*y)) TileCompute(pos);

                bool isOpaque = BlocksLight(pos, wallMap, wall);
                if(x != rangeLimit)
                {
                    if(isOpaque) 
                    {            
                        if(wasOpaque == 0) // if we found a transition from clear to opaque, this sector is done in this column, so
                        {                  // adjust the bottom vector upwards and continue processing it in the next column.
                        Slope newBottom = new Slope(y*2+1, x*2-1); // (x*2-1, y*2+1) is a vector to the top-left of the opaque tile
                        if(y == bottomY) { bottom = newBottom; break; } // don't recurse unless we have to
                        else Compute(octant, lPos, rangeLimit, x+1, top, newBottom, wallMap, wall);
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

    static public bool BlocksLight(Vector3Int pos, Tilemap wallMap, TileBase wall){
        
        if(wallMap.GetTile(pos) == wall){
            return true;
        }
        else{
            return false;
        }
    }

    static public void TileCompute(Vector3Int pos){
        WorldTile _tile;
        var tiles = GameTiles.instance.tiles;
        List<Vector3Int> visibleTiles = PlayerFov.instance.visibleTiles;
        Color clear = new Color(1.0f, 1.0f, 1.0f, 0f);
        Color grey = new Color(1.0f, 1.0f, 1.0f, 0.5f);
        Color black = new Color(1.0f, 1.0f, 1.0f, 1f);
        if (!tiles.TryGetValue(pos, out _tile)) return;
            _tile.TilemapMember.SetTileFlags(_tile.LocalPlace, TileFlags.None);
            _tile.TilemapMember.SetColor(_tile.LocalPlace, clear);
            _tile.IsVisible = true;
            _tile.IsExplored = true;
            if(!visibleTiles.Contains(pos)) visibleTiles.Add(pos);
    }
}
