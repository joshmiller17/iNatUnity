using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JoshAaronMiller.INaturalist
{
    /// <summary>
    /// A MessageSearch is a set of parameters for searching an iNaturalist user's private messages.
    /// Usage: myINatManager.GetUserMessages(myMessageSearch)
    /// </summary>
    public class MessageSearch : SearchObject
    {

        public enum Box
        {
            Inbox, Sent, Any
        };

        /// <summary>
        /// Set the pagination page number.
        /// </summary>
        /// <param name="page">The page of results to fetch.</param>
        public void SetPage(int page)
        {
            stringParams["page"] = page.ToString();
        }

        /// <summary>
        /// Set which box to search, defaults to inbox.
        /// </summary>
        /// <param name="box">The box of messages to search.</param>
        public void SetBox(Box box)
        {
            stringParams["box"] = box.ToString().ToLower();
        }

        /// <summary>
        /// Set an optional query string to search for in the subject and body of messages.
        /// </summary>
        /// <param name="q">The query string; search will return only messages that match this query in subject or body.</param>
        public void SetQuery(string q)
        {
            stringParams["q"] = q;
        }

        /// <summary>
        /// Limit the search to only messages with a correspondent of this user ID or username.
        /// </summary>
        /// <param name="user">The user ID or username to limit the search to.</param>
        public void SetCorrespondent(string user)
        {
            stringParams["user_id"] = user;
        }

        /// <summary>
        /// If this setting is toggled on (default off), group the results by thread ID,
        /// only show the latest message per thread, and include a thread_messages_count
        /// which shows the total number of messages in that thread.
        /// </summary>
        /// <remarks>
        /// Note that this setting can only be used if query (`q`) is not set and
        /// thread_messages_count always reports the count from `any` box instead of the
        /// box specified.
        /// </remarks>
        /// <param name="on">Whether to only show the latest message per thread.</param>
        public void ShowOnlyLatestMessagePerThread(bool on = false)
        {
            if (stringParams.ContainsKey("q"))
            {
                Debug.LogWarning("Cannot use MessageSearch parameter `threads` if `q` has been set.");
            }

            if (stringParams.ContainsKey("box") && stringParams["box"] != "any")
            {
                Debug.LogWarning("MessageSearch will report inaccurate thread_messages_count because box is not set to any.");
            }

            boolParams["threads"] = on;
        }
    }
}