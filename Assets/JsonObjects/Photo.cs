using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Photo
{
    public int id;
    public string license_code;
    public string attribution;
    public string url;
    public Dimensions original_dimensions;
    // flags not yet implemented
    public string square_url;
    public string medium_url;

    public static Photo CreateFromJson(string jsonString)
    {
        return JsonUtility.FromJson<Photo>(jsonString);
    }
}
