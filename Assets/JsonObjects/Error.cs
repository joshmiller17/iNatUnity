using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;

namespace JoshAaronMiller.INaturalist
{

    [System.Serializable]
    public class Error : JsonObject<Error>
    {
        public string error; //TODO error may be a JSON
        public int status = (int)HttpStatusCode.OK;
    }
}