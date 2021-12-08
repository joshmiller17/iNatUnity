using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JoshAaronMiller.INaturalist
{
    [System.Serializable]
    public class Vote : JsonObject<Vote>
    {
        public string vote; //"up" or "down"

        /* An empty scope is recorded as a "fave";
         * A scope "needs_id" is a vote on the Quality Grade 
         * criterion "can the Community ID still be confirmed or improved?"
         */
        public string scope; // "" (fave) or "needs_id"
    }
}