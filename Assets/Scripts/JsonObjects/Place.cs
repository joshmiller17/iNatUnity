using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JoshAaronMiller.INaturalist
{
    [System.Serializable]
    public class Place : JsonObject<Place>
    {
        public List<int> ancestor_place_ids;
        public BoundingBoxGeoJson bounding_box_geojson;
        public float bbox_area;
        public int admin_level; /// Supported admin levels are: -1 (continent), 0 (country), 1 (state), 2 (county), 3 (town), 10 (park)
        public int place_type;
        public string name;
        public string location; /// geographic lng/lat
        public int id;
        public string display_name;
        public string uuid;
        public string slug;
        public GeometryGeoJson geometry_geojson;
    }
}