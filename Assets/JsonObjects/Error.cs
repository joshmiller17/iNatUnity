using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;

namespace JoshAaronMiller.INaturalist
{

    [System.Serializable]
    public class Error
    {
        public string error;
        public int status = (int)HttpStatusCode.OK;


        public static Error CreateFromJson(string jsonString)
        {
            return JsonUtility.FromJson<Error>(jsonString);
        }
    }
}