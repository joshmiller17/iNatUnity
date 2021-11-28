using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace JoshAaronMiller.INaturalist
{

    /// <summary>
    /// The INatManager handles direct requests to the iNaturalist API.
    /// </summary>
    public class INatManager : MonoBehaviour
    {
        public static string BaseUrl = "https://api.inaturalist.org/v1/";

        //TEST
        void Start()
        {
            ObservationSearch os = new ObservationSearch();
            os.SetOrder(ObservationSearch.OrderBy.SpeciesGuess, ObservationSearch.SortOrder.Asc);
            os.SetQualityGrade(ObservationSearch.QualityGrade.NeedsId);
            os.SetIconicTaxa(new List<ObservationSearch.IconicTaxon>() { ObservationSearch.IconicTaxon.Reptilia });
            os.SetObservedOnDateLimits("2021-10-1", "2021-11-1");
            os.SetBooleanParameter(ObservationSearch.BooleanParameter.HasPhotos, true);
            os.SetBooleanParameter(ObservationSearch.BooleanParameter.IsPopular, true);
            List<Observation> results = SearchObservations(os);
            foreach (Observation r in results)
            {
                foreach (string url in r.GetPhotoUrls(Observation.ImageSize.Large))
                {
                    Debug.Log("url is " + url);
                }
            }
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
        /// <returns>A list of Observation JSON objects</returns>
        public List<Observation> GetObservationsById(List<int> ids)
        {
            string idsAsStringList = string.Join(",", ids);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(BaseUrl + "observations/" + idsAsStringList);
            return ObsWebResult.CreateFromJson(DoWebRequest(request)).results;
        }

        /// <summary>
        /// Given an ID, returns the corresponding observation
        /// </summary>
        /// <param name="id">The list of observation IDs to fetch</param>
        /// <returns>The Observation JSON object</returns>
        public Observation GetObservationById(int id)
        {
            return GetObservationsById(new List<int>() { id })[0];
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

        /// <summary>
        /// Given an ObservationSearch object, returns a list of matching observations
        /// </summary>
        /// <param name="obsSearch">An ObservationSearch object holding the parameters of the search</param>
        /// <returns></returns>
        public List<Observation> SearchObservations(ObservationSearch obsSearch)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(BaseUrl + "observations/?" + obsSearch.ToUrlParameters());
            
            //TEST BLOCK
            string test = DoWebRequest(request); //TEST
            string destination = Application.persistentDataPath + "/log.txt";
            File.WriteAllText(destination, test);

            return ObsWebResult.CreateFromJson(test).results;
        }

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
}