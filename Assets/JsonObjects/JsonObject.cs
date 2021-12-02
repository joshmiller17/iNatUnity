using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JoshAaronMiller.INaturalist
{
    public abstract class JsonObject<T>
    {
        public static T CreateFromJson(string jsonString)
        {
            return JsonUtility.FromJson<T>(jsonString);
        }

        public static string ToJson(T obj)
        {
            return JsonUtility.ToJson(obj);
        }
    }
}