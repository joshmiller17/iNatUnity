using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace JoshAaronMiller.INaturalist
{
    [System.Serializable]
    public class Sound : JsonObject<Sound>
    {
        public int id;
        public string attribution;
        public string license_code;
    }
}