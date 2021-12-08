using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JoshAaronMiller.INaturalist
{
    [System.Serializable]
    public class PlacesByCuration : JsonObject<PlacesByCuration>
    {
        public List<Place> standard; //iNaturalist curator approved
        public List<Place> community; //community places, non-curated
    }
}