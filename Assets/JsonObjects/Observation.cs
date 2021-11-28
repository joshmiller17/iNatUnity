using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using UnityEngine;

namespace JoshAaronMiller.INaturalist
{

    [System.Serializable]
    public class Observation
    {
        static readonly Regex PhotoNameRegex = new Regex("photos/(.*)/");

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
        public List<Photo> photos;
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

        /// <summary>
        /// Return all URLs of square-shaped photos from this observation.
        /// </summary>
        /// <returns>List of URL strings.</returns>
        public List<string> GetSquarePhotoUrls()
        {
            List<string> urls = new List<string>();
            foreach (Photo p in photos)
            {
                urls.Add(p.square_url);
            }
            return urls;
        }

        /// <summary>
        /// Return all URLs of full-resolution photos from this observation.
        /// </summary>
        /// <returns>List of URL strings.</returns>
        public List<string> GetFullResPhotoUrls()
        {
            List<string> urls = new List<string>();
            foreach (Photo p in photos)
            {
                urls.Add(p.medium_url);
            }
            return urls;
        }

        /// <summary>
        /// Download all square photos from this observation to the folder path specified.
        /// </summary>
        /// <param name="path">The folder to download photos to.</param>
        public void DownloadSquarePhotos(string path)
        {
            List<string> urls = GetSquarePhotoUrls();
            _DownloadPhotos(urls, path);
        }

        /// <summary>
        /// Download all full-resolution photos from this observation to the folder path specified.
        /// </summary>
        /// <param name="path">The folder to download photos to.</param>
        public void DownloadFullResPhotos(string path)
        {
            List<string> urls = GetFullResPhotoUrls();
            _DownloadPhotos(urls, path);
        }

        /// <summary>
        /// Internal helper for actually downloading the photos.
        /// </summary>
        /// <param name="urls">The urls to download photos from.</param>
        /// <param name="path">The folder to download photos to.</param>
        void _DownloadPhotos(List<string> urls, string path)
        {
            Directory.CreateDirectory(path); //create the folder if it doesn't exist
            using (WebClient client = new WebClient()) {
                foreach (string url in urls)
                {
                    MatchCollection mc = PhotoNameRegex.Matches(url);
                    if (mc.Count < 1)
                    {
                        Debug.LogWarning("Could not read photo ID from URL " + url);
                        continue;
                    }

                    string name = mc[0].Value;
                    client.DownloadFile(new System.Uri(url), Path.Combine(path, name));
                }
            }
        }


        public static Observation CreateFromJson(string jsonString)
        {
            return JsonUtility.FromJson<Observation>(jsonString);
        }

    }
}