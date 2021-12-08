using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;

namespace JoshAaronMiller.INaturalist
{

    [System.Serializable]
    public class Error : JsonObject<Error>
    {
        public string error; //not yet implemented: error may sometimes be a JSON
        public int status = (int)HttpStatusCode.OK;
        public int code;
        public string message;
    }
}