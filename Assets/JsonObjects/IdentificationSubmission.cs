using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JoshAaronMiller.INaturalist
{
    [System.Serializable]
    public class IdentificationSubmission : JsonObject
    {
        public int observation_id;
        public int taxon_id;
        public bool current = true;
        public string body = ""; //Optional user remarks on the identification.
    }

    [System.Serializable]
    public class WrappedIdentificationSubmission : JsonObject
    {
        public IdentificationSubmission identification;
    }
}
