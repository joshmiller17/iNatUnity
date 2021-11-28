using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace JoshAaronMiller.INaturalist
{
    [System.Serializable]
    public class Sound
    {
        public int id;
        public string attribution;
        public string license_code;

        public static Sound CreateFromJson(string jsonString)
        {
            return JsonUtility.FromJson<Sound>(jsonString);
        }
    }
}