using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JoshAaronMiller.INaturalist
{
    [System.Serializable]
    public class ObservationSubmission : JsonObject<ObservationSubmission>
    {
        public string species_guess;
        public int taxon_id;
        public string description;
    }

    [System.Serializable]
    public class WrappedObservationSubmission : JsonObject<WrappedObservationSubmission>
    {
        public ObservationSubmission observation;
    }
}