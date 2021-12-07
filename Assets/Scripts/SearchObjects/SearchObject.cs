using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JoshAaronMiller.INaturalist
{
    /// <summary>
    /// A search object has a set of bool and string parameters for conducting a query using the INatManager.
    /// </summary>
    public abstract class SearchObject
    {
        protected Dictionary<string, bool> boolParams = new Dictionary<string, bool>();
        protected Dictionary<string, string> stringParams = new Dictionary<string, string>();

        public enum BooleanParameter
        {
            HasPhotos, HasPositionalAccuracy, HasProjectId, HasSounds, IsCaptive, 
            IsCurrent, IsEndemic, IsFromTaxonChange, IsGeoreferenced, IsIdentificationTaxonActive, 
            IsIdentified, IsLicensed, IsMappable, IsNative, IsObservationTaxonActive, 
            IsObservationTaxonActiveForIdentification, IsObserversIdentification, IsOutOfRange, 
            IsPhotoLicensed, IsPopular, IsThreatened, IsVerifiable, MatchesObservationTaxon, 
            ReturnIdOnly, WasIntroduced
        };

        static Dictionary<BooleanParameter, string> boolParamsToStrings = new Dictionary<BooleanParameter, string>() {
            { BooleanParameter.HasPhotos, "photos" },
            { BooleanParameter.HasPositionalAccuracy, "acc" },
            { BooleanParameter.HasProjectId, "pcid" },
            { BooleanParameter.HasSounds, "sounds" },
            { BooleanParameter.IsCaptive, "captive" },
            { BooleanParameter.IsCurrent, "current" }, //defaults to True
            { BooleanParameter.IsEndemic, "endemic" },
            { BooleanParameter.IsFromTaxonChange, "is_change" },
            { BooleanParameter.IsGeoreferenced, "geo" },
            { BooleanParameter.IsIdentificationTaxonActive, "taxon_active" },
            { BooleanParameter.IsIdentified, "identified" },
            { BooleanParameter.IsLicensed, "licensed" },
            { BooleanParameter.IsMappable, "mappable" },
            { BooleanParameter.IsNative, "native" },
            { BooleanParameter.IsObservationTaxonActive, "taxon_is_active" },
            { BooleanParameter.IsObservationTaxonActiveForIdentification, "observation_taxon_active" },
            { BooleanParameter.IsObserversIdentification, "own_observation" },
            { BooleanParameter.IsOutOfRange, "out_of_range" },
            { BooleanParameter.IsPhotoLicensed, "photo_licensed" },
            { BooleanParameter.IsPopular, "popular" },
            { BooleanParameter.IsThreatened, "threatened" },
            { BooleanParameter.IsVerifiable, "verifiable" },
            { BooleanParameter.MatchesObservationTaxon, "current_taxon" },
            { BooleanParameter.ReturnIdOnly, "only_id" },
            { BooleanParameter.WasIntroduced, "introduced" }
    };
        public enum StringParameter
        {
            AccMax, AccMaxOrUnknown, AccMin, ApplyProjectRules, Box, Category, ConservationStatus, 
            ConservationStatusAuthority, CreatedEndDate, CreatedOn, CreatedStartDate, Day, EndDate, 
            ExcludeId, ExcludeObservationTaxonId, ExcludeProjectId, ExcludeProjectRules, ExcludeTaxonId,
            Geoprivacy, HighestRank, IconicTaxa, IconicTaxonId, IdentificationObservedEndDate, IdentificationObservedStartDate,
            IdMax, IdMin, IdentificationAgreement, IdentifiedByUserId, IncludeId, IncludeObservationTaxonId, 
            IncludePlaceId, IncludeProjectId, IncludeTaxonId, IucnConservationStatus, Latitude, License, 
            Longitude, LowestRank, Month, NortheastLatitudeBound, NortheastLongitudeBound, 
            ObservationCreatedEndDate, ObservationCreatedStartDate, ObservationHighestRank, ObservationIconicTaxonId, 
            ObservationLowestRank, ObservationTaxonRank, ObservedOn,  OrderBy, Page, PerPage, PhotoLicense, QualityGrade, 
            Query, Radius, Reviewed, ReviewerId, SearchOnProperty, SortOrder, SoundLicense, SouthwestLatitudeBound, 
            SouthwestLongitudeBound, StartDate, TaxonGeoprivacy, TaxonName, TaxonRank, TimeToLive, UnobservedByUser, 
            UserId, UserLogin, Year
        }

        Dictionary<StringParameter, string> stringParamsToStrings = new Dictionary<StringParameter, string>() {
            { StringParameter.AccMax , "acc_below" },
            { StringParameter.AccMaxOrUnknown , "acc_below_or_unknown" },
            { StringParameter.AccMin , "acc_above" },
            { StringParameter.ApplyProjectRules , "apply_project_rules_for" },
            { StringParameter.Box , "box" },
            { StringParameter.Category , "category" },
            { StringParameter.ConservationStatus , "cs" },
            { StringParameter.ConservationStatusAuthority , "csa" },
            { StringParameter.CreatedEndDate , "created_d2" },
            { StringParameter.CreatedOn , "created_on" },
            { StringParameter.CreatedStartDate , "created_d1" },
            { StringParameter.Day , "day" },
            { StringParameter.EndDate , "d2" },
            { StringParameter.ExcludeId , "not_id" },
            { StringParameter.ExcludeObservationTaxonId , "without_observation_taxon_id" },
            { StringParameter.ExcludeProjectId , "not_in_project" },
            { StringParameter.ExcludeProjectRules , "not_matching_project_rules_for" },
            { StringParameter.ExcludeTaxonId , "without_taxon_id" },
            { StringParameter.Geoprivacy , "geoprivacy" },
            { StringParameter.HighestRank , "hrank" },
            { StringParameter.IconicTaxa , "iconic_taxa" },
            { StringParameter.IconicTaxonId , "iconic_taxon_id" },
            { StringParameter.IdentificationObservedEndDate , "observed_d2" },
            { StringParameter.IdentificationObservedStartDate , "observed_d1" },
            { StringParameter.IdMax , "id_below" },
            { StringParameter.IdMin , "id_above" },
            { StringParameter.IdentificationAgreement , "identifications" },
            { StringParameter.IdentifiedByUserId , "user_id" },
            { StringParameter.IncludeId , "id" },
            { StringParameter.IncludeObservationTaxonId , "observation_taxon_id" },
            { StringParameter.IncludePlaceId , "place_id" },
            { StringParameter.IncludeProjectId , "project_id" },
            { StringParameter.IncludeTaxonId , "taxon_id" },
            { StringParameter.IucnConservationStatus , "csi" },
            { StringParameter.Latitude , "lat" },
            { StringParameter.License , "license" },
            { StringParameter.Longitude , "lng" },
            { StringParameter.LowestRank , "lrank" },
            { StringParameter.Month , "month" },
            { StringParameter.NortheastLatitudeBound , "nelat" },
            { StringParameter.NortheastLongitudeBound , "nelng" },
            { StringParameter.ObservationCreatedEndDate , "observation_created_d2" },
            { StringParameter.ObservationCreatedStartDate , "observation_created_d1" },
            { StringParameter.ObservationHighestRank , "observation_hrank" },
            { StringParameter.ObservationIconicTaxonId , "observation_iconic_taxon_id" },
            { StringParameter.ObservationLowestRank , "observation_lrank" },
            { StringParameter.ObservationTaxonRank , "observation_rank" },
            { StringParameter.ObservedOn , "observed_on" },
            { StringParameter.OrderBy , "order_by" },
            { StringParameter.Page , "page" },
            { StringParameter.PerPage , "per_page" },
            { StringParameter.PhotoLicense , "photo_license" },
            { StringParameter.QualityGrade , "quality_grade" },
            { StringParameter.Query , "q" },
            { StringParameter.Radius , "radius" },
            { StringParameter.Reviewed , "reviewed" },
            { StringParameter.ReviewerId , "viewer_id" },
            { StringParameter.SearchOnProperty , "search_on" },
            { StringParameter.SortOrder , "order" },
            { StringParameter.SoundLicense , "sound_license" },
            { StringParameter.SouthwestLatitudeBound , "swlat" },
            { StringParameter.SouthwestLongitudeBound , "swlng" },
            { StringParameter.StartDate , "d1" },
            { StringParameter.TaxonGeoprivacy , "taxon_geoprivacy" },
            { StringParameter.TaxonName , "taxon_name" },
            { StringParameter.TaxonRank , "rank" },
            { StringParameter.TimeToLive , "TODO" },
            { StringParameter.UnobservedByUser , "unobserved_by_user_id" },
            { StringParameter.UserId , "user_id" },
            { StringParameter.UserLogin , "user_login" },
            { StringParameter.Year , "year" }
        };

        /// <summary>
        /// Add a parameter from the search.
        /// </summary>
        /// <param name="key">The key to add.</param>
        /// <param name="value">The value to add.</param>
        public void SetBooleanParameter(BooleanParameter key, bool value)
        {
            boolParams[boolParamsToStrings[key]] = value;
        }

        /// <summary>
        /// Remove a parameter from the search.
        /// </summary>
        /// <param name="key">The key to remove.</param>
        public void RemoveBooleanParameter(BooleanParameter key)
        {
            boolParams.Remove(boolParamsToStrings[key]);
        }

        /// <summary>
        /// Add a parameter from the search.
        /// </summary>
        /// <param name="key">The key to add.</param>
        /// <param name="value">The value to add.</param>
        public void SetStringParameter(StringParameter key, string value)
        {
            stringParams[stringParamsToStrings[key]] = value;
        }

        /// <summary>
        /// Remove a parameter from the search.
        /// </summary>
        /// <param name="key">The key to remove.</param>
        public void RemoveStringParameter(StringParameter key)
        {
            stringParams.Remove(stringParamsToStrings[key]);
        }

        /// <summary>
        /// Returns this search object as a string of URL parameters.
        /// </summary>
        /// <returns>A string of URL parameters</returns>
        public string ToUrlParameters()
        {
            List<string> keyValuePairs = new List<string>();

            foreach (KeyValuePair<string, bool> entry in boolParams)
            {
                keyValuePairs.Add(entry.Key + "=" + entry.Value.ToString().ToLower());
            }

            foreach (KeyValuePair<string, string> entry in stringParams)
            {
                keyValuePairs.Add(entry.Key + "=" + entry.Value);
            }

            string ret = string.Join("&", keyValuePairs);
            ret.Replace(",", "%2C");
            return ret;
        }

    }
}