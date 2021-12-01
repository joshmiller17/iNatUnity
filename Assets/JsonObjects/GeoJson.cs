using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JoshAaronMiller.INaturalist
{

    [System.Serializable]
    public class GeoJson : JsonObject
    {
        public string type;
        public string coordinates;
    }
}