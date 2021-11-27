using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class INatManager : MonoBehaviour
{
    public static string BaseUrl = "https://api.inaturalist.org/v1/";

    //TEST
    void Start()
    {
        Debug.Log(GetObservationById(1000));
    }

    /// <summary>
    /// Performs a web request.
    /// </summary>
    /// <param name="request">The HttpWebRequest</param>
    /// <returns>The JSON response in string format. To extract, use JsonUtility.FromJson<DataType>(jsonString)</returns>
    public string DoWebRequest(HttpWebRequest request)
    {
        Debug.Log("[INatManager] Web request: " + request.RequestUri.ToString());
        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        StreamReader reader = new StreamReader(response.GetResponseStream());
        return reader.ReadToEnd();
    }

    /// <summary>
    /// Perform an asynchronous web request.
    /// </summary>
    /// <param name="request">The UnityWebRequest</param>
    /// <param name="callback">A function to callback when the request is done which takes as input the JSON response as a string.</param>
    public IEnumerator DoWebRequestAsync(UnityWebRequest request, Action<string> callback)
    {
        yield return request.SendWebRequest();
        while (!request.isDone)
            yield return null;
        byte[] result = request.downloadHandler.data;
        string json = System.Text.Encoding.Default.GetString(result);
        callback(json);
    }

    // --- HELPER FUNCTIONS ---

    /// <summary>
    /// A helper function for constructing web requests: turns a list of ints into a comma-separated string
    /// </summary>
    /// <param name="list">List of ints</param>
    /// <returns>The list as a string with commas separating the values</returns>
    string IntListToUrlParams(List<int> list)
    {
        if (list.Count == 0)
            throw new ArgumentException("List cannot be empty", nameof(list));

        string ret = list[0].ToString();
        int index = 1;
        while (index < list.Count)
        {
            ret += "," + list[index];
            index += 1;
        }
        return ret;
    }

    // --- ANNOTATIONS ---

    //CreateAnnotation
    //DeleteAnnotation
    //VoteAnnotation
    //UnvoteAnnotation

    // --- COMMENTS ---


    // --- CONTROLLED TERMS ---


    // --- FLAGS ---


    // --- IDENTIFICATIONS ---

    // --- MESSAGES ---

    // --- OBSERVATION FIELD VALUES ---


    // --- OBSERVATION PHOTOS ---


    // --- OBSERVATIONS ---

    //DeleteObservation

    /// <summary>
    /// Given an array of IDs, returns corresponding observations 
    /// </summary>
    /// <param name="id">The list of observation IDs to fetch</param>
    /// <returns>Returns a JSON string object with metadata and an array of observations</returns>
    public string GetObservationsById(List<int> ids)
    {
        string idsAsStringList = IntListToUrlParams(ids);
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(BaseUrl + "observations/" + idsAsStringList);
        return DoWebRequest(request);
    }

    /// <summary>
    /// Given an array of IDs, returns corresponding observations 
    /// </summary>
    /// <param name="id">The list of observation IDs to fetch</param>
    /// <returns>Returns a JSON string object with metadata and an array of observations</returns>
    public string GetObservationById(int id)
    {
        return GetObservationsById(new List<int>() { id });
    }


    //UpdateObservation
    //FaveObservation
    //UnfaveObservation
    //ReviewObservation
    //UnreviewObservation
    //GetObservationSubscriptions
    //DeleteQualityMetric
    //SetQualityMetric
    //GetObservationTaxonSummary
    //SubscribeToObservation
    //VoteObservation
    //UnvoteObservation
    //SearchObservations
    //CreateObservation
    //GetDeletedObservations
    //GetObservationHistogram
    //GetObservationIdentifiers
    //GetObservationObservers
    //GetObservationPopularFieldValues
    //GetObservationSpeciesCounts
    //GetObservationUserUpdates
    //MarkObservationUpdatesAsViewed


    // --- PLACES ---


    // --- PROJECT OBSERVATIONS ---


    // --- PROJECTS ---


    // --- SEARCH ---


    // --- TAXA ---


    // --- USERS ---


    // --- OBSERVATION TILES ---


    // --- POLYGON TILES ---


    // --- UTFGRID ---


    // --- PHOTOS ---



}
