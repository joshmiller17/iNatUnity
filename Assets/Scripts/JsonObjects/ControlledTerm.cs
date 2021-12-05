using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JoshAaronMiller.INaturalist
{
    [System.Serializable]
    public class ControlledTerm : JsonObject<ControlledTerm>
    {
        public int id;
        public string ontology_uri;
        public bool is_value;
        public bool multivalued;
        public string uuid;
        public List<ControlledTermValue> values;
    }
}