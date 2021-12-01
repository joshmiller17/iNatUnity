using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace JoshAaronMiller.INaturalist
{
    [System.Serializable]
    public class Taxon : JsonObject
    {
        public bool is_active;
        public string ancestry;
        public string min_species_ancestry;
        public bool endemic;
        public int iconic_taxon_id;
        public int min_species_taxon_id;
        public bool threatened;
        public int rank_level;
        public bool introduced;
        public bool native;
        public int parent_id;
        public string name;
        public string rank;
        public bool extinct;
        public int id;
        public List<int> ancestor_ids;
        public bool photos_locked;
        public int taxon_schemes_count;
        public string wikipedia_url;
        // current synonymous taxon ids not yet implemented
        public string created_at;
        public int taxon_changes_count;
        public int complete_species_count;
        public int universal_search_rank;
        public int observations_count;
        public FlagCounts flag_counts;
        public int atlas_id;
        public Photo default_photo;
        public string iconic_taxon_name;
        public string preferred_common_name;
        public string wikipedia_summary;
    }
}