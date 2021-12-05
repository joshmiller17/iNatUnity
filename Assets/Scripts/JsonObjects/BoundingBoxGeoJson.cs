using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JoshAaronMiller.INaturalist
{

    [System.Serializable]
    public class BoundingBoxGeoJson : JsonObject<BoundingBoxGeoJson>
    {
        public string type;
        public List<List<int>> coordinates; //4 (x,y) pairs
    }
}