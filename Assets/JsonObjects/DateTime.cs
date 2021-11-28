using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TimeDetails
{
    public string date;
    public int day;
    public int month;
    public int year;
    public int hour;
    public int week;

    public static TimeDetails CreateFromJson(string jsonString)
    {
        return JsonUtility.FromJson<TimeDetails>(jsonString);
    }
}
