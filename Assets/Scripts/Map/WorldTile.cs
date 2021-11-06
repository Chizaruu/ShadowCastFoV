using UnityEngine;

namespace SCF.Map
{
    /// <summary> A tile in the world. </summary>
    [System.Serializable]
    public class WorldTile{
        public string tileBase;
        public bool isExplored;
        public bool isVisible;
        public Vector3Int localPlace;
        public Vector3 gridLocation;
    }
}

