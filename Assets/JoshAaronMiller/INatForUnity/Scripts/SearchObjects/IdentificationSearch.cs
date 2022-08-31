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
            SetStringParameter(StringParameter.IncludeId, string.Join(",", ids));
        }


        /// <summary>
        /// Require the observations to be within these place IDs.
        /// </summary>
        /// <param name="ids">Place IDs to require</param>
        public void IncludePlaceIds(List<int> ids)
        {
            SetStringParameter(StringParameter.IncludePlaceId, string.Join(",", ids));
        }

        /// <summary>
        /// Limit the search to these taxa IDs and their descendents.
        /// </summary>
        /// <param name="ids">Taxa IDs to include</param>
        public void IncludeTaxonIds(List<int> ids)
        {
            SetStringParameter(StringParameter.IncludeTaxonId, string.Join(",", ids));
        }

        /// <summary>
        /// Limit the search to observations with these taxa IDs and their descendents.
        /// </summary>
        /// <param name="ids">Observation taxa IDs to include</param>
        public void IncludeObservationTaxonIds(List<int> ids)
        {
            SetStringParameter(StringParameter.IncludeObservationTaxonId, string.Join(",", ids));
        }

        /// <summary>
        /// Limit the search to identifications with these iconic taxa.
        /// </summary>
        /// <param name="ids">Iconic taxa IDs to include</param>
        public void IncludeIconicTaxonIds(List<int> ids)
        {
            SetStringParameter(StringParameter.IconicTaxonId, string.Join(",", ids));
        }

        /// <summary>
        /// Limit the search to identifications of observations with these iconic taxa.
        /// </summary>
        /// <param name="ids">Observation iconic taxa IDs to include</param>
        public void IncludeObservationIconicTaxonIds(List<int> ids)
        {
            SetStringParameter(StringParameter.ObservationIconicTaxonId, string.Join(",", ids));
        }

        /// <summary>
        /// Limit the search to exclude these taxa IDs and their descendents.
        /// </summary>
        /// <param name="ids">Taxa IDs to exclude</param>
        public void ExcludeTaxonIds(List<int> ids)
        {
            SetStringParameter(StringParameter.ExcludeTaxonId, string.Join(",", ids));
        }

        /// <summary>
        /// Limit the search to exclude identifications of observations with taxa IDs and their descendents.
        /// </summary>
        /// <param name="ids">Observation taxa IDs to exclude</param>
        public void ExcludeObservationTaxonIds(List<int> ids)
        {
            SetStringParameter(StringParameter.ExcludeObservationTaxonId, string.Join(",", ids));
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
                SetStringParameter(StringParameter.IdentificationObservedStartDate, start);
            }
            if (end != "")
            {
                SetStringParameter(StringParameter.IdentificationObservedEndDate, end);
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
                SetStringParameter(StringParameter.StartDate, start);
            }
            if (end != "")
            {
                SetStringParameter(StringParameter.EndDate, end);
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
                SetStringParameter(StringParameter.ObservationCreatedStartDate, start);
            }
            if (end != "")
            {
                SetStringParameter(StringParameter.ObservationCreatedEndDate, end);
            }
        }

        /// <summary>
        /// Limit the search to identifications of taxa of these ranks.
        /// </summary>
        /// <param name="ranks">The list of taxon ranks to include in the search.</param>
        public void IncludeTaxonRanks(List<TaxonRank> ranks)
        {
            SetStringParameter(StringParameter.TaxonRank, string.Join(",", ranks));
        }

        /// <summary>
        /// Limit the search to identifications of observations that have a taxon of these ranks.
        /// </summary>
        /// <param name="ranks">The list of observation taxon ranks to include in the search.</param>
        public void IncludeObservationTaxonRanks(List<TaxonRank> ranks)
        {
            SetStringParameter(StringParameter.ObservationTaxonRank, string.Join(",", ranks));
        }

        /// <summary>
        /// Limit the search to identifications made by users of these IDs.
        /// </summary>
        /// <param name="userIds">The list of user IDs; limit the search to their identifications.</param>
        public void IncludeIdentificationsByUserId(List<int> userIds)
        {
            SetStringParameter(StringParameter.UserId, string.Join(",", userIds));
        }

        /// <summary>
        /// Limit the search to identifications made by users with these login names.
        /// </summary>
        /// <param name="userLogins">The list of usernames; limit the search to their identifications.</param>
        public void IncludeIdentificationsByUserLogin(List<int> userLogins)
        {
            SetStringParameter(StringParameter.UserLogin, string.Join(",", userLogins));
        }

        /// <summary>
        /// Limit the search to identifications with these category labels.
        /// </summary>
        /// <param name="categories">The identification categories to include in the search.</param>
        public void IncludeCategories(List<string> categories)
        {
            if (categories.Contains("Any"))
            {
                RemoveStringParameter(StringParameter.Category); //Any is already default
            }
            else
            {
                SetStringParameter(StringParameter.Category, string.Join(",", categories));
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
                SetStringParameter(StringParameter.LowestRank, lowest.ToString().ToLower());
            }
            else
            {
                RemoveStringParameter(StringParameter.LowestRank);
            }

            if (highest != TaxonRank.None)
            {
                SetStringParameter(StringParameter.HighestRank, highest.ToString().ToLower());
            }
            else
            {
                RemoveStringParameter(StringParameter.HighestRank);
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
                SetStringParameter(StringParameter.ObservationLowestRank, lowest.ToString().ToLower());
            }
            else
            {
                RemoveStringParameter(StringParameter.ObservationLowestRank);
            }

            if (highest != TaxonRank.None)
            {
                SetStringParameter(StringParameter.ObservationHighestRank, highest.ToString().ToLower());
            }
            else
            {
                RemoveStringParameter(StringParameter.ObservationHighestRank);
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
                SetStringParameter(StringParameter.QualityGrade, Enums.QualityToString[quality]);
            }
            else
            {
                RemoveStringParameter(StringParameter.QualityGrade);
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
                SetStringParameter(StringParameter.IdMin, (min - 1).ToString()); //API lists min and max as exclusive
            }
            if (max >= 0)
            {
                SetStringParameter(StringParameter.IdMax, (max + 1).ToString()); //API lists min and max as exclusive
            }
        }

        /// <summary>
        /// Set the number of search results per page and which page to receive. For example, SetPagination(50,2) would return results 51-100.
        /// </summary>
        /// <param name="resultsPerPage">How many results to return per page (default 30).</param>
        /// <param name="page">Which page of results to return (default 1).</param>
        public void SetPagination(int resultsPerPage = 30, int page = 1)
        {
            SetStringParameter(StringParameter.PerPage, resultsPerPage.ToString());
            SetStringParameter(StringParameter.Page, page.ToString());
        }

        /// <summary>
        /// Set how the results are ordered. Defaults to created at date, descending.
        /// </summary>
        /// <param name="orderBy">The parameter to sort the order by.</param>
        /// <param name="sortOrder">Whether to sort ascending or descending.</param>
        public void SetOrder(OrderBy orderBy, SortOrder sortOrder)
        {
            SetStringParameter(StringParameter.SortOrder, sortOrder.ToString().ToLower());
            SetStringParameter(StringParameter.OrderBy, Enums.OrderByToString[orderBy]);
        }
    }
}