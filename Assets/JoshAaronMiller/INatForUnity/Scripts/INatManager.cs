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
        static Results<T> ResultsFromJson<T>(string jsonString) => JsonObject<Results<T>>.CreateFromJson(jsonString);
        static T FirstResultFromJson<T>(string jsonString) => ResultsFromJson<T>(jsonString).results[0];

        static T NoOp<T>(T x) => x;

        static readonly string UserAgent = "iNat+Unity by Josh Aaron Miller";

        static readonly float ServerSleepTime = 3; //be nice to server
        static float TimeSinceLastServerCall = float.MaxValue;

        string apiToken = "";

        public enum PlaceAdminLevel { 
            None = -999,
            Continent = -1, 
            Country = 0,
            State = 1,
            County = 2,
            Town = 3,
            Park = 10
        };


        private void Update()
        {
            if (TimeSinceLastServerCall < ServerSleepTime)
            {
                TimeSinceLastServerCall += Time.unscaledDeltaTime;
            }
        }

        /// <summary>
        /// Returns whether the INatManager is currently rate limiting itself to be kind to the server.
        /// </summary>
        /// <returns>Whether the INatManager is rate limited. Only make server calls when not rate limited.</returns>
        bool IsRateLimited()
        {
            return TimeSinceLastServerCall < ServerSleepTime;
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
            TimeSinceLastServerCall = 0;
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

            //Uncomment for extra debug logging
            //string destination = Application.persistentDataPath + "/log.txt";
            //File.WriteAllText(destination, json);

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

            if (request.result != UnityWebRequest.Result.ProtocolError
                && request.result != UnityWebRequest.Result.ConnectionError
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
            yield return _SendWebRequestAsync(request, authenticate);

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
            yield return _SendWebRequestAsync(request, authenticate);

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

            // would be safer to wrap all calls in using blocks, but that makes it harder to use this generic request function
            request.Dispose();
        }

        /// <summary>
        /// Construct and return a PUT request.
        /// </summary>
        /// <param name="url">The URL to send a PUT to.</param>
        /// <param name="body">The string data to attach to the PUT.</param>
        /// <returns>The UnityWebRequest.</returns>
        UnityWebRequest _MakePutRequest(string url, string body)
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
        UnityWebRequest _MakePutRequest(string url, WWWForm body)
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
            UnityWebRequest request = _MakePutRequest(url, body);
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
            UnityWebRequest request = _MakePutRequest(url, body);
            request.method = "POST";
            return request;
        }



        #region Login
        public void GetApiToken()
        {
            Application.OpenURL("https://www.inaturalist.org/users/api_token");
        }

        public void SetApiToken(string token)
        {
            apiToken = token;
        }
        #endregion

        #region Annotations

        //CreateAnnotation not yet implemented
        //DeleteAnnotation not yet implemented
        //VoteAnnotation not yet implemented
        //UnvoteAnnotation not yet implemented

        #endregion

        #region Comments

        //CreateComment not yet implemented
        //DeleteComment not yet implemented
        //UpdateComment not yet implemented

        #endregion

        #region Controlled Terms

        /// <summary>
        /// Fetch a list of all attribute controlled terms as a List of ControlledTerms.
        /// </summary>
        /// <param name="callback">A function to callback when the request is done which takes as input the Results list of Controlled Terms created.</param>
        /// <param name="errorCallback">A function to callback when iNaturalist returns an error message.</param>
        public void GetTermsIndex(Action<Results<ControlledTerm>> callback, Action<Error> errorCallback)
        {
            UnityWebRequest request = UnityWebRequest.Get(BaseUrl + "controlled_terms/");
            StartCoroutine(DoWebRequestAsync(request, ResultsFromJson<ControlledTerm>, callback, errorCallback));
        }

        /// <summary>
        /// Fetch a list of all attribute controlled terms relevant to a taxon as a List of ControlledTerms.
        /// </summary>
        /// <param name="taxonId">The ID of the Taxon.</param>
        /// <param name="callback">A function to callback when the request is done which takes as input the Results list of Controlled Terms created.</param>
        /// <param name="errorCallback">A function to callback when iNaturalist returns an error message.</param>
        public void GetTermsForTaxon(int taxonId, Action<Results<ControlledTerm>> callback, Action<Error> errorCallback)
        {
            UnityWebRequest request = UnityWebRequest.Get(BaseUrl + "controlled_terms/for_taxon?taxon_id=" + taxonId.ToString());
            StartCoroutine(DoWebRequestAsync(request, ResultsFromJson<ControlledTerm>, callback, errorCallback));
        }

        #endregion

        #region Flags

        /// <summary>
        /// Submit a Flag.
        /// </summary>
        /// <param name="flag">The parameters of the Flag.</param>
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
        /// <param name="flagId">The ID of the Flag.</param>
        /// <param name="callback">A function to callback when the request is done.</param>
        /// <param name="errorCallback">A function to callback when iNaturalist returns an error message.</param>
        public void DeleteFlag(int flagId, Action callback, Action<Error> errorCallback)
        {
            UnityWebRequest request = UnityWebRequest.Delete(BaseUrl + "flags/" + flagId.ToString());
            StartCoroutine(DoWebRequestAsync(request, callback, errorCallback));
        }

        /// <summary>
        /// Update a Flag.
        /// </summary>
        /// <param name="flagId">The ID of the Flag.</param>
        /// <param name="flag">The parameters of the flag.</param>
        /// <param name="callback">A function to callback when the request is done which takes as input the Flag updated.</param>
        /// <param name="errorCallback">A function to callback when iNaturalist returns an error message.</param>
        public void UpdateFlag(int flagId, WrappedFlag flag, Action<Identification> callback, Action<Error> errorCallback)
        {
            string postData = WrappedFlag.ToJson(flag);
            UnityWebRequest request = _MakePutRequest(BaseUrl + "flags/" + flagId.ToString(), postData);
            StartCoroutine(DoWebRequestAsync(request, FromJson<Identification>, callback, errorCallback));
        }

        #endregion

        #region Identifications


        /// <summary>
        /// Delete an Identification.
        /// </summary>
        /// <param name="identId">The ID of the Identification.</param>
        /// <param name="callback">A function to callback when the request is done.</param>
        /// <param name="errorCallback">A function to callback when iNaturalist returns an error message.</param>
        public void DeleteIdentification(int identId, Action<string> callback, Action<Error> errorCallback)
        {
            UnityWebRequest request = UnityWebRequest.Delete(BaseUrl + "identifications/" + identId.ToString());
            StartCoroutine(DoWebRequestAsync(request, NoOp, callback, errorCallback));
        }

        /// <summary>
        /// Given an array of IDs, returns corresponding Identifications.
        /// </summary>
        /// <param name="identIds">The List of Identification IDs to fetch</param>
        /// <param name="callback">A function to callback when the request is done which takes as input the Results list of Identification objects found.</param>
        /// <param name="errorCallback">A function to callback when iNaturalist returns an error message.</param>
        public void GetIdentifications(List<int> identIds, Action<Results<Identification>> callback, Action<Error> errorCallback)
        {
            string idsAsStringList = string.Join(",", identIds);
            UnityWebRequest request = UnityWebRequest.Get(BaseUrl + "identifications/" + idsAsStringList);
            StartCoroutine(DoWebRequestAsync(request, ResultsFromJson<Identification>, callback, errorCallback));
        }

        /// <summary>
        /// Given an ID, returns corresponding Identification.
        /// </summary>
        /// <param name="identId">The Identification ID to fetch</param>
        /// <param name="callback">A function to callback when the request is done which takes as input the Identification object found.</param>
        /// <param name="errorCallback">A function to callback when iNaturalist returns an error message.</param>
        public void GetIdentification(int identId, Action<Identification> callback, Action<Error> errorCallback)
        {
            UnityWebRequest request = UnityWebRequest.Get(BaseUrl + "identifications/" + identId.ToString());
            StartCoroutine(DoWebRequestAsync(request, FirstResultFromJson<Identification>, callback, errorCallback));
        }


        /// <summary>
        /// Update an Identification.
        /// </summary>
        /// <param name="identId">The Identification ID to update</param>
        /// <param name="identSub">The updated information for the Identification.</param>
        /// <param name="callback">A function to callback when the request is done which takes as input the Identification object returned.</param>
        /// <param name="errorCallback">A function to callback when iNaturalist returns an error message.</param>
        public void UpdateIdentification(int identId, IdentificationSubmission identSub, Action<Identification> callback, Action<Error> errorCallback)
        {
            WrappedIdentificationSubmission submission = new WrappedIdentificationSubmission();
            submission.identification = identSub;
            string putData = WrappedIdentificationSubmission.ToJson(submission);
            UnityWebRequest request = _MakePutRequest(BaseUrl + "identifications/" + identId.ToString(), putData);
            StartCoroutine(DoWebRequestAsync(request, FromJson<Identification>, callback, errorCallback));
        }


        /// <summary>
        /// Given an IdentificationSearch object, returns a list of matching Identifications.
        /// </summary>
        /// <param name="identSearch">An IdentificationSearch object holding the parameters of the search</param>
        /// <param name="callback">A function to callback when the request is done which takes as input the Results list of Identification objects found.</param>
        /// <param name="errorCallback">A function to callback when iNaturalist returns an error message.</param>
        public void SearchIdentifications(IdentificationSearch identSearch, Action<Results<Identification>> callback, Action<Error> errorCallback)
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
        /// Given an IdentificationSearch object, returns the number of identifications matching the search which have a particular category.
        /// </summary>
        /// <param name="identSearch">An IdentificationSearch object holding the parameters of the search.</param>
        /// <param name="callback">A function to callback when the request is done which takes as input the Results list of IdentificationCategoryCount objects found.</param>
        /// <param name="errorCallback">A function to callback when iNaturalist returns an error message.</param>
        public void GetIdentificationCategories(IdentificationSearch identSearch, Action<Results<IdentificationCategoryCount>> callback, Action<Error> errorCallback)
        {
            UnityWebRequest request = UnityWebRequest.Get(BaseUrl + "identifications/categories?" + identSearch.ToUrlParameters());
            StartCoroutine(DoWebRequestAsync(request, ResultsFromJson<IdentificationCategoryCount>, callback, errorCallback));
        }

        /// <summary>
        /// Given an IdentificationSearch object, returns the counts of how many identifications matching the search have a particular leaf taxon.
        /// </summary>
        /// <param name="identSearch">An IdentificationSearch object holding the parameters of the search.</param>
        /// <param name="callback">A function to callback when the request is done which takes as input the Results list of SpeciesCount objects found.</param>
        /// <param name="errorCallback">A function to callback when iNaturalist returns an error message.</param>
        public void GetIdentificationSpeciesCounts(IdentificationSearch identSearch, Action<Results<SpeciesCount>> callback, Action<Error> errorCallback)
        {
            UnityWebRequest request = UnityWebRequest.Get(BaseUrl + "identifications/species_counts?" + identSearch.ToUrlParameters());
            StartCoroutine(DoWebRequestAsync(request, ResultsFromJson<SpeciesCount>, callback, errorCallback));
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
        /// <param name="callback">A function to callback when the request is done which takes as input the Results list of SpeciesCount objects representing the results.</param>
        /// <param name="errorCallback">A function to callback when iNaturalist returns an error message.</param>
        public void GetSimilarSpecies(int taxonId, Action<Results<SpeciesCount>> callback, Action<Error> errorCallback)
        {
            UnityWebRequest request = UnityWebRequest.Get(BaseUrl + "identifications/similar_species?taxon_id=" + taxonId.ToString());
            StartCoroutine(DoWebRequestAsync(request, ResultsFromJson<SpeciesCount>, callback, errorCallback));
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
        /// <param name="obsSearch">Additional parameters to refine the search, limiting what observations can be included.</param>
        /// <param name="callback">A function to callback when the request is done which takes as input the Results list of SpeciesCount objects representing the results.</param>
        /// <param name="errorCallback">A function to callback when iNaturalist returns an error message.</param>
        public void GetSimilarSpecies(int taxonId, ObservationSearch obsSearch, Action<Results<SpeciesCount>> callback, Action<Error> errorCallback)
        {
            UnityWebRequest request = UnityWebRequest.Get(BaseUrl + "identifications/similar_species?taxon_id=" + taxonId.ToString() + "&" + obsSearch.ToUrlParameters());
            StartCoroutine(DoWebRequestAsync(request, ResultsFromJson<SpeciesCount>, callback, errorCallback));
        }

        #endregion

        #region Messages

        /// <summary>
        /// Given a MessageSearch object, returns a list of matching UserMessages.
        /// </summary>
        /// <remarks>
        /// This function does not mark these messages as read. See GetUserMessageThread.
        /// </remarks>
        /// <param name="messageSearch">A MessageSearch object holding the parameters of the search</param>
        /// <param name="callback">A function to callback when the request is done which takes as input the Results list of UserMessage objects found.</param>
        /// <param name="errorCallback">A function to callback when iNaturalist returns an error message.</param>
        public void SearchUserMessages(MessageSearch messageSearch, Action<Results<UserMessage>> callback, Action<Error> errorCallback)
        {
            UnityWebRequest request = UnityWebRequest.Get(BaseUrl + "messages?" + messageSearch.ToUrlParameters());
            StartCoroutine(DoWebRequestAsync(request, ResultsFromJson<UserMessage>, callback, errorCallback, authenticate:true));
        }

        /// <summary>
        /// Send a private message.
        /// </summary>
        /// <param name="newMessage">The message to be sent.</param>
        /// <param name="callback">A function to callback when the request is done which takes as input the new UserMessage created.</param>
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
        /// <param name="callback">A function to callback when the request is done which takes as input the thread fetched as a Results list of UserMessages.</param>
        /// <param name="errorCallback">A function to callback when iNaturalist returns an error message.</param>
        public void GetUserMessageThread(int threadId, Action<Results<UserMessage>> callback, Action<Error> errorCallback)
        {
            UnityWebRequest request = UnityWebRequest.Get(BaseUrl + "messages/" + threadId.ToString());
            StartCoroutine(DoWebRequestAsync(request, ResultsFromJson<UserMessage>, callback, errorCallback, authenticate: true));
        }


        /// <summary>
        /// Returns a Count of unread messages in the authenticated user's inbox.
        /// </summary>
        /// <param name="callback">A function to callback when the request is done which takes as input the Count of unread messages.</param>
        /// <param name="errorCallback">A function to callback when iNaturalist returns an error message.</param>
        public void GetUnreadMessagesCount(Action<Count> callback, Action<Error> errorCallback)
        {
            UnityWebRequest request = UnityWebRequest.Get(BaseUrl + "messages/unread");
            StartCoroutine(DoWebRequestAsync(request, FromJson<Count>, callback, errorCallback, authenticate: true));
        }

        #endregion

        #region Observation Field Values

        //DeleteObservationFieldValue not yet implemented
        //UpdateObservationFieldValue not yet implemented
        //CreateObservationFieldValue not yet implemented

        #endregion

        #region Observation Photos

        /// <summary>
        /// Delete an Observation Photo.
        /// </summary>
        /// <param name="obsPhotoId">The Observation Photo to delete.</param>
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
        /// <param name="obsPhotoId">The Observation Photo ID to update.</param>
        /// <param name="position">The position in which the Photo is displayed for the observation.</param>
        /// <param name="photo">The Photo to upload.</param>
        /// <param name="callback">A function to callback when the request is done which takes as input the Observation Photo object returned.</param>
        /// <param name="errorCallback">A function to callback when iNaturalist returns an error message.</param>
        public void UpdateObservationPhoto(int obsPhotoId, int position, PhotoFile photo, Action<ObservationPhoto> callback, Action<Error> errorCallback)
        {
            WWWForm formData = new WWWForm();
            formData.AddField("observation_photo[position]", position);
            formData.AddBinaryData("file", photo.ToBytes(), photo.fileName, photo.fileType);

            UnityWebRequest request = _MakePutRequest(BaseUrl + "observation_photos/" + obsPhotoId.ToString(), formData);
            StartCoroutine(DoWebRequestAsync(request, FromJson<ObservationPhoto>, callback, errorCallback));
        }

        /// <summary>
        /// Create an Observation Photo.
        /// </summary>
        /// <param name="obsId">The Observation ID to add the Photo to.</param>
        /// <param name="obsUuid">The UUID of the observation.</param>
        /// <param name="photo">The Photo to upload.</param>
        /// <param name="callback">A function to callback when the request is done which takes as input the Observation Photo object returned.</param>
        /// <param name="errorCallback">A function to callback when iNaturalist returns an error message.</param>
        public void CreateObservationPhoto(int obsId, string obsUuid, PhotoFile photo, Action<ObservationPhoto> callback, Action<Error> errorCallback)
        {
            WWWForm formData = new WWWForm();
            formData.AddField("observation_photo[observation_id]", obsId);
            formData.AddField("observation_photo[uuid]", obsUuid);
            formData.AddBinaryData("file", photo.ToBytes(), photo.fileName, photo.fileType);

            UnityWebRequest request = MakePostRequest(BaseUrl + "observation_photos", formData);
            StartCoroutine(DoWebRequestAsync(request, FromJson<ObservationPhoto>, callback, errorCallback));
        }

        #endregion

        #region Observations

        /// <summary>
        /// Delete an Observation.
        /// </summary>
        /// <param name="obsId">The ID of the Observation.</param>
        /// <param name="callback">A function to callback when the request is done.</param>
        /// <param name="errorCallback">A function to callback when iNaturalist returns an error message.</param>
        public void DeleteObservation(int obsId, Action callback, Action<Error> errorCallback)
        {
            UnityWebRequest request = UnityWebRequest.Delete(BaseUrl + "observations/" + obsId.ToString());
            StartCoroutine(DoWebRequestAsync(request, callback, errorCallback));
        }

        /// <summary>
        /// Given an array of IDs, returns corresponding observations .
        /// </summary>
        /// <param name="ids">The list of observation IDs to fetch.</param>
        /// <param name="callback">A function to callback when the request is done which takes as input the Results list of Observation objects found.</param>
        /// <param name="errorCallback">A function to callback when iNaturalist returns an error message.</param>
        public void GetObservations(List<int> ids, Action<Results<Observation>> callback, Action<Error> errorCallback)
        {
            string idsAsStringList = string.Join(",", ids);
            UnityWebRequest request = UnityWebRequest.Get(BaseUrl + "observations/" + idsAsStringList);
            StartCoroutine(DoWebRequestAsync(request, ResultsFromJson<Observation>, callback, errorCallback));
        }


        /// <summary>
        /// Given an ID, returns corresponding observations.
        /// </summary>
        /// <param name="id">The observation ID to fetch.</param>
        /// <param name="callback">A function to callback when the request is done which takes as input the Observation object found.</param>
        /// <param name="errorCallback">A function to callback when iNaturalist returns an error message.</param>
        public void GetObservation(int id, Action<Observation> callback, Action<Error> errorCallback)
        {
            UnityWebRequest request = UnityWebRequest.Get(BaseUrl + "observations/" + id.ToString());
            StartCoroutine(DoWebRequestAsync(request, FirstResultFromJson<Observation>, callback, errorCallback));
        }


        /// <summary>
        /// Update an Observation.
        /// </summary>
        /// <param name="obsId">The Observation ID to update.</param>
        /// <param name="observation">An ObservationSubmission object containing the updated information for the observation.</param>
        /// <param name="callback">A function to callback when the request is done which takes as input the Observation object returned.</param>
        /// <param name="errorCallback">A function to callback when iNaturalist returns an error message.</param>
        public void UpdateObservation(int obsId, ObservationSubmission observation, Action<Observation> callback, Action<Error> errorCallback)
        {
            WrappedObservationSubmission submission = new WrappedObservationSubmission();
            submission.observation = observation;
            string putData = WrappedObservationSubmission.ToJson(submission);
            UnityWebRequest request = _MakePutRequest(BaseUrl + "observations/" + obsId.ToString(), putData);
            StartCoroutine(DoWebRequestAsync(request, FromJson<Observation>, callback, errorCallback));
        }

        /// <summary>
        /// "Fave" an Observation.
        /// </summary>
        /// <param name="obsId">The Observation ID.</param>
        /// <param name="callback">A function to callback when the request is done which takes as input the Observation object returned.</param>
        /// <param name="errorCallback">A function to callback when iNaturalist returns an error message.</param>
        public void FaveObservation(int obsId, Action<Observation> callback, Action<Error> errorCallback)
        {
            UnityWebRequest request = MakePostRequest(BaseUrl + "observations/" + obsId.ToString() + "/fave", ""); //API doesn't expect any POST data
            StartCoroutine(DoWebRequestAsync(request, FromJson<Observation>, callback, errorCallback));
        }

        /// <summary>
        /// "Unfave" an Observation, or delete Fave mark.
        /// </summary>
        /// <param name="obsId">The Observation ID.</param>
        /// <param name="callback">A function to callback when the request is done.</param>
        /// <param name="errorCallback">A function to callback when iNaturalist returns an error message.</param>
        public void UnfaveObservation(int obsId, Action callback, Action<Error> errorCallback)
        {
            UnityWebRequest request = UnityWebRequest.Delete(BaseUrl + "observations/" + obsId.ToString() + "/unfave");
            StartCoroutine(DoWebRequestAsync(request, callback, errorCallback));
        }

        /// <summary>
        /// Mark an observation as reviewed by the authenticated user.
        /// </summary>
        /// <param name="obsId">The Observation ID.</param>
        /// <param name="callback">A function to callback when the request is done which takes as input the Observation object returned.</param>
        /// <param name="errorCallback">A function to callback when iNaturalist returns an error message.</param>
        public void ReviewObservation(int obsId, Action<Observation> callback, Action<Error> errorCallback)
        {
            UnityWebRequest request = MakePostRequest(BaseUrl + "observations/" + obsId.ToString() + "/review", ""); //API doesn't expect any POST data
            StartCoroutine(DoWebRequestAsync(request, FromJson<Observation>, callback, errorCallback));
        }

        /// <summary>
        /// Mark an observation as unreviewed by the authenticated user.
        /// </summary>
        /// <param name="obsId">The Observation ID.</param>
        /// <param name="callback">A function to callback when the request is done.</param>
        /// <param name="errorCallback">A function to callback when iNaturalist returns an error message.</param>
        public void UnreviewObservation(int obsId, Action callback, Action<Error> errorCallback)
        {
            UnityWebRequest request = UnityWebRequest.Delete(BaseUrl + "observations/" + obsId.ToString() + "/unreview");
            StartCoroutine(DoWebRequestAsync(request, callback, errorCallback));
        }



        //GetObservationSubscriptions not yet implemented
        //   API hook seems confusing and/or deprecated

        //DeleteQualityMetric not yet implemented
        //SetQualityMetric not yet implemented

        /// <summary>
        /// Given an observation ID, fetch the TaxonSummary of that Observation.
        /// </summary>
        /// <remarks>
        /// Probably most useful for getting a Wikipedia summary of the taxon, see TaxonSummary object.
        /// </remarks>
        /// <param name="obsId">The observation ID to fetch</param>
        /// <param name="callback">A function to callback when the request is done which takes as input the TaxonSummary object returned.</param>
        /// <param name="errorCallback">A function to callback when iNaturalist returns an error message.</param>
        public void GetObservationTaxonSummary(int obsId, Action<TaxonSummary> callback, Action<Error> errorCallback)
        {
            UnityWebRequest request = UnityWebRequest.Get(BaseUrl + "observations/" + obsId.ToString() + "/taxon_summary");
            StartCoroutine(DoWebRequestAsync(request, FirstResultFromJson<TaxonSummary>, callback, errorCallback));
        }


        //SubscribeToObservation not yet implemented
        //    always returns true

        /// <summary>
        /// Vote on an Observation; see Vote object for usage.
        /// </summary>
        /// <param name="obsId">The Observation ID.</param>
        /// <param name="vote">The Vote to submit.</param>
        /// <param name="callback">A function to callback when the request is done which takes as input the Observation object returned.</param>
        /// <param name="errorCallback">A function to callback when iNaturalist returns an error message.</param>
        public void VoteObservation(int obsId, Vote vote, Action<Observation> callback, Action<Error> errorCallback)
        {
            UnityWebRequest request = MakePostRequest(BaseUrl + "votes/vote/observation/" + obsId.ToString(), Vote.ToJson(vote));
            StartCoroutine(DoWebRequestAsync(request, FromJson<Observation>, callback, errorCallback));
        }


        /// <summary>
        /// Delete a Vote from an Observation.
        /// </summary>
        /// <param name="obsId">The Observation ID.</param>
        /// <param name="vote">The Vote to delete.</param>
        /// <param name="callback">A function to callback when the request is done.</param>
        /// <param name="errorCallback">A function to callback when iNaturalist returns an error message.</param>
        public void UnvoteObservation(int obsId, Vote vote, Action callback, Action<Error> errorCallback)
        {
            UnityWebRequest request = _MakePutRequest(BaseUrl + "votes/unvote/observation/", Vote.ToJson(vote)); //weirdly, this DELETE has a body
            request.method = "DELETE";
            StartCoroutine(DoWebRequestAsync(request, callback, errorCallback));
        }


        /// <summary>
        /// Given an ObservationSearch object, returns a list of matching observations
        /// </summary>
        /// <param name="obsSearch">An ObservationSearch object holding the parameters of the search</param>
        /// <param name="callback">A function to callback when the request is done which takes as input the Results list of Observation objects found.</param>
        /// <param name="errorCallback">A function to callback when iNaturalist returns an error message.</param>
        public void SearchObservations(ObservationSearch obsSearch, Action<Results<Observation>> callback, Action<Error> errorCallback)
        {
            UnityWebRequest request = UnityWebRequest.Get(BaseUrl + "observations?" + obsSearch.ToUrlParameters());
            StartCoroutine(DoWebRequestAsync(request, ResultsFromJson<Observation>, callback, errorCallback));
        }

        /// <summary>
        /// Create an Observation.
        /// </summary>
        /// <param name="observation">An ObservationSubmission object containing the information for the observation.</param>
        /// <param name="callback">A function to callback when the request is done which takes as input the Observation object returned.</param>
        /// <param name="errorCallback">A function to callback when iNaturalist returns an error message.</param>
        public void CreateObservation(ObservationSubmission observation, Action<Observation> callback, Action<Error> errorCallback)
        {
            WrappedObservationSubmission submission = new WrappedObservationSubmission();
            submission.observation = observation;
            string postData = WrappedObservationSubmission.ToJson(submission);
            UnityWebRequest request = MakePostRequest(BaseUrl + "observations", postData);
            StartCoroutine(DoWebRequestAsync(request, FromJson<Observation>, callback, errorCallback));
        }



        //GetDeletedObservations not yet implemented

        //GetObservationHistogram not yet implemented
        /* This is a neat function, but likely would require 
        *  being able to JSONify dictionaries
        *  which is currently unsupported by JsonUtility.
        *  If you need a particular histogram, you can probably hard-code
        *  something to get the data structure you need.
        */

        //GetObservationIdentifiers not yet implemented
        //GetObservationObservers not yet implemented
        //GetObservationPopularFieldValues not yet implemented


        /// <summary>
        /// Given an ObservationSearch object, returns the counts of how many observations matching the search have a particular leaf taxon.
        /// </summary>
        /// <param name="observationSearch">An ObservationSearch object holding the parameters of the search.</param>
        /// <param name="callback">A function to callback when the request is done which takes as input the Results list of SpeciesCount objects found.</param>
        /// <param name="errorCallback">A function to callback when iNaturalist returns an error message.</param>
        public void GetObservationSpeciesCounts(ObservationSearch observationSearch, Action<Results<SpeciesCount>> callback, Action<Error> errorCallback)
        {
            UnityWebRequest request = UnityWebRequest.Get(BaseUrl + "observations/species_counts?" + observationSearch.ToUrlParameters());
            StartCoroutine(DoWebRequestAsync(request, ResultsFromJson<SpeciesCount>, callback, errorCallback));
        }

        //GetObservationUserUpdates not yet implemented
        //MarkObservationUpdatesAsViewed not yet implemented

        #endregion

        #region Places

        /// <summary>
        /// Given an ID, return the corresponding Place.
        /// </summary>
        /// <param name="placeId">The ID of the Place.</param>
        /// <param name="adminLevel">Optionally, the admin level of the place to search.</param>
        /// <param name="callback">A function to callback when the request is done which takes as input the Place fetched.</param>
        /// <param name="errorCallback">A function to callback when iNaturalist returns an error message.</param>
        public void GetPlaceDetails(int placeId, Action<Place> callback, Action<Error> errorCallback, PlaceAdminLevel adminLevel = PlaceAdminLevel.None)
        {
            string urlSuffix = placeId.ToString();
            if (adminLevel != PlaceAdminLevel.None)
            {
                urlSuffix += "?admin_level=" + ((int)adminLevel).ToString();
            }
            UnityWebRequest request = UnityWebRequest.Get(BaseUrl + "places/" + urlSuffix);
            StartCoroutine(DoWebRequestAsync(request, FirstResultFromJson<Place>, callback, errorCallback));
        }

        /// <summary>
        /// Given a list of IDs, return the corresponding Places.
        /// </summary>
        /// <param name="placeIds">The IDs of the Places.</param>
        /// <param name="adminLevel">Optionally, the admin level of the place to search.</param>
        /// <param name="callback">A function to callback when the request is done which takes as input the Results list of Places fetched.</param>
        /// <param name="errorCallback">A function to callback when iNaturalist returns an error message.</param>
        public void GetPlaceDetails(List<int> placeIds, Action<Results<Place>> callback, Action<Error> errorCallback, PlaceAdminLevel adminLevel = PlaceAdminLevel.None)
        {
            string idsAsStringList = string.Join(",", placeIds);
            string urlSuffix = idsAsStringList;
            if (adminLevel != PlaceAdminLevel.None)
            {
                urlSuffix += "?admin_level=" + ((int)adminLevel).ToString();
            }
            UnityWebRequest request = UnityWebRequest.Get(BaseUrl + "places/" + urlSuffix);
            StartCoroutine(DoWebRequestAsync(request, ResultsFromJson<Place>, callback, errorCallback));
        }


        /// <summary>
        /// Given a string query, find all places with names starting with the query.
        /// </summary>
        /// <param name="query">The search term.</param>
        /// <param name="orderByArea">If true, sort the results by area (default false).</param>
        /// <param name="callback">A function to callback when the request is done which takes as input the Results list of Places fetched.</param>
        /// <param name="errorCallback">A function to callback when iNaturalist returns an error message.</param>
        public void GetPlacesAutocomplete(string query, Action<Results<Place>> callback, Action<Error> errorCallback, bool orderByArea = false)
        {
            string urlSuffix = query;
            if (orderByArea)
            {
                urlSuffix += "&order_by=area";
            }
            UnityWebRequest request = UnityWebRequest.Get(BaseUrl + "places/autocomplete?q=" + urlSuffix);
            StartCoroutine(DoWebRequestAsync(request, ResultsFromJson<Place>, callback, errorCallback));
        }


        /// <summary>
        /// Given a bounding box and optional name query, return nearby places separated by curation status.
        /// </summary>
        /// <param name="nelat">The northeast latitude corner of the bounding box.</param>
        /// <param name="nelng">The northeast longitude corner of the bounding box.</param>
        /// <param name="swlat">The southwest latitude corner of the bounding box.</param>
        /// <param name="swlng">The southwest longitude corner of the bounding box.</param>
        /// <param name="name">The optional search term for the name.</param>
        /// <param name="perPage">Number of results per page (default 30, max 200).</param>
        /// <param name="callback">A function to callback when the request is done which takes as input the list of PlacesByCuration fetched.</param>
        /// <param name="errorCallback">A function to callback when iNaturalist returns an error message.</param>
        public void GetPlacesNearby(double nelat, double nelng, double swlat, double swlng, Action<PlacesByCuration> callback, Action<Error> errorCallback, string name = "", int perPage = 30)
        {
            string urlSuffix = String.Format("nelat={0}&nelng={1}&swlat={2}&swlng={3}", nelat, nelng, swlat, swlng);
            if (name != "")
            {
                urlSuffix += "&name=" + name;
            }
            urlSuffix += "&per_page=" + perPage.ToString();
            UnityWebRequest request = UnityWebRequest.Get(BaseUrl + "places/nearby?" + urlSuffix);
            StartCoroutine(DoWebRequestAsync(request, FirstResultFromJson<PlacesByCuration>, callback, errorCallback));
        }

        #endregion

        #region Posts

        // SearchPosts not yet implemented
        // CreatePost not yet implemented
        // DeletePost not yet implemented
        // UpdatePost not yet implemented
        // GetPostsForUser not yet implemented

        #endregion

        #region Project Observations

        //DeleteProjectObservation not yet implemented
        //UpdateProjectObservation not yet implemented
        //CreateProjectObservation not yet implemented

        #endregion

        #region Projects

        //SearchProjects not yet implemented
        //GetProjectDetails not yet implemented
        //JoinProject not yet implemented
        //LeaveProject not yet implemented
        //GetProjectMembers not yet implemented
        //GetMyProjectMembership not yet implemented
        //AddProject not yet implemented
        //RemoveProject not yet implemented
        //GetProjectAutocomplete not yet implemented

        #endregion

        #region Search

        //SearchSite not yet implemented

        #endregion

        #region Taxa

        /// <summary>
        /// Given an array of IDs, returns corresponding Taxa 
        /// </summary>
        /// <param name="taxaIds">The list of Taxa IDs to fetch</param>
        /// <param name="callback">A function to callback when the request is done which takes as input the Results list of Taxon objects found.</param>
        /// <param name="errorCallback">A function to callback when iNaturalist returns an error message.</param>
        public void GetTaxonDetails(List<int> taxaIds, Action<Results<Taxon>> callback, Action<Error> errorCallback)
        {
            string idsAsStringList = string.Join(",", taxaIds);
            UnityWebRequest request = UnityWebRequest.Get(BaseUrl + "taxa/" + idsAsStringList);
            StartCoroutine(DoWebRequestAsync(request, ResultsFromJson<Taxon>, callback, errorCallback));
        }

        /// <summary>
        /// Given an ID, returns corresponding Taxon 
        /// </summary>
        /// <param name="taxonId">The Taxon ID to fetch</param>
        /// <param name="callback">A function to callback when the request is done which takes as input the Taxon object found.</param>
        /// <param name="errorCallback">A function to callback when iNaturalist returns an error message.</param>
        public void GetTaxonDetails(int taxonId, Action<Taxon> callback, Action<Error> errorCallback)
        {
            UnityWebRequest request = UnityWebRequest.Get(BaseUrl + "taxa/" + taxonId.ToString());
            StartCoroutine(DoWebRequestAsync(request, FirstResultFromJson<Taxon>, callback, errorCallback));
        }


        /// <summary>
        /// Given a TaxonSearch object, returns a list of matching Taxa
        /// </summary>
        /// <param name="taxonSearch">An TaxonSearch object holding the parameters of the search</param>
        /// <param name="callback">A function to callback when the request is done which takes as input the Results list of Taxon objects found.</param>
        /// <param name="errorCallback">A function to callback when iNaturalist returns an error message.</param>
        public void SearchTaxa(TaxonSearch taxonSearch, Action<Results<Taxon>> callback, Action<Error> errorCallback)
        {
            UnityWebRequest request = UnityWebRequest.Get(BaseUrl + "taxa?" + taxonSearch.ToUrlParameters());
            StartCoroutine(DoWebRequestAsync(request, ResultsFromJson<Taxon>, callback, errorCallback));
        }

        #endregion

        #region Users

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


        #endregion

        #region Observation Tiles

        //GetColoredHeatmap not yet implemented
        //GetGridTiles not yet implemented
        //GetHeatmapTiles not yet implemented
        //GetPointsTiles not yet implemented

        #endregion

        #region Polygon Tiles

        //GetPlaceTiles not yet implemented
        //GetTaxonPlaceTiles not yet implemented
        //GetTaxonRangeTiles not yet implemented

        #endregion

        #region UTF Grid

        //GetColoredHeatmapTilesUtfGrid not yet implemented
        //GetGridTilesUtfGrid not yet implemented
        //GetHeatmapTilesUtfGrid not yet implemented
        //GetPointsTilesUtfGrid not yet implemented

        #endregion

        #region Photos

        /// <summary>
        /// Create a Photo.
        /// </summary>
        /// <param name="photo">The Photo to upload.</param>
        /// <param name="callback">A function to callback when the request is done which takes as input the Photo object returned.</param>
        /// <param name="errorCallback">A function to callback when iNaturalist returns an error message.</param>
        public void CreatePhoto(PhotoFile photo, Action<PhotoJson> callback, Action<Error> errorCallback)
        {
            WWWForm formData = new WWWForm();
            formData.AddBinaryData("file", photo.ToBytes(), photo.fileName, photo.fileType);

            UnityWebRequest request = MakePostRequest(BaseUrl + "photos", formData);
            StartCoroutine(DoWebRequestAsync(request, FromJson<PhotoJson>, callback, errorCallback));
        }

        #endregion

    }
}