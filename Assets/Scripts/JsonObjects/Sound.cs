using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace JoshAaronMiller.INaturalist
{
    [System.Serializable]
    public class Sound : JsonObject<Sound>
    {
        public int id;
        public string attribution;
        public string license_code;
        public string secret_token;
        public string file_content_type;
        public List<Flag> flags;
        public int native_sound_id;
        public bool play_local;
        //subtype not yet implemented
        public string file_url;
    }
}