using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace JoshAaronMiller.INaturalist
{
    public static class Utilities
    {

        /// <summary>
        /// Loads an image from a local file or URL into the texture of a GameObject's RawImage component.
        /// Creates a RawImage component if none exists.
        /// </summary>
        /// <param name="fullPath">The full local path or URL.</param>
        /// <param name="loadTo">The GameObject to load the image into.</param>
        /// <returns></returns>
        public static IEnumerator LoadImageFromPath(string fullPath, GameObject loadTo)
        {
            Debug.Log("Loading image from: " + fullPath);
            RawImage rawImage = loadTo.GetComponent<RawImage>();
            if (rawImage == null)
            {
                rawImage = loadTo.AddComponent<RawImage>();
            }

            UnityWebRequest request = UnityWebRequestTexture.GetTexture(fullPath);
            yield return request.SendWebRequest();

            if (request.isHttpError || request.isNetworkError)
            {
                Debug.LogError("LoadImageFromPath failed with status code " + request.responseCode.ToString());
                Debug.LogError(request.error);
            }
            else
            {
                rawImage.texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            }
        }
    }
}