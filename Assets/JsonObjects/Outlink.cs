using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JoshAaronMiller.INaturalist
{

    [System.Serializable]
    public class Outlink : JsonObject<Outlink>
    {
        public string source;
        public string url;
    }
}
