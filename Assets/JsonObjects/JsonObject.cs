using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JoshAaronMiller.INaturalist
{
    public class JsonObject
    {
        public static T CreateFromJson<T>(string jsonString)
        {
            return JsonUtility.FromJson<T>(jsonString);
        }

        public static string ToJson<T>(T obj)
        {
            return JsonUtility.ToJson(obj);
        }
    }
}