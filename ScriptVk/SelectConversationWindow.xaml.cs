using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using VkNet;

namespace ScriptVk
{

    public partial class SelectConversationWindow : Window
    {
        private VkApi Api;

        private ulong? ConversationCount;

        List<VkConversation> conversationCollection;

        public SelectConversationWindow(VkApi api, int conversationCount)
        {
            InitializeComponent();
            Api = api;
            ConversationCount = (ulong?)conversationCount;
            try
            {
                LoadConversations();
            }
            catch (Exception error)
            {
                MessageBox.Show("Произошла ошибка при загрузке диалогов: " + error);
            }
        }

        private void LoadConversations()
        {
            var conversationsCollection = Api.Messages.GetConversations(new VkNet.Model.RequestParams.GetConversationsParams() { 
            Count = ConversationCount
            }).Items;
            conversationCollection = new List<VkConversation>();
            foreach (var dialog in conversationsCollection)
            {
                conversationCollection.Add(new VkConversation(Api, dialog));
            }
            ConversationList.ItemsSource = conversationCollection;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Instance.Conversation = ConversationList.SelectedItem as VkConversation;
            Close();
        }

        private void ConversationList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(ConversationList.SelectedIndex != - 1)
            {
                Select.IsEnabled = true;
            }
            else
            {
                Select.IsEnabled = false;
            }
        }
    }
}
