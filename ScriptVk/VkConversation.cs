using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
        public async static void LoadConversations(VkApi api, ulong? conversationCount, ObservableCollection<VkConversation> vkConversations, CancellationTokenSource cts)
        {

            var conversationsCollection = api.Messages.GetConversations(new VkNet.Model.RequestParams.GetConversationsParams()
            {
                Count = conversationCount
            }).Items;
            foreach (var conv in conversationsCollection)
            {
                if (cts.IsCancellationRequested)
                    return;
                vkConversations.Add(new VkConversation(api, conv));
                await Task.Delay(15);
            }
        }

        public override string ToString()
        {
            return Title;
        }
    }
}
