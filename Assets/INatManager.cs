using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;
using UnityEngine.Networking;

public class INatManager : MonoBehaviour
{
    public static string BaseUrl = "https://api.inaturalist.org/v1/";

    /// <summary>
    /// Performs a web request.
    /// </summary>
    /// <param name="request">The HttpWebRequest</param>
    /// <returns>The JSON response in string format. To extract, use JsonUtility.FromJson<DataType>(jsonString)</returns>
    public string DoWebRequest(HttpWebRequest request)
    {
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
}
