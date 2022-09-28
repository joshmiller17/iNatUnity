using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JoshAaronMiller.INaturalist
{

    [System.Serializable]
    public class GeoJson : JsonObject<GeoJson>
    {
        public float[] coordinates; // longitude, latitude
        public string type;
    }
}