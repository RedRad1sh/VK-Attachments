using System;
using System.Windows;
using System.Windows.Controls;
using VkNet;

namespace ScriptVk
{

    public partial class SelectConversationWindow : Window
    {
        private VkApi Api;

        public SelectConversationWindow(VkApi api, int conversationCount)
        {
            InitializeComponent();
            Api = api;
            ulong? ConversationCount = (ulong?)conversationCount;
            try
            {
                ConversationList.ItemsSource = VkConversation.LoadConversations(Api, ConversationCount);
            }
            catch (Exception error)
            {
                MessageBox.Show("Произошла ошибка при загрузке диалогов: " + error);
            }
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
