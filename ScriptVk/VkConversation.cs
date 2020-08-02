using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VkNet;

namespace ScriptVk
{
    public class VkConversation
    {   
        public string Title;
        public long ID;
        public string type;

        public VkConversation(VkApi api, VkNet.Model.ConversationAndLastMessage conversation)
        {
            type = conversation.Conversation.Peer.Type.ToString();
            switch (type)
            {
                case "chat":
                    ID = conversation.Conversation.Peer.Id;
                    Title = conversation.Conversation.ChatSettings.Title;
                    break;
                case "group":
                    ID = conversation.Conversation.Peer.LocalId;
                    Title = api.Groups.GetById(null, ID.ToString(), null).FirstOrDefault().Name;
                    break;
                case "user":
                    ID = conversation.Conversation.Peer.LocalId;
                    var user = api.Users.Get(new long[] { ID }).FirstOrDefault();
                    Title = user.FirstName + user.LastName;
                    break;
            }
        }
        public override string ToString()
        {
            return Title;
        }
    }
}
