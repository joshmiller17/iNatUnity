using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JoshAaronMiller.INaturalist
{

    [System.Serializable]
    public class IdentificationSpeciesCount : JsonObject<IdentificationSpeciesCount>
    {
        public int count;
        public Taxon taxon;
    }
}