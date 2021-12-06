using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JoshAaronMiller.INaturalist
{
    /// <summary>
    /// A search object has a set of bool and string parameters for conducting a query using the INatManager.
    /// </summary>
    public abstract class SearchObject
    {
        protected Dictionary<string, bool> boolParams = new Dictionary<string, bool>();
        protected Dictionary<string, string> stringParams = new Dictionary<string, string>();

        /// <summary>
        /// Remove a parameter from the search.
        /// </summary>
        /// <param name="key">The key to remove</param>
        public void RemoveStringParameter(string key)
        {
            stringParams.Remove(key);
        }

        /// <summary>
        /// Returns this search object as a string of URL parameters.
        /// </summary>
        /// <returns>A string of URL parameters</returns>
        public string ToUrlParameters()
        {
            List<string> keyValuePairs = new List<string>();

            foreach (KeyValuePair<string, bool> entry in boolParams)
            {
                keyValuePairs.Add(entry.Key + "=" + entry.Value.ToString().ToLower());
            }

            foreach (KeyValuePair<string, string> entry in stringParams)
            {
                keyValuePairs.Add(entry.Key + "=" + entry.Value);
            }

            string ret = string.Join("&", keyValuePairs);
            ret.Replace(",", "%2C");
            return ret;
        }

    }
}