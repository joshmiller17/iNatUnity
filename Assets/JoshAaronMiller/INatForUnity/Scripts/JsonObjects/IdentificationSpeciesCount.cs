using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JoshAaronMiller.INaturalist
{

    [System.Serializable]
    public class SpeciesCount : JsonObject<SpeciesCount>
    {
        public int count;
        public Taxon taxon;
    }
}