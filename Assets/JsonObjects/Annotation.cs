using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Annotation
{
    public string resource_type;
    public int resource_id;
    public int controlled_attribute_id;
    public int controlled_value_id;


    public static Annotation CreateFromJson(string jsonString)
    {
        return JsonUtility.FromJson<Annotation>(jsonString);
    }
}
