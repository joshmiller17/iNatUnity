using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JoshAaronMiller.INaturalist
{

    [System.Serializable]
    public class Dimensions : JsonObject<Dimensions>
    {
        public int height;
        public int width;
    }
}