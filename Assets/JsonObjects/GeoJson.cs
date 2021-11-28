using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JoshAaronMiller.INaturalist
{

    [System.Serializable]
    public class GeoJson
    {
        public string type;
        public string coordinates;

        public static GeoJson CreateFromJson(string jsonString)
        {
            return JsonUtility.FromJson<GeoJson>(jsonString);
        }
    }
}