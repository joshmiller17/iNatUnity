using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JoshAaronMiller.INaturalist
{

    [System.Serializable]
    public class Flag : JsonObject<Flag>
    {
        public string flaggable_type; //ex: "Comment"
        public int flaggable_id; //presumably the ID of the thing to flag
        public string flag; // must be spam, inappropriate, or other
    }

    [System.Serializable]
    public class WrappedFlag : JsonObject<WrappedFlag>
    {
        public Flag flag;
        public string flag_explanation; // if flag is type "other", include a comment
}