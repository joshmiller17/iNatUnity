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
    public class ObservationSearch : SearchObject
    {
        public enum BooleanParameter
        {
            HasPositionalAccuracy, IsCaptive, IsEndemic, IsGeoreferenced,
            IsIdentified, WasIntroduced, IsMappable, IsNative, IsOutOfRange, HasProjectId, HasPhotos,
            IsPopular, HasSounds, IsTaxonActive, IsThreatened, IsVerifiable, IsLicensed,
            IsPhotoLicensed, ReturnIdOnly
        };

        public static Dictionary<BooleanParameter, string> BoolParamToString = new Dictionary<BooleanParameter, string>() {
        { BooleanParameter.HasPositionalAccuracy, "acc" },
        { BooleanParameter.IsCaptive, "captive" },
        { BooleanParameter.IsEndemic, "endemic" },
        { BooleanParameter.IsGeoreferenced, "geo" },
        { BooleanParameter.IsIdentified, "identified" },
        { BooleanParameter.WasIntroduced, "introduced" },
        { BooleanParameter.IsMappable, "mappable" },
        { BooleanParameter.IsNative, "native" },
        { BooleanParameter.IsOutOfRange, "out_of_range" },
        { BooleanParameter.HasProjectId, "pcid" },
        { BooleanParameter.HasPhotos, "photos" },
        { BooleanParameter.IsPopular, "popular" },
        { BooleanParameter.HasSounds, "sounds" },
        { BooleanParameter.IsTaxonActive, "taxon_is_active" },
        { BooleanParameter.IsThreatened, "threatened" },
        { BooleanParameter.IsVerifiable, "verifiable" },
        { BooleanParameter.IsLicensed, "licensed" },
        { BooleanParameter.IsPhotoLicensed, "photo_licensed" },
        { BooleanParameter.ReturnIdOnly, "only_id" }
    };

        public enum License
        {
            None, CcAttr, CcAttrNonCommercial, CcAttrNoDerivs, CcShareAlike,
            CcAttrNonCommercialNoDerivs, CcAttrNonCommercialShareAlike, Cc0
        };

        public static Dictionary<License, string> LicenseToString = new Dictionary<License, string>() {
        { License.None, "" }, // doesn't distinguish between no license restrictions and no license (all rights reserved)
        { License.CcAttr, "cc-by" },
        { License.CcAttrNonCommercial, "cc-by-nc" },
        { License.CcAttrNoDerivs, "cc-by-nd" },
        { License.CcShareAlike, "cc-by-sa" },
        { License.CcAttrNonCommercialNoDerivs, "cc-by-nc-nd" },
        { License.CcAttrNonCommercialShareAlike, "cc-by-nc-sa" },
        { License.Cc0, "cc0" }
    };

        public enum IucnConservationStatus
        {
            None, Lc, Nt, Vu, En, Cr, Ew, Ex
        };

        public enum Geoprivacy
        {
            None, Obscured, ObscuredPrivate, Open, Private
        };

        public static Dictionary<Geoprivacy, string> GeoprivacyToString = new Dictionary<Geoprivacy, string>() {
        { Geoprivacy.None, "" },
        { Geoprivacy.Obscured, "obscured" },
        { Geoprivacy.ObscuredPrivate, "obscured_private" },
        { Geoprivacy.Open, "open" },
        { Geoprivacy.Private, "private" },
    };

        public enum IconicTaxon
        {
            None, Actinopterygii, Animalia, Amphibia, Arachnida, Aves, Chromista, Fungi,
            Insecta, Mammalia, Mollusca, Reptilia, Plantae, Protozoa, Unknown
        };

        public enum IdentificationAgreement
        {
            None, MostAgree, MostDisagree, SomeAgree
        };

        public static Dictionary<IdentificationAgreement, string> IdentAgreementToString = new Dictionary<IdentificationAgreement, string>()
    {
        { IdentificationAgreement.None, "" },
        { IdentificationAgreement.MostAgree, "most_agree" },
        { IdentificationAgreement.MostDisagree, "most_disagree" },
        { IdentificationAgreement.SomeAgree, "some_agree" }
    };

        public enum SearchProperty
        {
            All, Names, Tags, Description, Place
        }

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
        /// Require the search to exclude these IDs.
        /// </summary>
        /// <param name="ids">IDs to exclude</param>
        public void ExcludeIds(List<int> ids)
        {
            stringParams["not_id"] = string.Join(",", ids);
        }

        /// <summary>
        /// Require the observations returned to have this license.
        /// </summary>
        /// <param name="license">Required license</param>
        public void SetLicense(License license)
        {
            if (license != License.None)
            {
                stringParams["license"] = LicenseToString[license];
            }
            else
            {
                stringParams.Remove("license");
            }
        }

        public void SetOfvDataType()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Require the observations to have at least one photo with this license.
        /// </summary>
        /// <param name="license">Required license</param>
        public void SetPhotoLicense(License license)
        {
            if (license != License.None)
            {
                stringParams["photo_license"] = LicenseToString[license];
            }
            else
            {
                stringParams.Remove("photo_license");
            }
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
        /// Require the observations to be included in these project IDs.
        /// </summary>
        /// <param name="ids">Project IDs to search</param>
        public void IncludeProjectIds(List<int> ids)
        {
            stringParams["project_id"] = string.Join(",", ids);
        }

        /// <summary>
        /// Require the observations returned to have this taxon rank.
        /// </summary>
        /// <param name="license">Required license</param>
        public void SetTaxonRank(TaxonRank rank)
        {
            if (rank != TaxonRank.None)
            {
                stringParams["rank"] = rank.ToString().ToLower();
            }
            else
            {
                stringParams.Remove("rank");
            }
        }

        public void SetSiteId()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Require the observations to have at least one sound with this license.
        /// </summary>
        /// <param name="license">Required license</param>
        public void SetSoundLicense(License license)
        {
            if (license != License.None)
            {
                stringParams["sound_license"] = LicenseToString[license];
            }
            else
            {
                stringParams.Remove("sound_license");
            }
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

        /// <summary>
        /// Limit the search to these scientific or common taxa names.
        /// </summary>
        /// <param name="ids">Taxa names to match</param>
        public void IncludeTaxonNames(List<string> names)
        {
            stringParams["taxon_name"] = string.Join(",", names);
        }

        public void IncludeUserIds()
        {
            throw new NotImplementedException();
        }

        public void IncludeUserLogins()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Limit the search to observations with identifications by the user with this ID.
        /// </summary>
        /// <param name="userId">The user's ID to restrict the search to.</param>
        public void SetIdentifiedByUserId(int userId)
        {
            stringParams["ident_user_id"] = userId.ToString();
        }

        /// <summary>
        /// Limit the search to observations on these days of the month.
        /// </summary>
        /// <param name="days">Days to match.</param>
        public void IncludeDays(List<int> days)
        {
            stringParams["day"] = string.Join(",", days);
        }

        /// <summary>
        /// Limit the search to observations on these months.
        /// </summary>
        /// <param name="months">Months to match.</param>
        public void IncludeMonths(List<int> months)
        {
            stringParams["month"] = string.Join(",", months);
        }

        /// <summary>
        /// Limit the search to observations on these years.
        /// </summary>
        /// <param name="months">Years to match.</param>
        public void IncludeYears(List<int> years)
        {
            stringParams["year"] = string.Join(",", years);
        }

        public void IncludeTermIds()
        {
            throw new NotImplementedException();
        }

        public void IncludeTermValueIds()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Limit the search to observations with a positional accuracy above this value.
        /// </summary>
        /// <param name="min">The positional accuracy in meters.</param>
        public void SetAccuracyMinimum(int min)
        {
            stringParams["acc_above"] = min.ToString();
        }

        /// <summary>
        /// Limit the search to observations with a positional accuracy below this value.
        /// </summary>
        /// <param name="max">The positional accuracy in meters.</param>
        /// <param name="canBeUnknown">Whether the positional accuracy can be unknown.</param>
        public void SetAccuracyMaximum(int max, bool canBeUnknown = false)
        {
            if (canBeUnknown)
            {
                stringParams["acc_below_or_unknown"] = max.ToString();
            }
            else
            {
                stringParams["acc_below"] = max.ToString();
            }
        }

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
            else
            {
                stringParams.Remove("identifications");
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
                stringParams["quality_grade"] = Enums.QualityToString[quality];
            }
            else
            {
                stringParams.Remove("quality_grade");
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
            stringParams["order_by"] = Enums.OrderByToString[orderBy];
        }
    }
}