using UnityEngine;

namespace SCF.Map
{
    [System.Serializable]
    public class WorldTile{
        public string tileBase;
        public bool isExplored;
        public bool isVisible;
        public Vector3Int localPlace;
        public Vector3 gridLocation;
    }
}

