using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An ObservationSearch is a set of parameters for searching iNaturalist Observations.
/// Usage: myINatManager.SearchObservations(myObservationSearch)
/// </summary>
public class ObservationSearch
{
    public enum BooleanParameter { HasPositionalAccuracy, IsCaptive, IsEndemic, IsGeoreferenced, 
        IsIdentified, WasIntroduced, IsMappable, IsNative, IsOutOfRange, HasProjectId, HasPhotos,
        IsPopular, HasSounds, IsTaxonActive, IsThreatened, IsVerifiable, IsLicensed, IsPhotoLicensed};
    
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
        { BooleanParameter.IsPhotoLicensed, "photo_licensed" }
    };

    Dictionary<string, bool> boolParams = new Dictionary<string, bool>();

    public void SetBooleanParameter(BooleanParameter param, bool setting)
    {
        boolParams[BoolParamToString[param]] = setting;
    }


    /// <summary>
    /// Returns this ObservationSearch as a string of URL parameters.
    /// For use with INatManager::SearchObservations()
    /// </summary>
    /// <returns>A string of URL parameters</returns>
    public string ToUrlParameters()
    {
        List<string> keyValuePairs = new List<string>();

        // syntax: key = value, separated by &
        foreach(KeyValuePair<string, bool> entry in boolParams)
        {
            keyValuePairs.Add(entry.Key + "=" + entry.Value.ToString().ToLower());
        }

        return string.Join("&", keyValuePairs);
    }
}
