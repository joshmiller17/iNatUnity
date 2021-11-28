using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JoshAaronMiller.INaturalist
{
    [System.Serializable]
    public class User
    {
        public int id;
        public string login;
        public bool spam;
        public bool suspended;
        public string created_at;
        public int site_id;
        // preferences not yet implemented
        public string login_autocomplete;
        public string login_exact;
        public string name;
        public string name_autocomplete;
        public string orcid;
        public string icon;
        public int observations_count;
        public int identifications_count;
        public int journal_posts_count;
        public int activity_count;
        public int species_count;
        public int universal_search_rank;
        public List<string> roles;
        public string icon_url;

        public static User CreateFromJson(string jsonString)
        {
            return JsonUtility.FromJson<User>(jsonString);
        }
    }
}