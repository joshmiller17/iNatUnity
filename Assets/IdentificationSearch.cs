using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JoshAaronMiller.INaturalist
{

    /// <summary>
    /// An ObservationSearch is a set of parameters for searching iNaturalist Observations.
    /// Usage: myINatManager.SearchObservations(myObservationSearch)
    /// </summary>
    public class IdentificationSearch
    {
        public enum BooleanParameter
        {
            MatchesObservationTaxon, IsObserversIdentification, IsFromTaxonChange, IsTaxonActive,
            IsObservationTaxonActive, IsCurrent, ReturnIdOnly
        };

        public static Dictionary<BooleanParameter, string> BoolParamToString = new Dictionary<BooleanParameter, string>() {
        { BooleanParameter.MatchesObservationTaxon, "current_taxon" },
        { BooleanParameter.IsObserversIdentification, "own_observation" },
        { BooleanParameter.IsFromTaxonChange, "is_change" },
        { BooleanParameter.IsTaxonActive, "taxon_active" },
        { BooleanParameter.IsObservationTaxonActive, "observation_taxon_active" },
        { BooleanParameter.IsCurrent, "current" },
        { BooleanParameter.ReturnIdOnly, "only_id" }
    };


        Dictionary<string, bool> boolParams = new Dictionary<string, bool>();
        Dictionary<string, string> stringParams = new Dictionary<string, string>();


        public void SetBooleanParameter(BooleanParameter param, bool setting)
        {
            boolParams[BoolParamToString[param]] = setting;
        }


        /// <summary>
        /// Require the search to include these IDs.
        /// </summary>
        /// <param name="ids">IDs to include</param>
        public void IncludeIds(List<int> ids)
        {
            stringParams["id"] = string.Join(",", ids);
        }


        /// <summary>
        /// Require the observations to be within these place IDs.
        /// </summary>
        /// <param name="ids">Place IDs to require</param>
        public void IncludePlaceIds(List<int> ids)
        {
            stringParams["place_id"] = string.Join(",", ids);
        }

        /// <summary>
        /// Limit the search to these taxa IDs and their descendents.
        /// </summary>
        /// <param name="ids">Taxa IDs to include</param>
        public void IncludeTaxonIds(List<int> ids)
        {
            stringParams["taxon_id"] = string.Join(",", ids);
        }

        /// <summary>
        /// Limit the search to exclude these taxa IDs and their descendents.
        /// </summary>
        /// <param name="ids">Taxa IDs to exclude</param>
        public void ExcludeTaxonIds(List<int> ids)
        {
            stringParams["without_taxon_id"] = string.Join(",", ids);
        }




        // TODO pick up from here







        /// <summary>
        /// Limit the search to observations observed within a timeframe. Both start and end dates are optional.
        /// </summary>
        /// <param name="start">The start date, formatted as YYYY-MM-DD. Limit observations to on or after this date.</param>
        /// <param name="end">The end date, formatted as YYYY-MM-DD. Limit observations to on or before this date.</param>
        public void SetObservedOnDateLimits(string start = "", string end = "")
        {
            if (start == end && start != "")
            {
                stringParams["observed_on"] = start;
            }
            else
            {
                if (start != "")
                {
                    stringParams["d1"] = start;
                }
                if (end != "")
                {
                    stringParams["d2"] = start;
                }
            }
        }

        /// <summary>
        /// Limit the search to observations created within a timeframe. Both start and end dates are optional.
        /// </summary>
        /// <param name="start">The start datetime, formatted as ISO-8601 datetime format: YYYY-MM-DDTHH:MMSS.mmmZ. Limit observations to created on or after this date.</param>
        /// <param name="end">The end datetime, formatted as ISO-8601 datetime format: YYYY-MM-DDTHH:MMSS.mmmZ. Limit observations to created on or before this date.</param>
        public void SetCreatedOnDateTimeLimits(string start = "", string end = "")
        {
            if (start == end && start != "")
            {
                stringParams["created_on"] = start;
            }
            else
            {
                if (start != "")
                {
                    stringParams["created_d1"] = start;
                }
                if (end != "")
                {
                    stringParams["created_d2"] = start;
                }
            }
        }

        /// <summary>
        /// Limit the search to taxa that this user has not previously observed.
        /// </summary>
        /// <param name="userId">The user ID to base this restriction on.</param>
        public void SetUnobservedByUserId(int userId)
        {
            stringParams["unobserved_by_user_id"] = userId.ToString();
        }

        /// <summary>
        /// Limit the search to the project restrictions of the given project ID.
        /// </summary>
        /// <param name="projectId">The project ID to base this restriction on.</param>
        public void ApplyProjectRulesFor(int projectId)
        {
            stringParams["apply_project_rules_for"] = projectId.ToString();
        }

        /// <summary>
        /// Limit the search to taxa of this conservation status code. If PlaceId is also set, only consider statuses specific to those places.
        /// </summary>
        /// <param name="code">The conservation status code.</param>
        public void SetConservationStatusCode(string code)
        {
            stringParams["cs"] = code;
        }

        /// <summary>
        /// Limit the search to taxa with a conservation status from this authority. If PlaceId is also set, only consider statuses specific to those places.
        /// </summary>
        /// <param name="auth">The conservation status authority.</param>
        public void SetConservationStatusAuthority(string auth)
        {
            stringParams["csa"] = auth;
        }

        /// <summary>
        /// Limit the search to taxa with these IUCN conservation statuses. If PlaceId is also set, only consider statuses specific to those places.
        /// </summary>
        /// <param name="cs">The IUCN conservation statuses.</param>
        public void IncludeIUCNConservationStatuses(List<IucnConservationStatus> statuses)
        {
            List<string> stringStatuses = new List<string>();
            foreach (IucnConservationStatus cs in statuses)
            {

                if (cs != IucnConservationStatus.None)
                {
                    stringStatuses.Add(cs.ToString().ToUpper());
                }
            }
            if (stringStatuses.Count > 0)
            {
                stringParams["csi"] = string.Join(",", stringStatuses);
            }
        }

        /// <summary>
        /// Limit the search to observations with these geoprivacy settings.
        /// </summary>
        /// <param name="geo">The geoprivacy settings.</param>
        public void SetGeoprivacy(List<Geoprivacy> geos)
        {
            List<string> geoStrings = new List<string>();
            foreach (Geoprivacy geo in geos)
            {
                if (geo != Geoprivacy.None)
                {
                    geoStrings.Add(GeoprivacyToString[geo]);
                }
            }

            if (geoStrings.Count > 0)
            {
                stringParams["geoprivacy"] = string.Join(",", geoStrings);
            }
        }

        /// <summary>
        /// Filter observations by the most conservative geoprivacy applied by a conservation status associated with one of the taxa proposed in the current identifications.
        /// </summary>
        /// <param name="geo">The geoprivacy settings.</param>
        public void IncludeTaxonGeoprivacy(List<Geoprivacy> geos)
        {
            List<string> geoStrings = new List<string>();
            foreach (Geoprivacy geo in geos)
            {
                if (geo != Geoprivacy.None)
                {
                    geoStrings.Add(GeoprivacyToString[geo]);
                }
            }

            if (geoStrings.Count > 0)
            {
                stringParams["taxon_geoprivacy"] = string.Join(",", geoStrings);
            }
        }

        /// <summary>
        /// Limit the search to taxa between these limits.
        /// </summary>
        /// <param name="lowest">The lowest taxon rank to include in the search (include this and higher).</param>
        /// <param name="highest">The highest taxon rank to include in the search (include this and lower).</param>
        public void SetTaxonRankLimits(TaxonRank lowest = TaxonRank.None, TaxonRank highest = TaxonRank.None)
        {
            if (lowest != TaxonRank.None)
            {
                stringParams["lrank"] = lowest.ToString().ToLower();
            }

            if (highest != TaxonRank.None)
            {
                stringParams["hrank"] = highest.ToString().ToLower();
            }
        }

        /// <summary>
        /// Limit the search to observations with these iconic taxa.
        /// </summary>
        /// <param name="iconicTaxa">The iconic taxa to include.</param>
        public void SetIconicTaxa(List<IconicTaxon> iconicTaxa)
        {
            List<string> taxaStrings = new List<string>();
            foreach (IconicTaxon taxon in iconicTaxa)
            {
                if (taxon == IconicTaxon.None)
                {
                    continue;
                }
                else if (taxon == IconicTaxon.Unknown)
                {
                    taxaStrings.Add("unknown");
                }
                else
                {
                    taxaStrings.Add(taxon.ToString());
                }
            }

            if (taxaStrings.Count > 0)
            {
                stringParams["iconic_taxa"] = string.Join(",", taxaStrings);
            }
        }

        /// <summary>
        /// Limit the search to observations with IDs within this range. Both min and max are optional.
        /// </summary>
        /// <param name="min">Include this ID and higher.</param>
        /// <param name="max">Include this ID and lower.</param>
        public void SetObservationIdLimits(int min = -1, int max = -1)
        {
            if (min >= 0)
            {
                stringParams["id_above"] = (min - 1).ToString(); //API lists min and max as exclusive
            }
            if (max >= 0)
            {
                stringParams["id_below"] = (max + 1).ToString(); //API lists min and max as exclusive
            }
        }

        public void SetIdentificationAgreement(IdentificationAgreement agreement)
        {
            if (agreement != IdentificationAgreement.None)
            {
                stringParams["identifications"] = IdentAgreementToString[agreement];
            }
        }

        /// <summary>
        /// Limit the search to a circle of [radius] kilometers around the specified latitude and longitude.
        /// </summary>
        /// <param name="lat">The latitude of the search's center.</param>
        /// <param name="lng">The longitude of the search's center.</param>
        /// <param name="radius">The radius of search in kilometers.</param>
        public void SetBoundingCircle(double lat, double lng, double radius)
        {
            stringParams["lat"] = lat.ToString();
            stringParams["lng"] = lng.ToString();
            stringParams["radius"] = radius.ToString();
        }

        /// <summary>
        /// Limit the search within a bounding box specified by northeast and southwest corners given in latitude and longitude.
        /// </summary>
        /// <param name="nelat">The northeast latitude.</param>
        /// <param name="nelng">The northeast longitude.</param>
        /// <param name="swlat">The southwest latitude.</param>
        /// <param name="swlng">The southwest longitude.</param>
        public void SetBoundingBox(double nelat, double nelng, double swlat, double swlng)
        {
            stringParams["nelat"] = nelat.ToString();
            stringParams["nelng"] = nelng.ToString();
            stringParams["swlat"] = swlat.ToString();
            stringParams["swlng"] = swlng.ToString();
        }


        public void SetListId()
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Require the observations to be not in this project ID.
        /// </summary>
        /// <param name="ids">Project ID to exclude</param>
        public void ExcludeProjectId(int id)
        {
            stringParams["not_in_project"] = id.ToString();
        }

        /// <summary>
        /// Limit the search to observations that don't match the rules of the given project ID.
        /// </summary>
        /// <param name="projectId">The project ID to base this restriction on.</param>
        public void ExcludeProjectRulesFor(int projectId)
        {
            stringParams["not_matching_project_rules_for"] = projectId.ToString();
        }


        /// <summary>
        /// Search observation properties matching the query.
        /// </summary>
        /// <param name="property">The search property to apply the query to.</param>
        /// <param name="query">The search query.</param>
        public void SearchOnProperties(SearchProperty property, string query)
        {
            stringParams["q"] = query;
            if (property != SearchProperty.All) //defaults to all
            {
                stringParams["search_on"] = property.ToString().ToLower();
            }
        }

        /// <summary>
        /// Limit the search to observations of this quality grade.
        /// </summary>
        /// <param name="quality">The quality grade to search for.</param>
        public void SetQualityGrade(QualityGrade quality)
        {
            if (quality != QualityGrade.None)
            {
                stringParams["quality_grade"] = QualityToString[quality];
            }
        }

        public void SetUpdatedSince()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Limit the search to observations that [have / have not] been reviewed by a given user.
        /// </summary>
        /// <param name="userId">The user ID to consider their review.</param>
        /// <param name="hasReviewed">If true, only include observations they have reviewed. If false, only include observations they have not reviewed.</param>
        public void SetReviewedByUser(int userId, bool hasReviewed)
        {
            stringParams["reviewed"] = hasReviewed.ToString().ToLower();
            stringParams["viewer_id"] = userId.ToString();
        }

        public void SetLocalePreference()
        {
            throw new NotImplementedException();
        }

        public void SetPreferredPlaceId()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Set the Cache-Control HTTP header with this value as max-age, in seconds.This means subsequent identical requests will be cached on iNaturalist servers, and commonly within web browsers
        /// </summary>
        /// <param name="timeToLive">The max-age of the request, in seconds.</param>
        public void SetCacheControl(int timeToLive)
        {
            stringParams["ttl"] = timeToLive.ToString();
        }

        /// <summary>
        /// Set the number of search results per page and which page to receive. For example, SetPagination(50,2) would return results 51-100.
        /// </summary>
        /// <param name="resultsPerPage">How many results to return per page (default 30).</param>
        /// <param name="page">Which page of results to return (default 1).</param>
        public void SetPagination(int resultsPerPage = 30, int page = 1)
        {
            stringParams["per_page"] = resultsPerPage.ToString();
            stringParams["page"] = page.ToString();
        }

        /// <summary>
        /// Set how the results are ordered. Defaults to created at date, descending.
        /// </summary>
        /// <param name="orderBy">The parameter to sort the order by.</param>
        /// <param name="sortOrder">Whether to sort ascending or descending.</param>
        public void SetOrder(OrderBy orderBy, SortOrder sortOrder)
        {
            stringParams["order"] = sortOrder.ToString().ToLower();
            stringParams["order_by"] = OrderByToString[orderBy];
        }


        /// <summary>
        /// Returns this ObservationSearch as a string of URL parameters.
        /// For use with INatManager::SearchObservations()
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