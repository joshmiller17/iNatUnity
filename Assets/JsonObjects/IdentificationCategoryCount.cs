using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JoshAaronMiller.INaturalist
{

    [System.Serializable]
    public class IdentificationCategoryCount : JsonObject<IdentificationCategoryCount>
    {
        public string category;
        public int count;
    }
}