using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JoshAaronMiller.INaturalist
{
    [System.Serializable]
    public class ControlledTermValue : JsonObject<ControlledTermValue>
    {
        public int id;
        public string ontology_uri;
        public string uri;
        public bool blocking;
        public string uuid;
        public List<int> taxon_ids;
        public string label;
    }
}