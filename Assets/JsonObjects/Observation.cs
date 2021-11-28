using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JoshAaronMiller.INaturalist
{

    [System.Serializable]
    public class Observation
    {
        public string quality_grade;
        public string time_observed_at;
        public string taxon_geoprivacy;
        public List<Annotation> annotations;
        public string uuid;
        public TimeDetails observed_on_details;
        public int id;
        public int cached_votes_total;
        public bool identifications_most_agree;
        public TimeDetails created_at_details;
        public string species_guess;
        public bool identifications_most_disagree;
        public List<string> tags;
        public bool positional_accuracy;
        public int comments_count;
        public int site_id;
        public string created_time_zone;
        public string license_code;
        public string observed_time_zone;
        // quality_metrics not yet implemented
        // public_positional_accuracy not yet implemented
        public List<int> reviewed_by;
        public int oauth_application_id;
        //flags not yet implemented
        public string created_at;
        public string description;
        public string time_zone_offset;
        public List<int> project_ids_with_curator_id;
        public string observed_on;
        public string observed_on_string;
        public string updated_at;
        public List<Sound> sounds;
        public List<int> place_ids;
        public bool captive;
        public Taxon taxon;
        public List<int> ident_taxon_ids;
        public List<Outlink> outlinks;
        public int faves_count;
        //ofvs not yet implemented
        public int num_identification_agreements;
        // preferences not yet implemented
        //comments not yet implemented
        public int map_scale;
        public string uri;
        public List<int> project_ids;
        public int community_taxon_id;
        public GeoJson geojson;
        public bool owners_identification_from_vision;
        public int identifications_count;
        public bool obscured;
        public int num_identification_disagreements;
        public string geoprivacy;
        public string locatioan;
        //votes not yet implemented
        public User user;
        public bool mappable;
        public bool identifications_some_agree;
        public List<int> project_ids_without_curator_id;
        public string place_guess;
        public List<Identification> identifications;



        public static Observation CreateFromJson(string jsonString)
        {
            return JsonUtility.FromJson<Observation>(jsonString);
        }


    }
}