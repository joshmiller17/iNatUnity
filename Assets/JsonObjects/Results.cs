using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JoshAaronMiller.INaturalist
{
    [System.Serializable]
    public class Results<T> : JsonObject<Results<T>>
    {
        public int total_results;
        public int page;
        public int per_page;
        public List<T> results;
    }
}
