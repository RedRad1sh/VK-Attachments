using System.Collections.Generic;
using System.Linq;
using VkNet;

namespace ScriptVk
{
    /// <summary>
    /// Беседа.
    /// </summary>
    public class VkConversation
    {
        /// <summary>
        /// Название беседы
        /// </summary>
        public string Title { get; private set; }
        /// <summary>
        /// ID беседы
        /// </summary>
        public long ID { get; private set; }
        /// <summary>
        /// Тип беседы: "chat", "group", "user"
        /// </summary>
        public string type { get; private set; }

        /// <summary>
        /// Инициализирует новый экземпляр класса беседы
        /// </summary>
        /// <param name="api">Апи пользователя.</param>
        /// <param name="conversation">Объект беседы.</param>
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

        /// <summary>
        /// <para>Принимает объект класса VkApi и количество бесед</para>
        /// <para>Возвращает список бесед.</para>
        /// </summary>
        /// <param name="api">Апи пользователя</param>
        /// <param name="conversationCount">Количество бесед</param>
        /// <returns></returns>
        public static List<VkConversation> LoadConversations(VkApi api, ulong? conversationCount)
        {
            List<VkConversation> conversationCollection;

            var conversationsCollection = api.Messages.GetConversations(new VkNet.Model.RequestParams.GetConversationsParams()
            {
                Count = conversationCount
            }).Items;
            conversationCollection = new List<VkConversation>();
            foreach (var dialog in conversationsCollection)
            {
                conversationCollection.Add(new VkConversation(api, dialog));
            }
            return conversationCollection;
        }

        public override string ToString()
        {
            return Title;
        }
    }
}
