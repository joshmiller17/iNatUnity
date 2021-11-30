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
        public static readonly string BaseUrl = "https://api.inaturalist.org/v1/";
        public static readonly string ApiTokenUrl = "https://www.inaturalist.org/users/api_token";

        static List<Observation> JsonToObservations(string jsonString) => ObsWebResult.CreateFromJson(jsonString).results;
        static Observation JsonToObservation(string jsonString) => ObsWebResult.CreateFromJson(jsonString).results[0];

        static readonly float ServerSleepTime = 3; //be nice to server
        static float timeSinceLastServerCall = float.MaxValue;


        private void Update()
        {
            if (timeSinceLastServerCall < ServerSleepTime)
            {
                timeSinceLastServerCall += Time.unscaledDeltaTime;
            }
        }

        /// <summary>
        /// Returns whether the INatManager is currently rate limiting itself to be kind to the server.
        /// </summary>
        /// <returns>Whether the INatManager is rate limited. Only make server calls when not rate limited.</returns>
        bool IsRateLimited()
        {
            return timeSinceLastServerCall < ServerSleepTime;
        }


        /// <summary>
        /// Perform an asynchronous web request.
        /// </summary>
        /// <param name="request">The UnityWebRequest</param>
        /// <param name="receiveRequest">The function which processes the server response (JSON encoded as string) and returns a response of type T.</param>
        /// <param name="callback">A function to callback when the request is done which takes as input the processed response of type T.</param>
        IEnumerator DoWebRequestAsync<T>(UnityWebRequest request, Func<string, T> receiveRequest, Action<T> callback)
        {
            if (IsRateLimited())
            {
                yield return null;
            }
            timeSinceLastServerCall = 0;
            Debug.Log("Sending web request: " + request.url.ToString());
            yield return request.SendWebRequest();
            while (!request.isDone)
                yield return null;

            if (!request.isHttpError && !request.isNetworkError)
            {
                byte[] result = request.downloadHandler.data;
                string json = System.Text.Encoding.Default.GetString(result);

                //DEBUG PRINT TODO REMOVE
                string destination = Application.persistentDataPath + "/log.txt";
                File.WriteAllText(destination, json);

                T response = receiveRequest(json);
                callback(response);
            }
            else
            {
                Debug.LogError("Web request failed with status code " + request.responseCode.ToString());
                Debug.LogError(request.error);
            }
        }

        // --- LOGIN ---
        public void GetApiToken()
        {
            
        }

        public void TestPrint(string json)
        {
            Debug.Log(json);
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
        /// <param name="ids">The list of observation IDs to fetch</param>
        /// <param name="callback">A function to callback when the request is done which takes as input the list of Observation objects found.</param>
        public void GetObservationsById(List<int> ids, Action<List<Observation>> callback)
        {
            string idsAsStringList = string.Join(",", ids);
            UnityWebRequest request = UnityWebRequest.Get(BaseUrl + "observations/" + idsAsStringList);
            StartCoroutine(DoWebRequestAsync(request, JsonToObservations, callback));
        }


        /// <summary>
        /// Given an ID, returns corresponding observations
        /// </summary>
        /// <param name="id">The observation ID to fetch</param>
        /// <param name="callback">A function to callback when the request is done which takes as input the Observation object found.</param>
        public void GetObservationById(List<int> id, Action<Observation> callback)
        {
            UnityWebRequest request = UnityWebRequest.Get(BaseUrl + "observations/" + id.ToString());
            StartCoroutine(DoWebRequestAsync(request, JsonToObservation, callback));
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
        /// <param name="callback">A function to callback when the request is done which takes as input the list of Observation objects found.</param>
        public void SearchObservations(ObservationSearch obsSearch, Action<List<Observation>> callback)
        {
            UnityWebRequest request = UnityWebRequest.Get(BaseUrl + "observations/?" + obsSearch.ToUrlParameters());
            StartCoroutine(DoWebRequestAsync(request, JsonToObservations, callback));
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