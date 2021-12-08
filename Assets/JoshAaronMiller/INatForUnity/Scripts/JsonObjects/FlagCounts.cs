using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace JoshAaronMiller.INaturalist
{
    [System.Serializable]
    public class FlagCounts : JsonObject<FlagCounts>
    {
        public int resolved;
        public int unresolved;
    }
}