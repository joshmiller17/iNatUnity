using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JoshAaronMiller.INaturalist
{

    [System.Serializable]
    public class ObservationPhoto : JsonObject<ObservationPhoto>
    {
        public int id;
        public int observation_id;
        public int photo_id;
        public int position;
        public TimeDetails created_at;
        public TimeDetails updated_at;
        public string old_uuid;
        public string uuid;
        public TimeDetails created_at_utc;
        public TimeDetails updated_at_utc;
        public PhotoJson photo;
    }
}