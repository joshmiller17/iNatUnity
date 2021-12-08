using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace JoshAaronMiller.INaturalist
{

    [System.Serializable]
    public class GeometryGeoJson : JsonObject<GeometryGeoJson>
    {
        public string type;
        public List<List<List<int>>> coordinates; /// multi-polygon of (x,y) bounds
    }
}