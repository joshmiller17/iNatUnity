using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JoshAaronMiller.INaturalist
{
    [System.Serializable]
    public class Annotation : JsonObject<Annotation>
    {
        public string resource_type;
        public int resource_id;
        public int controlled_attribute_id;
        public int controlled_value_id;
    }
}