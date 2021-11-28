using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dimensions
{
    public int height;
    public int width;


    public static Dimensions CreateFromJson(string jsonString)
    {
        return JsonUtility.FromJson<Dimensions>(jsonString);
    }
}
