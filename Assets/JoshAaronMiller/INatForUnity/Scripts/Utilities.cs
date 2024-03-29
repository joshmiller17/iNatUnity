﻿using System.Collections;
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
        /// <param name="callback">Optionally, a void function to call once the image is loaded successfully.</param>
        /// <returns></returns>
        public static IEnumerator LoadImageFromPath(string fullPath, GameObject loadTo, System.Action callback = null)
        {
            Debug.Log("Loading image from: " + fullPath);
            RawImage rawImage = loadTo.GetComponent<RawImage>();
            if (rawImage == null)
            {
                rawImage = loadTo.AddComponent<RawImage>();
            }

            UnityWebRequest request = UnityWebRequestTexture.GetTexture(fullPath);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("LoadImageFromPath failed with status code " + request.responseCode.ToString());
                Debug.LogError(request.error);
            }
            else
            {
                rawImage.texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
                if (callback != null)
                {
                    callback();
                }
            }
        }

        /// <summary>
        /// A simple implementation of Python's defaultdict, which creates a default value if the key does not exist.
        /// Adapted from https://stackoverflow.com/questions/15622622/analogue-of-pythons-defaultdict
        /// </summary>
        public class DefaultDictionary<Key, Value> : Dictionary<Key, Value> where Value : new()
        {
            public new Value this[Key key]
            {
                get
                {
                    Value val;
                    if (!TryGetValue(key, out val))
                    {
                        val = new Value();
                        Add(key, val);
                    }
                    return val;
                }
                set { base[key] = value; }
            }
        }
    }
}