using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JoshAaronMiller.INaturalist
{

    [System.Serializable]
    public class Outlink
    {
        public string source;
        public string url;

        public static Outlink CreateFromJson(string jsonString)
        {
            return JsonUtility.FromJson<Outlink>(jsonString);
        }
    }
}
