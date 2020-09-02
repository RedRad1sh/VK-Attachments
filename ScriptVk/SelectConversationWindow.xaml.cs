using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using VkNet;

namespace ScriptVk
{

    public partial class SelectConversationWindow : Window
    {
        private VkApi Api;
        Action act;
        CancellationTokenSource cts;
        public SelectConversationWindow(VkApi api, int conversationCount)
        {
            cts = new CancellationTokenSource();
            ulong? ConversationCount = (ulong?)conversationCount;

            InitializeComponent();
            Api = api;
            ObservableCollection<VkConversation> vkConversations = new ObservableCollection<VkConversation>();
            ConversationList.ItemsSource = vkConversations;
            act = new Action(async () => VkConversation.LoadConversations(Api, ConversationCount, vkConversations, cts));
            try
            {
                Dispatcher.BeginInvoke(act);
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
            if (ConversationList.SelectedIndex != -1)
            {
                Select.IsEnabled = true;
            }
            else
            {
                Select.IsEnabled = false;
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            cts.Cancel();
        }
    }
}
