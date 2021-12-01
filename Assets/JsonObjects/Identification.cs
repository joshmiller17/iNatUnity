using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JoshAaronMiller.INaturalist
{
    [System.Serializable]
    public class Identification : JsonObject
    {
        public bool hidden;
        // disagreement not yet implemented
        // flags not yet implemented
        public string created_at;
        public int taxon_id;
        // body not yet implemented
        public bool own_observation;
        public string uuid;
        // taxon change not yet implemented
        // moderator actions not yet implemented
        public bool vision;
        public bool current;
        public int id;
        public TimeDetails created_at_details;
        public string category;
        public bool spam;
        public User user;
        public int previous_observation_taxon_id;
        public Taxon taxon;
    }
}