using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace JoshAaronMiller.INaturalist
{
    [System.Serializable]
    public class FlagCounts
    {
        public int resolved;
        public int unresolved;

        public static FlagCounts CreateFromJson(string jsonString)
        {
            return JsonUtility.FromJson<FlagCounts>(jsonString);
        }
    }
}