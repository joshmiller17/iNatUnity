using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ObsWebResult
{
    public int total_results;
    public int page;
    public int per_page;
    public List<Observation> results;

    public static ObsWebResult CreateFromJson(string jsonString)
    {
        return JsonUtility.FromJson<ObsWebResult>(jsonString);
    }
}
