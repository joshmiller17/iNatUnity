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

        static List<Observation> JsonToObservations(string jsonString) => Results<Observation>.CreateFromJson<Results<Observation>>(jsonString).results;
        static Observation JsonToObservation(string jsonString) => Results<Observation>.CreateFromJson<Results<Observation>>(jsonString).results[0];
        static Error JsonToError(string errorString) => Error.CreateFromJson<Error>(errorString);
        static User JsonToUser(string userString) => Results<User>.CreateFromJson<Results<User>>(userString).results[0];

        static Identification JsonToIdentification(string jsonString) => Identification.CreateFromJson<Identification>(jsonString);

        static readonly string UserAgent = "iNat+Unity by Josh Aaron Miller";

        static readonly float ServerSleepTime = 3; //be nice to server
        static float timeSinceLastServerCall = float.MaxValue;

        string apiToken = "";


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
        /// <param name="errorCallback">A function to callback when iNaturalist returns an error message.</param>
        /// <param name="authenticate">Whether to pass the API token along with the request, for API calls that require authentication only.</param>
        IEnumerator DoWebRequestAsync<T>(UnityWebRequest request, Func<string, T> receiveRequest, Action<T> callback, Action<Error> errorCallback, bool authenticate=false)
        {
            if (IsRateLimited())
            {
                Debug.Log("Too many requests, waiting");
                yield return new WaitForSeconds(1);
            }
            timeSinceLastServerCall = 0;
            request.SetRequestHeader("User-Agent", UserAgent);
            Debug.Log("Sending web request: " + request.url.ToString());
            if (authenticate)
            {
                Debug.Log("Authorizing request");
                request.SetRequestHeader("Authorization", apiToken);
            }
            yield return request.SendWebRequest();
            while (!request.isDone)
                yield return null;


            byte[] result = request.downloadHandler.data;
            string json = System.Text.Encoding.Default.GetString(result);

            //Extra logging for debug
            string destination = Application.persistentDataPath + "/log.txt";
            File.WriteAllText(destination, json);

            if (!request.isHttpError && !request.isNetworkError)
            {
                // check if it's an error by interpreting it as an error and seeing if we're wrong
                Error error = JsonToError(json);
                if (error.status == (int)HttpStatusCode.OK)
                {
                    T response = receiveRequest(json);
                    callback(response);
                }
                else
                {
                    Debug.LogWarning("Web request failed with status code " + request.responseCode.ToString());
                    if (error.status == (int)HttpStatusCode.Unauthorized)
                    {
                        apiToken = ""; //invalidate any API token we have on record, this one didn't work.
                    }
                    errorCallback(error);
                }
            }
            else
            {
                Debug.LogError("Web request failed with status code " + request.responseCode.ToString());
                Debug.LogError(request.error);
                Error error = JsonToError(json);
                errorCallback(error);
            }
        }

        // --- LOGIN ---
        public void GetApiToken()
        {
            Application.OpenURL("https://www.inaturalist.org/users/api_token");
        }

        public void SetApiToken(string token)
        {
            apiToken = token;
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

        //DeleteIdentification
        //GetIdentification(id)
        //UpdateIdentification
        //SearchIdentifications

        /// <summary>
        /// Submit an Identification.
        /// </summary>
        /// <param name="identSub">The parameters of the Identification. Requires at minimum observation ID and taxon ID.</param>
        /// /// <param name="callback">A function to callback when the request is done which takes as input the Identification created.</param>
        /// <param name="errorCallback">A function to callback when iNaturalist returns an error message.</param>
        public void CreateIdentification(IdentificationSubmission identSub, Action<Identification> callback, Action<Error> errorCallback)
        {
            string postData = IdentificationSubmission.ToJson<IdentificationSubmission>(identSub);
            UnityWebRequest request = UnityWebRequest.Post(BaseUrl + "identifications/", postData);
            StartCoroutine(DoWebRequestAsync(request, JsonToIdentification, callback, errorCallback));
        }


        //GetIdentificationCategories
        //GetIdentificationSpeciesCounts
        //GetIdentificationIdentifiers
        //GetIdentificationObservers
        //GetIdentificationRecentTaxa
        //GetIdentificationSimilarSpecies

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
        /// <param name="errorCallback">A function to callback when iNaturalist returns an error message.</param>
        public void GetObservationsById(List<int> ids, Action<List<Observation>> callback, Action<Error> errorCallback)
        {
            string idsAsStringList = string.Join(",", ids);
            UnityWebRequest request = UnityWebRequest.Get(BaseUrl + "observations/" + idsAsStringList);
            StartCoroutine(DoWebRequestAsync(request, JsonToObservations, callback, errorCallback));
        }


        /// <summary>
        /// Given an ID, returns corresponding observations
        /// </summary>
        /// <param name="id">The observation ID to fetch</param>
        /// <param name="callback">A function to callback when the request is done which takes as input the Observation object found.</param>
        /// <param name="errorCallback">A function to callback when iNaturalist returns an error message.</param>
        public void GetObservationById(List<int> id, Action<Observation> callback, Action<Error> errorCallback)
        {
            UnityWebRequest request = UnityWebRequest.Get(BaseUrl + "observations/" + id.ToString());
            StartCoroutine(DoWebRequestAsync(request, JsonToObservation, callback, errorCallback));
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
        /// <param name="errorCallback">A function to callback when iNaturalist returns an error message.</param>
        public void SearchObservations(ObservationSearch obsSearch, Action<List<Observation>> callback, Action<Error> errorCallback)
        {
            UnityWebRequest request = UnityWebRequest.Get(BaseUrl + "observations/?" + obsSearch.ToUrlParameters());
            StartCoroutine(DoWebRequestAsync(request, JsonToObservations, callback, errorCallback));
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

        //GetUserDetails(id)
        //UpdateUser(id)
        //GetUserProjects
        //GetAutocompleteUser



        /// <summary>
        /// Fetch the User details for the authenticated user.
        /// </summary>
        /// <param name="callback">A function to callback when the request is done which takes as input the User object.</param>
        /// <param name="errorCallback">A function to callback when iNaturalist returns an error message.</param>
        /// <returns>Whether the INatManager has an API token to use for the authentication-required request.</returns>
        public bool GetUserMe(Action<User> callback, Action<Error> errorCallback)
        {
            if (apiToken == "")
            {
                return false;
            }
            UnityWebRequest request = UnityWebRequest.Get(BaseUrl + "users/me");
            StartCoroutine(DoWebRequestAsync(request, JsonToUser, callback, errorCallback, true));
            return true;
        }


        //UnmuteUser
        //MuteUser
        //UpdateUserSession






        // --- OBSERVATION TILES ---


        // --- POLYGON TILES ---


        // --- UTFGRID ---


        // --- PHOTOS ---



    }
}