using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;

namespace JoshAaronMiller.INaturalist
{

    [System.Serializable]
    public class Error : JsonObject
    {
        public string error;
        public int status = (int)HttpStatusCode.OK;
    }
}