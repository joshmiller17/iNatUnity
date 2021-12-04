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

        static T FromJson<T>(string jsonString) => JsonObject<T>.CreateFromJson(jsonString);
        static List<T> ResultsFromJson<T>(string jsonString) => JsonObject<Results<T>>.CreateFromJson(jsonString).results;
        static T FirstResultFromJson<T>(string jsonString) => ResultsFromJson<T>(jsonString)[0];

        static T NoOp<T>(T x) => x;

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
        /// Helper function to send the web request.
        /// </summary>
        /// <param name="request">The web request.</param>
        /// <param name="authenticate">Whether to attach authentication header and API token.</param>
        /// <returns></returns>
        IEnumerator _SendWebRequestAsync(UnityWebRequest request, bool authenticate)
        {
            if (IsRateLimited())
            {
                Debug.Log("Too many requests, waiting");
                yield return new WaitForSeconds(1);
            }
            timeSinceLastServerCall = 0;
            request.SetRequestHeader("User-Agent", UserAgent);
            Debug.Log("Sending web request: " + request.url.ToString());
            if (authenticate || request.method != "GET") // everything that isn't a GET needs authentication
            {
                Debug.Log("Authorizing request");
                request.SetRequestHeader("Authorization", apiToken);
            }
            yield return request.SendWebRequest();
        }

        /// <summary>
        /// Download and interpret the server response as a JSON string.
        /// </summary>
        /// <param name="request">The web request.</param>
        /// <returns>The response as a JSON string.</returns>
        string _HandleWebResponse(UnityWebRequest request)
        {
            byte[] result = request.downloadHandler.data;
            string json = System.Text.Encoding.Default.GetString(result);

            //Extra logging for debug
            string destination = Application.persistentDataPath + "/log.txt";
            File.WriteAllText(destination, json);

            return json;
        }

        /// <summary>
        /// If the server response was an error, handle it and return true. Otherwise return false.
        /// </summary>
        /// <param name="request">The web request.</param>
        /// <param name="json">The response as a JSON string.</param>
        /// <returns>If error, return it; otherwise return null.</returns>
        Error _HandleWebError(UnityWebRequest request, string json)
        {
            // check if it's an error by interpreting it as an error and seeing if we're wrong
            Error error = FromJson<Error>(json);

            if (!request.isHttpError
                && !request.isNetworkError
                && error.status == (int)HttpStatusCode.OK)
            {
                return null;
            }
            else
            {
                Debug.LogError("Web request failed with status code " + request.responseCode.ToString());
                Debug.LogError(request.error);
                if (error.status == (int)HttpStatusCode.Unauthorized)
                {
                    apiToken = ""; //invalidate any API token we have on record, this one didn't work.
                }
                return error;
            }
        }


        /// <summary>
        /// Perform an asynchronous web request.
        /// </summary>
        /// <param name="request">The UnityWebRequest</param>
        /// <param name="callback">A function to callback when the request is done.</param>
        /// <param name="errorCallback">A function to callback when iNaturalist returns an error message.</param>
        /// <param name="authenticate">Whether to pass the API token along with the request, for API calls that require authentication only. This is forced to true for all non-GET calls.</param>
        IEnumerator DoWebRequestAsync(UnityWebRequest request, Action callback, Action<Error> errorCallback, bool authenticate = false)
        {
            _SendWebRequestAsync(request, authenticate);

            while (!request.isDone)
                yield return null;

            string json = _HandleWebResponse(request);
            Error error = _HandleWebError(request, json);

            if (error == null)
            {
                callback();
            }
            else
            {
                errorCallback(error);
            }
        }


        /// <summary>
        /// Perform an asynchronous web request.
        /// </summary>
        /// <param name="request">The UnityWebRequest</param>
        /// <param name="receiveRequest">The function which processes the server response (JSON encoded as string) and returns a response of type T.</param>
        /// <param name="callback">A function to callback when the request is done which takes as input the processed response of type T.</param>
        /// <param name="errorCallback">A function to callback when iNaturalist returns an error message.</param>
        /// <param name="authenticate">Whether to pass the API token along with the request, for API calls that require authentication only. This is forced to true for all non-GET calls.</param>
        IEnumerator DoWebRequestAsync<T>(UnityWebRequest request, Func<string, T> receiveRequest, Action<T> callback, Action<Error> errorCallback, bool authenticate=false)
        {
            _SendWebRequestAsync(request, authenticate);

            while (!request.isDone)
                yield return null;

            string json = _HandleWebResponse(request);
            Error error = _HandleWebError(request, json);

            if (error == null)
            {
                T response = receiveRequest(json);
                callback(response);
            }
            else
            {
                errorCallback(error);
            }
        }

        /// <summary>
        /// Construct and return a PUT request.
        /// </summary>
        /// <param name="url">The URL to send a PUT to.</param>
        /// <param name="body">The string data to attach to the PUT.</param>
        /// <returns>The UnityWebRequest.</returns>
        UnityWebRequest MakePutRequest(string url, string body)
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(body);
            UnityWebRequest request = UnityWebRequest.Put(url, bodyRaw);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.uploadHandler.contentType = "application/json";
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Accept", "application/json");
            return request;
        }

        /// <summary>
        /// Construct and return a PUT request.
        /// </summary>
        /// <param name="url">The URL to send a PUT to.</param>
        /// <param name="body">The WWWForm data to attach to the PUT.</param>
        /// <returns>The UnityWebRequest.</returns>
        UnityWebRequest MakePutRequest(string url, WWWForm body)
        {
            UnityWebRequest request = UnityWebRequest.Post(url, body);
            request.method = "PUT";
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Accept", "application/json");
            return request;
        }

        /// <summary>
        /// Construct and return a POST request.
        /// </summary>
        /// <param name="url">The URL to send a POST to.</param>
        /// <param name="body">The string data to attach to the POST.</param>
        /// <returns>The UnityWebRequest.</returns>
        UnityWebRequest MakePostRequest(string url, string body)
        {
            UnityWebRequest request = MakePutRequest(url, body);
            request.method = "POST";
            return request;
        }

        /// <summary>
        /// Construct and return a POST request.
        /// </summary>
        /// <param name="url">The URL to send a POST to.</param>
        /// <param name="body">The WWWForm data to attach to the POST.</param>
        /// <returns>The UnityWebRequest.</returns>
        UnityWebRequest MakePostRequest(string url, WWWForm body)
        {
            UnityWebRequest request = MakePutRequest(url, body);
            request.method = "POST";
            return request;
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

        // --- ANNOTATIONS ---

        //CreateAnnotation not yet implemented
        //DeleteAnnotation not yet implemented
        //VoteAnnotation not yet implemented
        //UnvoteAnnotation not yet implemented

        // --- COMMENTS ---

        //CreateComment not yet implemented
        //DeleteComment not yet implemented
        //UpdateComment not yet implemented


        // --- CONTROLLED TERMS ---

        /// <summary>
        /// Fetch a list of all attribute controlled terms as a List of ControlledTerms.
        /// </summary>
        /// <param name="callback">A function to callback when the request is done which takes as input the List of Controlled Terms created.</param>
        /// <param name="errorCallback">A function to callback when iNaturalist returns an error message.</param>
        public void GetTermsIndex(Action<List<ControlledTerm>> callback, Action<Error> errorCallback)
        {
            UnityWebRequest request = UnityWebRequest.Get(BaseUrl + "controlled_terms/");
            StartCoroutine(DoWebRequestAsync(request, ResultsFromJson<ControlledTerm>, callback, errorCallback));
        }

        /// <summary>
        /// Fetch a list of all attribute controlled terms relevant to a taxon as a List of ControlledTerms.
        /// </summary>
        /// <param name="taxonId">The ID of the Taxon.</param>
        /// <param name="callback">A function to callback when the request is done which takes as input the List of Controlled Terms created.</param>
        /// <param name="errorCallback">A function to callback when iNaturalist returns an error message.</param>
        public void GetTermsForTaxon(int taxonId, Action<List<ControlledTerm>> callback, Action<Error> errorCallback)
        {
            UnityWebRequest request = UnityWebRequest.Get(BaseUrl + "controlled_terms/for_taxon?taxon_id=" + taxonId.ToString());
            StartCoroutine(DoWebRequestAsync(request, ResultsFromJson<ControlledTerm>, callback, errorCallback));
        }

        // --- FLAGS ---

        /// <summary>
        /// Submit a Flag.
        /// </summary>
        /// <param name="flag">The parameters of the flag.</param>
        /// <param name="callback">A function to callback when the request is done which takes as input the Flag created.</param>
        /// <param name="errorCallback">A function to callback when iNaturalist returns an error message.</param>
        public void CreateFlag(WrappedFlag flag, Action<Identification> callback, Action<Error> errorCallback)
        {
            string postData = WrappedFlag.ToJson(flag);
            UnityWebRequest request = MakePostRequest(BaseUrl + "flags", postData);
            StartCoroutine(DoWebRequestAsync(request, FromJson<Identification>, callback, errorCallback));
        }

        /// <summary>
        /// Delete a Flag.
        /// </summary>
        /// <param name="flagId">The ID of the flag.</param>
        /// <param name="callback">A function to callback when the request is done which takes as input the server response (typically empty).</param>
        /// <param name="errorCallback">A function to callback when iNaturalist returns an error message.</param>
        public void DeleteFlag(int flagId, Action callback, Action<Error> errorCallback)
        {
            UnityWebRequest request = UnityWebRequest.Delete(BaseUrl + "flags/" + flagId.ToString());
            StartCoroutine(DoWebRequestAsync(request, callback, errorCallback));
        }

        /// <summary>
        /// Update a Flag.
        /// </summary>
        /// <param name="flagId">The ID of the flag.</param>
        /// <param name="flag">The parameters of the flag.</param>
        /// <param name="callback">A function to callback when the request is done which takes as input the Flag updated.</param>
        /// <param name="errorCallback">A function to callback when iNaturalist returns an error message.</param>
        public void UpdateFlag(int flagId, WrappedFlag flag, Action<Identification> callback, Action<Error> errorCallback)
        {
            string postData = WrappedFlag.ToJson(flag);
            UnityWebRequest request = MakePutRequest(BaseUrl + "flags/" + flagId.ToString(), postData);
            StartCoroutine(DoWebRequestAsync(request, FromJson<Identification>, callback, errorCallback));
        }


        // --- IDENTIFICATIONS ---

        /// <summary>
        /// Delete an Identification.
        /// </summary>
        /// <param name="flagId">The ID of the Identification.</param>
        /// <param name="callback">A function to callback when the request is done which takes as input the server response (typically empty).</param>
        /// <param name="errorCallback">A function to callback when iNaturalist returns an error message.</param>
        public void DeleteIdentification(int identId, Action<string> callback, Action<Error> errorCallback)
        {
            UnityWebRequest request = UnityWebRequest.Delete(BaseUrl + "identifications/" + identId.ToString());
            StartCoroutine(DoWebRequestAsync(request, NoOp, callback, errorCallback));
        }

        /// <summary>
        /// Given an array of IDs, returns corresponding Identifications 
        /// </summary>
        /// <param name="identIds">The list of identification IDs to fetch</param>
        /// <param name="callback">A function to callback when the request is done which takes as input the list of Identification objects found.</param>
        /// <param name="errorCallback">A function to callback when iNaturalist returns an error message.</param>
        public void GetIdentifications(List<int> identIds, Action<List<Identification>> callback, Action<Error> errorCallback)
        {
            string idsAsStringList = string.Join(",", identIds);
            UnityWebRequest request = UnityWebRequest.Get(BaseUrl + "identifications/" + idsAsStringList);
            StartCoroutine(DoWebRequestAsync(request, ResultsFromJson<Identification>, callback, errorCallback));
        }

        /// <summary>
        /// Given an ID, returns corresponding Identifications 
        /// </summary>
        /// <param name="identId">The identification ID to fetch</param>
        /// <param name="callback">A function to callback when the request is done which takes as input the Identification object found.</param>
        /// <param name="errorCallback">A function to callback when iNaturalist returns an error message.</param>
        public void GetIdentification(int identId, Action<Identification> callback, Action<Error> errorCallback)
        {
            UnityWebRequest request = UnityWebRequest.Get(BaseUrl + "identifications/" + identId.ToString());
            StartCoroutine(DoWebRequestAsync(request, FirstResultFromJson<Identification>, callback, errorCallback));
        }


        /// <summary>
        /// Update an Identification
        /// </summary>
        /// <param name="identId">The identification ID to update</param>
        /// <param name="identSub">The updated information for the identification.</param>
        /// <param name="callback">A function to callback when the request is done which takes as input the Identification object returned.</param>
        /// <param name="errorCallback">A function to callback when iNaturalist returns an error message.</param>
        public void UpdateIdentification(int identId, IdentificationSubmission identSub, Action<Identification> callback, Action<Error> errorCallback)
        {
            WrappedIdentificationSubmission submission = new WrappedIdentificationSubmission();
            submission.identification = identSub;
            string postData = WrappedIdentificationSubmission.ToJson(submission);
            UnityWebRequest request = MakePutRequest(BaseUrl + "identifications/" + identId.ToString(), postData);
            StartCoroutine(DoWebRequestAsync(request, FromJson<Identification>, callback, errorCallback));
        }


        /// <summary>
        /// Given an IdentificationSearch object, returns a list of matching Identifications
        /// </summary>
        /// <param name="identSearch">An IdentificationSearch object holding the parameters of the search</param>
        /// <param name="callback">A function to callback when the request is done which takes as input the list of Identification objects found.</param>
        /// <param name="errorCallback">A function to callback when iNaturalist returns an error message.</param>
        public void SearchIdentifications(IdentificationSearch identSearch, Action<List<Identification>> callback, Action<Error> errorCallback)
        {
            UnityWebRequest request = UnityWebRequest.Get(BaseUrl + "identifications?" + identSearch.ToUrlParameters());
            StartCoroutine(DoWebRequestAsync(request, ResultsFromJson<Identification>, callback, errorCallback));
        }



        /// <summary>
        /// Submit an Identification.
        /// </summary>
        /// <param name="identSub">The parameters of the Identification. Requires at minimum observation ID and taxon ID.</param>
        /// <param name="callback">A function to callback when the request is done which takes as input the Identification created.</param>
        /// <param name="errorCallback">A function to callback when iNaturalist returns an error message.</param>
        public void CreateIdentification(IdentificationSubmission identSub, Action<Identification> callback, Action<Error> errorCallback)
        {
            WrappedIdentificationSubmission submission = new WrappedIdentificationSubmission();
            submission.identification = identSub;
            string postData = WrappedIdentificationSubmission.ToJson(submission);
            UnityWebRequest request = MakePostRequest(BaseUrl + "identifications", postData);
            StartCoroutine(DoWebRequestAsync(request, FromJson<Identification>, callback, errorCallback));
        }

        /// <summary>
        /// Given an IdentificationSearch object, returns the counts of how many identifications matching the search have a particular category.
        /// </summary>
        /// <param name="identSearch">An IdentificationSearch object holding the parameters of the search.</param>
        /// <param name="callback">A function to callback when the request is done which takes as input the list of IdentificationCategoryCount objects found.</param>
        /// <param name="errorCallback">A function to callback when iNaturalist returns an error message.</param>
        public void GetIdentificationCategories(IdentificationSearch identSearch, Action<List<IdentificationCategoryCount>> callback, Action<Error> errorCallback)
        {
            UnityWebRequest request = UnityWebRequest.Get(BaseUrl + "identifications/categories?" + identSearch.ToUrlParameters());
            StartCoroutine(DoWebRequestAsync(request, ResultsFromJson<IdentificationCategoryCount>, callback, errorCallback));
        }

        /// <summary>
        /// Given an IdentificationSearch object, returns the counts of how many identifications matching the search have a particular leaf taxon.
        /// </summary>
        /// <param name="identSearch">An IdentificationSearch object holding the parameters of the search.</param>
        /// <param name="callback">A function to callback when the request is done which takes as input the list of IdentificationSpeciesCount objects found.</param>
        /// <param name="errorCallback">A function to callback when iNaturalist returns an error message.</param>
        public void GetIdentificationSpeciesCounts(IdentificationSearch identSearch, Action<List<IdentificationSpeciesCount>> callback, Action<Error> errorCallback)
        {
            UnityWebRequest request = UnityWebRequest.Get(BaseUrl + "identifications/species_counts?" + identSearch.ToUrlParameters());
            StartCoroutine(DoWebRequestAsync(request, ResultsFromJson<IdentificationSpeciesCount>, callback, errorCallback));
        }


        //GetIdentificationIdentifiers not yet implemented
        //GetIdentificationObservers not yet implemented
        //GetIdentificationRecentTaxa not yet implemented

        /// <summary>
        /// Given a Taxon ID, return similar taxa and counts of co-occurrence.
        /// </summary>
        /// <remarks>
        /// The definition of "similar taxa" is operationalized by finding all observations of this taxon or identified as this taxon,
        /// then taking the identifications of those observations and counting frequencies of identifications of other taxa.
        /// In short, this returns a list mapping taxa to how many times they co-occurred with the searched taxon.
        /// </remarks>
        /// <param name="taxonId">The ID of the taxon to find similar taxa to.</param>
        /// <param name="callback">A function to callback when the request is done which takes as input the list of IdentificationSpeciesCount objects representing the results.</param>
        /// <param name="errorCallback">A function to callback when iNaturalist returns an error message.</param>
        public void GetSimilarSpecies(int taxonId, Action<List<IdentificationSpeciesCount>> callback, Action<Error> errorCallback)
        {
            UnityWebRequest request = UnityWebRequest.Get(BaseUrl + "identifications/similar_species?taxon_id=" + taxonId.ToString());
            StartCoroutine(DoWebRequestAsync(request, ResultsFromJson<IdentificationSpeciesCount>, callback, errorCallback));
        }

        /// <summary>
        /// Given a Taxon ID, return similar taxa and counts of co-occurrence.
        /// </summary>
        /// <remarks>
        /// The definition of "similar taxa" is operationalized by finding all observations of this taxon or identified as this taxon,
        /// then taking the identifications of those observations and counting frequencies of identifications of other taxa.
        /// In short, this returns a list mapping taxa to how many times they co-occurred with the searched taxon.
        /// </remarks>
        /// <param name="taxonId">The ID of the taxon to find similar taxa to.</param>
        /// /// <param name="obsSearch">Additional parameters to refine the search, limiting what observations can be included.</param>
        /// <param name="callback">A function to callback when the request is done which takes as input the list of IdentificationSpeciesCount objects representing the results.</param>
        /// <param name="errorCallback">A function to callback when iNaturalist returns an error message.</param>
        public void GetSimilarSpecies(int taxonId, ObservationSearch obsSearch, Action<List<IdentificationSpeciesCount>> callback, Action<Error> errorCallback)
        {
            UnityWebRequest request = UnityWebRequest.Get(BaseUrl + "identifications/similar_species?taxon_id=" + taxonId.ToString() + "&" + obsSearch.ToUrlParameters());
            StartCoroutine(DoWebRequestAsync(request, ResultsFromJson<IdentificationSpeciesCount>, callback, errorCallback));
        }


        // --- MESSAGES ---


        /// <summary>
        /// Given a MessageSearch object, returns a list of matching user messages
        /// </summary>
        /// <remarks>
        /// This function does not mark these messages as read. See GetUserMessageThread.
        /// </remarks>
        /// <param name="messageSearch">A MessageSearch object holding the parameters of the search</param>
        /// <param name="callback">A function to callback when the request is done which takes as input the list of Message objects found.</param>
        /// <param name="errorCallback">A function to callback when iNaturalist returns an error message.</param>
        public void SearchUserMessages(MessageSearch messageSearch, Action<List<UserMessage>> callback, Action<Error> errorCallback)
        {
            UnityWebRequest request = UnityWebRequest.Get(BaseUrl + "messages?" + messageSearch.ToUrlParameters());
            StartCoroutine(DoWebRequestAsync(request, ResultsFromJson<UserMessage>, callback, errorCallback, authenticate:true));
        }

        /// <summary>
        /// Send a private message.
        /// </summary>
        /// <param name="newMessage">The message to be sent.</param>
        /// <param name="callback">A function to callback when the request is done which takes as input the new Message created.</param>
        /// <param name="errorCallback">A function to callback when iNaturalist returns an error message.</param>
        public void CreateUserMessage(UserMessage newMessage, Action<UserMessage> callback, Action<Error> errorCallback)
        {
            string postData = UserMessage.ToJson(newMessage);
            UnityWebRequest request = MakePostRequest(BaseUrl + "messages", postData);
            StartCoroutine(DoWebRequestAsync(request, FromJson<UserMessage>, callback, errorCallback));
        }

        /// <summary>
        /// Delete all messages in a message thread.
        /// </summary>
        /// <param name="threadId">The thread to delete.</param>
        /// <param name="callback">A function to callback when the request is done.</param>
        /// <param name="errorCallback">A function to callback when iNaturalist returns an error message.</param>
        public void DeleteUserMessageThread(int threadId, Action callback, Action<Error> errorCallback)
        {
            UnityWebRequest request = UnityWebRequest.Delete(BaseUrl + "messages/" + threadId.ToString());
            StartCoroutine(DoWebRequestAsync(request, callback, errorCallback));
        }

        /// <summary>
        /// Retrieves all messages in the specified thread and marks them all as read.
        /// </summary>
        /// <param name="threadId">The thread to fetch.</param>
        /// <param name="callback">A function to callback when the request is done which takes as input the thread fetched as a list of UserMessages.</param>
        /// <param name="errorCallback">A function to callback when iNaturalist returns an error message.</param>
        public void GetUserMessageThread(int threadId, Action<List<UserMessage>> callback, Action<Error> errorCallback)
        {
            UnityWebRequest request = UnityWebRequest.Get(BaseUrl + "messages/" + threadId.ToString());
            StartCoroutine(DoWebRequestAsync(request, ResultsFromJson<UserMessage>, callback, errorCallback, authenticate: true));
        }


        /// <summary>
        /// Returns a Count of unread messages in the authenticated user's inbox.
        /// </summary>
        /// <param name="callback">A function to callback when the request is done which takes as input the Count of unread messages.</param>
        /// <param name="errorCallback">A function to callback when iNaturalist returns an error message.</param>
        public void GetCountUnreadMessages(Action<Count> callback, Action<Error> errorCallback)
        {
            UnityWebRequest request = UnityWebRequest.Get(BaseUrl + "messages/unread");
            StartCoroutine(DoWebRequestAsync(request, FromJson<Count>, callback, errorCallback, authenticate: true));
        }

        // --- OBSERVATION FIELD VALUES ---

        //DeleteObservationFieldValue not yet implemented
        //UpdateObservationFieldValue not yet implemented
        //CreateObservationFieldValue not yet implemented

        // --- OBSERVATION PHOTOS ---


        /// <summary>
        /// Delete an observation photo.
        /// </summary>
        /// <param name="threadId">The observation photo to delete.</param>
        /// <param name="callback">A function to callback when the request is done.</param>
        /// <param name="errorCallback">A function to callback when iNaturalist returns an error message.</param>
        public void DeleteObservationPhoto(int obsPhotoId, Action callback, Action<Error> errorCallback)
        {
            UnityWebRequest request = UnityWebRequest.Delete(BaseUrl + "observation_photos/" + obsPhotoId.ToString());
            StartCoroutine(DoWebRequestAsync(request, callback, errorCallback));
        }



        /// <summary>
        /// Update an Observation Photo.
        /// </summary>
        /// <param name="obsPhotoId">The Observation Photo ID to update</param>
        /// <param name="position">The position in which the photo is displayed for the observation.</param>
        /// <param name="photo">The Photo to upload.</param>
        /// <param name="callback">A function to callback when the request is done which takes as input the Observatoin Photo object returned.</param>
        /// <param name="errorCallback">A function to callback when iNaturalist returns an error message.</param>
        public void UpdateObservationPhoto(int obsPhotoId, int position, Photo photo, Action<Identification> callback, Action<Error> errorCallback)
        {
            WWWForm formData = new WWWForm();
            formData.AddField("observation_photo[position]", position);
            formData.AddBinaryData("file", photo.ToBytes(), photo.fileName, photo.fileType);

            UnityWebRequest request = MakePutRequest(BaseUrl + "observation_photos/" + obsPhotoId.ToString(), formData);
            StartCoroutine(DoWebRequestAsync(request, FromJson<Identification>, callback, errorCallback));
        }

        /// <summary>
        /// Create an Observation Photo.
        /// </summary>
        /// <param name="obsId">The Observation ID to add the photo to</param>
        /// <param name="obsUuid">The UUID of the observation.</param>
        /// <param name="photo">The Photo to upload.</param>
        /// <param name="callback">A function to callback when the request is done which takes as input the Observatoin Photo object returned.</param>
        /// <param name="errorCallback">A function to callback when iNaturalist returns an error message.</param>
        public void CreateObservationPhoto(int obsId, string obsUuid, Photo photo, Action<Identification> callback, Action<Error> errorCallback)
        {
            WWWForm formData = new WWWForm();
            formData.AddField("observation_photo[observation_id]", obsId);
            formData.AddField("observation_photo[uuid]", obsUuid);
            formData.AddBinaryData("file", photo.ToBytes(), photo.fileName, photo.fileType);

            UnityWebRequest request = MakePostRequest(BaseUrl + "observation_photos", formData);
            StartCoroutine(DoWebRequestAsync(request, FromJson<Identification>, callback, errorCallback));
        }


        // --- OBSERVATIONS ---

        //DeleteObservation not yet implemented TODO

        /// <summary>
        /// Given an array of IDs, returns corresponding observations 
        /// </summary>
        /// <param name="ids">The list of observation IDs to fetch</param>
        /// <param name="callback">A function to callback when the request is done which takes as input the list of Observation objects found.</param>
        /// <param name="errorCallback">A function to callback when iNaturalist returns an error message.</param>
        public void GetObservations(List<int> ids, Action<List<Observation>> callback, Action<Error> errorCallback)
        {
            string idsAsStringList = string.Join(",", ids);
            UnityWebRequest request = UnityWebRequest.Get(BaseUrl + "observations/" + idsAsStringList);
            StartCoroutine(DoWebRequestAsync(request, ResultsFromJson<Observation>, callback, errorCallback));
        }


        /// <summary>
        /// Given an ID, returns corresponding observations
        /// </summary>
        /// <param name="id">The observation ID to fetch</param>
        /// <param name="callback">A function to callback when the request is done which takes as input the Observation object found.</param>
        /// <param name="errorCallback">A function to callback when iNaturalist returns an error message.</param>
        public void GetObservation(int id, Action<Observation> callback, Action<Error> errorCallback)
        {
            UnityWebRequest request = UnityWebRequest.Get(BaseUrl + "observations/" + id.ToString());
            StartCoroutine(DoWebRequestAsync(request, FirstResultFromJson<Observation>, callback, errorCallback));
        }


        //UpdateObservation not yet implemented TODO
        //FaveObservation not yet implemented TODO
        //UnfaveObservation not yet implemented TODO
        //ReviewObservation not yet implemented TODO
        //UnreviewObservation not yet implemented TODO
        //GetObservationSubscriptions not yet implemented TODO
        //DeleteQualityMetric not yet implemented
        //SetQualityMetric not yet implemented
        //GetObservationTaxonSummary not yet implemented TODO
        //SubscribeToObservation not yet implemented TODO
        //VoteObservation not yet implemented TODO
        //UnvoteObservation not yet implemented TODO


        /// <summary>
        /// Given an ObservationSearch object, returns a list of matching observations
        /// </summary>
        /// <param name="obsSearch">An ObservationSearch object holding the parameters of the search</param>
        /// <param name="callback">A function to callback when the request is done which takes as input the list of Observation objects found.</param>
        /// <param name="errorCallback">A function to callback when iNaturalist returns an error message.</param>
        public void SearchObservations(ObservationSearch obsSearch, Action<List<Observation>> callback, Action<Error> errorCallback)
        {
            UnityWebRequest request = UnityWebRequest.Get(BaseUrl + "observations?" + obsSearch.ToUrlParameters());
            StartCoroutine(DoWebRequestAsync(request, ResultsFromJson<Observation>, callback, errorCallback));
        }

        //CreateObservation not yet implemented TODO
        //GetDeletedObservations not yet implemented
        //GetObservationHistogram not yet implemented TODO
        //GetObservationIdentifiers not yet implemented
        //GetObservationObservers not yet implemented
        //GetObservationPopularFieldValues not yet implemented
        //GetObservationSpeciesCounts not yet implemented TODO
        //GetObservationUserUpdates not yet implemented
        //MarkObservationUpdatesAsViewed not yet implemented


        // --- PLACES ---

        //GetPlaceDetails not yet implemented TODO
        //GetPlaceAutocomplete not yet implemented TODO
        //GetNearbyPlaces not yet implemented TODO

        // --- POSTS ---

        // SearchPosts not yet implemented
        // CreatePost not yet implemented
        // DeletePost not yet implemented
        // UpdatePost not yet implemented
        // GetPostsForUser not yet implemented

        // --- PROJECT OBSERVATIONS ---

        //DeleteProjectObservation not yet implemented
        //UpdateProjectObservation not yet implemented
        //CreateProjectObservation not yet implemented

        // --- PROJECTS ---

        //SearchProjects not yet implemented
        //GetProjectDetails not yet implemented
        //JoinProject not yet implemented
        //LeaveProject not yet implemented
        //GetProjectMembers not yet implemented
        //GetMyProjectMembership not yet implemented
        //AddProject not yet implemented
        //RemoveProject not yet implemented
        //GetProjectAutocomplete not yet implemented


        // --- SEARCH ---

        //SearchSite not yet implemented

        // --- TAXA ---

        //GetTaxonDetails not yet implemented TODO
        //SearchTaxa not yet implemented TODO
        //GetTaxonAutocomplete not yet implemented TODO

        // --- USERS ---

        //GetUserDetails(id) not yet implemented
        //UpdateUser(id) not yet implemented
        //GetUserProjects not yet implemented
        //GetAutocompleteUser not yet implemented



        /// <summary>
        /// Fetch the User details for the authenticated user.
        /// </summary>
        /// <param name="callback">A function to callback when the request is done which takes as input the User object.</param>
        /// <param name="errorCallback">A function to callback when iNaturalist returns an error message.</param>
        public void GetUserMe(Action<User> callback, Action<Error> errorCallback)
        {
            UnityWebRequest request = UnityWebRequest.Get(BaseUrl + "users/me");
            StartCoroutine(DoWebRequestAsync(request, FirstResultFromJson<User>, callback, errorCallback, authenticate:true));
        }


        //UnmuteUser not yet implemented
        //MuteUser not yet implemented
        //UpdateUserSession not yet implemented



        // --- OBSERVATION TILES ---

        //GetColoredHeatmap not yet implemented
        //GetGridTiles not yet implemented
        //GetHeatmapTiles not yet implemented
        //GetPointsTiles not yet implemented


        // --- POLYGON TILES ---

        //GetPlaceTiles not yet implemented
        //GetTaxonPlaceTiles not yet implemented
        //GetTaxonRangeTiles not yet implemented

        // --- UTFGRID ---

        //GetColoredHeatmapTilesUtfGrid not yet implemented
        //GetGridTilesUtfGrid not yet implemented
        //GetHeatmapTilesUtfGrid not yet implemented
        //GetPointsTilesUtfGrid not yet implemented


        // --- PHOTOS ---

        //CreatePhoto not yet implemented TODO


    }
}