using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JoshAaronMiller.INaturalist
{
    [System.Serializable]
    public class UserMessage : JsonObject<UserMessage>
    {
        public int id;
        public int user_id;
        public int thread_id;
        public string subject;
        public string body;
        public int thread_messages_count; //only loaded if MessageSearch::ShowOnlyLatestMessagePerThread(true)
        public TimeDetails read_at;
        public TimeDetails created_at;
        public TimeDetails updated_at;
        public User from_user;
        public User to_user;
        public List<Flag> thread_flags;
    }
}