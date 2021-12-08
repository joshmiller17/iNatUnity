using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JoshAaronMiller.INaturalist
{

    /// <summary>
    /// An IdentificationSearch is a set of parameters for searching iNaturalist Identifications.
    /// Usage: myINatManager.SearchIdentifications(myIdentificationSearch)
    /// </summary>
    public class IdentificationSearch : SearchObject
    { 

        public enum IdentificationCategory
        {
            Any, Improving, Supporting, Leading, Maverick
        };

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
        /// Limit the search to observations with these taxa IDs and their descendents.
        /// </summary>
        /// <param name="ids">Observation taxa IDs to include</param>
        public void IncludeObservationTaxonIds(List<int> ids)
        {
            stringParams["observation_taxon_id"] = string.Join(",", ids);
        }

        /// <summary>
        /// Limit the search to identifications with these iconic taxa.
        /// </summary>
        /// <param name="ids">Iconic taxa IDs to include</param>
        public void IncludeIconicTaxonIds(List<int> ids)
        {
            stringParams["iconic_taxon_id"] = string.Join(",", ids);
        }

        /// <summary>
        /// Limit the search to identifications of observations with these iconic taxa.
        /// </summary>
        /// <param name="ids">Observation iconic taxa IDs to include</param>
        public void IncludeObservationIconicTaxonIds(List<int> ids)
        {
            stringParams["observation_iconic_taxon_id"] = string.Join(",", ids);
        }

        /// <summary>
        /// Limit the search to exclude these taxa IDs and their descendents.
        /// </summary>
        /// <param name="ids">Taxa IDs to exclude</param>
        public void ExcludeTaxonIds(List<int> ids)
        {
            stringParams["without_taxon_id"] = string.Join(",", ids);
        }

        /// <summary>
        /// Limit the search to exclude identifications of observations with taxa IDs and their descendents.
        /// </summary>
        /// <param name="ids">Observation taxa IDs to exclude</param>
        public void ExcludeObservationTaxonIds(List<int> ids)
        {
            stringParams["without_observation_taxon_id"] = string.Join(",", ids);
        }


        /// <summary>
        /// Limit the search to observations observed within a timeframe. Both start and end dates are optional.
        /// </summary>
        /// <param name="start">The start date, formatted as YYYY-MM-DD. Limit observations to on or after this date.</param>
        /// <param name="end">The end date, formatted as YYYY-MM-DD. Limit observations to on or before this date.</param>
        public void SetObservedOnDateLimits(string start = "", string end = "")
        {
            if (start != "")
            {
                stringParams["observed_d1"] = start;
            }
            if (end != "")
            {
                stringParams["observed_d2"] = start;
            }
        }

        /// <summary>
        /// Limit the search to identifications created within a timeframe. Both start and end dates are optional.
        /// </summary>
        /// <param name="start">The start date, formatted as YYYY-MM-DD. Limit identifications created on or after this date.</param>
        /// <param name="end">The end date, formatted as YYYY-MM-DD. Limit identifications created on or before this date.</param>
        public void SetCreatedOnDateLimits(string start = "", string end = "")
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

        /// <summary>
        /// Limit the search to identifications of observations created within a timeframe. Both start and end dates are optional.
        /// </summary>
        /// <param name="start">The start date, formatted as YYYY-MM-DD. Limit identifications to observations created on or after this date.</param>
        /// <param name="end">The end date, formatted as YYYY-MM-DD. Limit identifications to observations created on or before this date.</param>
        public void SetObservationCreatedOnDateLimits(string start = "", string end = "")
        {
            if (start != "")
            {
                stringParams["observation_created_d1"] = start;
            }
            if (end != "")
            {
                stringParams["observation_created_d2"] = start;
            }
        }

        /// <summary>
        /// Limit the search to identifications of taxa of these ranks.
        /// </summary>
        /// <param name="ranks">The list of taxon ranks to include in the search.</param>
        public void IncludeTaxonRanks(List<TaxonRank> ranks)
        {
            stringParams["rank"] = string.Join(",", ranks);
        }

        /// <summary>
        /// Limit the search to identifications of observations that have a taxon of these ranks.
        /// </summary>
        /// <param name="ranks">The list of observation taxon ranks to include in the search.</param>
        public void IncludeObservationTaxonRanks(List<TaxonRank> ranks)
        {
            stringParams["observation_rank"] = string.Join(",", ranks);
        }

        /// <summary>
        /// Limit the search to identifications made by users of these IDs.
        /// </summary>
        /// <param name="userIds">The list of user IDs; limit the search to their identifications.</param>
        public void IncludeIdentificationsByUserId(List<int> userIds)
        {
            stringParams["user_id"] = string.Join(",", userIds);
        }

        /// <summary>
        /// Limit the search to identifications made by users with these login names.
        /// </summary>
        /// <param name="userLogins">The list of usernames; limit the search to their identifications.</param>
        public void IncludeIdentificationsByUserLogin(List<int> userLogins)
        {
            stringParams["user_login"] = string.Join(",", userLogins);
        }

        /// <summary>
        /// Limit the search to identifications with these category labels.
        /// </summary>
        /// <param name="categories">The identification categories to include in the search.</param>
        public void IncludeCategories(List<string> categories)
        {
            if (categories.Contains("Any"))
            {
                stringParams.Remove("category"); //Any is already default
            }
            else
            {
                stringParams["category"] = string.Join(",", categories);
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
            else
            {
                stringParams.Remove("lrank");
            }

            if (highest != TaxonRank.None)
            {
                stringParams["hrank"] = highest.ToString().ToLower();
            }
            else
            {
                stringParams.Remove("hrank");
            }
        }

        /// <summary>
        /// Limit the search to identifications of observations with taxa between these limits.
        /// </summary>
        /// <param name="lowest">The lowest observation taxon rank to include in the search (include this and higher).</param>
        /// <param name="highest">The highest observation taxon rank to include in the search (include this and lower).</param>
        public void SetObservationTaxonRankLimits(TaxonRank lowest = TaxonRank.None, TaxonRank highest = TaxonRank.None)
        {
            if (lowest != TaxonRank.None)
            {
                stringParams["observation_lrank"] = lowest.ToString().ToLower();
            }

            if (highest != TaxonRank.None)
            {
                stringParams["observation_hrank"] = highest.ToString().ToLower();
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
                stringParams["quality_grade"] = Enums.QualityToString[quality];
            }
            else
            {
                stringParams.Remove("quality_grade");
            }
        }

        /// <summary>
        /// Limit the search to identifications with IDs within this range. Both min and max are optional.
        /// </summary>
        /// <param name="min">Include this ID and higher.</param>
        /// <param name="max">Include this ID and lower.</param>
        public void SetIdentificationnIdLimits(int min = -1, int max = -1)
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
            stringParams["order_by"] = Enums.OrderByToString[orderBy];
        }
    }
}