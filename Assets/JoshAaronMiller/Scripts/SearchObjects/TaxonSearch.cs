using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JoshAaronMiller.INaturalist
{

    /// <summary>
    /// A TaxonSearch is a set of parameters for searching iNaturalist Taxa.
    /// Usage: myINatManager.SearchTaxa(myTaxonSearch)
    /// </summary>
    public class TaxonSearch : SearchObject
    {
        /// <summary>
        /// Limit the search to taxa that begin with this substring
        /// </summary>
        /// <param name="query">The start of the taxa names</param>
        public void SetNameBeginsWith(string query)
        {
            stringParams["q"] = query;
        }

        /// <summary>
        /// Require the search to include these IDs.
        /// </summary>
        /// <param name="ids">IDs to include</param>
        public void IncludeIds(List<int> ids)
        {
            stringParams["taxon_id"] = string.Join(",", ids);
        }

        /// <summary>
        /// Limit the search to children of this taxon ID.
        /// </summary>
        /// <param name="parentId">The parent taxon's ID</param>
        public void SetParentId(int parentId)
        {
            stringParams["parent_id"] = parentId.ToString();
        }

        /// <summary>
        /// Limit the search to taxa of these ranks.
        /// </summary>
        /// <param name="ranks">The list of taxon ranks to include in the search.</param>
        public void IncludeTaxonRanks(List<TaxonRank> ranks)
        {
            stringParams["rank"] = string.Join(",", ranks);
        }

        /// <summary>
        /// Limit the search to taxa of this rank level.
        /// </summary>
        /// <param name="rankLevel">The rank level to limit the search to.</param>
        public void SetTaxonRankLevel(TaxonRankLevel rankLevel)
        {
            if (rankLevel != TaxonRankLevel.None)
            {
                stringParams["rank_level"] = ((int)rankLevel).ToString();
            }
        }


        /// <summary>
        /// Limit the search to taxa with IDs within this range. Both min and max are optional.
        /// </summary>
        /// <param name="min">Include this ID and higher.</param>
        /// <param name="max">Include this ID and lower.</param>
        public void SetTaxonIdLimits(int min = -1, int max = -1)
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
        /// Set the locale preference for taxon common names.
        /// </summary>
        /// <remarks>
        /// I'm not really sure what a valid input looks like.
        /// </remarks>
        /// <param name="locale">The locale preference.</param>
        public void SetLocalePreference(string locale)
        {
            stringParams["locale"] = locale;
        }

        /// <summary>
        /// Set a place preference for regional taxon common names.
        /// </summary>
        /// <remarks>
        /// I think this means that taxon common names will be based on the place specified.
        /// </remarks>
        /// <param name="placeId">The place ID of the preference.</param>
        public void SetPreferredPlaceId(int placeId)
        {
            stringParams["preferred_place_id"] = placeId.ToString();
        }
    }
}