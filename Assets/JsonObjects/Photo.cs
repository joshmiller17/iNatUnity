using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JoshAaronMiller.INaturalist
{
    [System.Serializable]
    public class Photo : JsonObject
    {
        public int id;
        public string license_code;
        public string attribution;
        public string url;
        public Dimensions original_dimensions;
        // flags not yet implemented
        public string square_url;
        public string medium_url;
    }
}