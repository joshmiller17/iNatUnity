using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JoshAaronMiller.INaturalist
{
    [System.Serializable]
    public class TimeDetails : JsonObject
    {
        public string date;
        public int day;
        public int month;
        public int year;
        public int hour;
        public int week;
    }
}