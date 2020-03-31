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
using System.Windows.Navigation;
using System.Windows.Shapes;
using VkNet;
using VkNet.Model;
using VkNet.Enums.Filters;
using VkNet.Model.RequestParams;
using System.Reflection;
using System.Net;
using VkNet.AudioBypassService.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace ScriptVk
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {
        public static MainWindow Instance { get; private set; }
        private VkApi api;
        private string ext = ".jpg";
        public string token;
        List<string> list;
        public MainWindow()
        {
            InitializeComponent();
            Instance = this;
        }
        public void Autorization(string token)
        {
            var service = new ServiceCollection();
            service.AddAudioBypass();
            api = new VkApi(service);
            ulong appID = 2685278;
            Settings settings = Settings.All;
            api.Authorize(new ApiAuthParams
            {
                AccessToken = token,
                ApplicationId = appID,
                Login = LoginEnter.Text,
                Password = PassEnter.Password,
                Settings = settings
            });
            Console.WriteLine(api.Token);
        }

        private void PhotoShow()
        {
            string str;
            Images.Items.Clear();
            var getHistoryAttachments = api.Messages.GetHistoryAttachments(new MessagesGetHistoryAttachmentsParams
            {
                MediaType = VkNet.Enums.SafetyEnums.MediaType.Photo,
                PeerId = Convert.ToInt64(ConvID.Text),
                Count = Convert.ToInt32(NumberOfAttach.Text)
            }, out str);
            list = new List<string>();
            foreach (var photo in getHistoryAttachments)
            {
                var photos = photo.Attachment.Instance as VkNet.Model.Attachments.Photo;
                var URL = photos.Sizes[photos.Sizes.Count - 1].Url.AbsoluteUri;
                Images.Items.Add(URL);
                list.Add(URL);
            }
        }

        private void VideoShow()
        {
            string str;
            Images.Items.Clear();
            var getHistoryAttachments = api.Messages.GetHistoryAttachments(new MessagesGetHistoryAttachmentsParams
            {
                MediaType = VkNet.Enums.SafetyEnums.MediaType.Video,
                PeerId = Convert.ToInt64(ConvID.Text),
                Count = Convert.ToInt32(NumberOfAttach.Text)
            }, out str);
            List<VkNet.Model.Attachments.Video> vidlist = new List<VkNet.Model.Attachments.Video>();
            foreach (var video in getHistoryAttachments)
            {
                var vid = video.Attachment.Instance as VkNet.Model.Attachments.Video;
                vidlist.Add(vid);
            }
            var videos = api.Video.Get(new VideoGetParams
            {
                Videos = vidlist
            });
            list = new List<string>();
            foreach (var item in videos)
            {
                if (item.Files.Mp4_240 != null)
                {
                    Images.Items.Add(item.Title);
                    list.Add(item.Files.Mp4_240.ToString());
                }
            }
        }

        private void AudioShow()
        {
            Images.Items.Clear();

            string str;

            var getHistoryAttachments = api.Messages.GetHistoryAttachments(new MessagesGetHistoryAttachmentsParams
            {
                MediaType = VkNet.Enums.SafetyEnums.MediaType.Audio,
                PeerId = Convert.ToInt64(ConvID.Text),
                Count = Convert.ToInt32(NumberOfAttach.Text)
            }, out str);

            list = new List<string>();
            foreach (var audio in getHistoryAttachments)
            {
                var aud = audio.Attachment.Instance as VkNet.Model.Attachments.Audio;
                list.Add(aud.Url.AbsoluteUri);
                Images.Items.Add(aud.Title);
            }

        }

        private void Images_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ext == ".jpg")
            {
                img.Source = new BitmapImage(new Uri(Images.SelectedItem.ToString()));
            }
        }

        private void Autorization_Click(object sender, RoutedEventArgs e)
        {
            AuthorizationForm GettingToken = new AuthorizationForm
            {
                login = LoginEnter.Text,
                pass = PassEnter.Password
            };
            GettingToken.ShowDialog();
            Autorization(token);
        }

        private void DownloadAll_Click(object sender, RoutedEventArgs e)
        {
            DownloadAttachments(Images.Items);
        }

        private void DownloadAttachments(System.Collections.IList collection)
        {
            int i = 0;
            foreach (var item in collection)
            {
                try
                {
                    using (WebClient client = new WebClient())
                    {
                        client.DownloadFile(list[collection.IndexOf(item)], AppDomain.CurrentDomain.BaseDirectory + "\\" + ConvID.Text + "\\" + item.ToString() + ext);
                    }
                } catch
                {
                    MessageBox.Show("Неизвестная ошибка при загрузке вложения", "Ошибка");
                }
                i++;
            }
        }

        private void DownloadCurrent_Click(object sender, RoutedEventArgs e)
        {
            DownloadAttachments(Images.SelectedItems);
        }

        private void ChoiseAttach()
        {
            switch (Extension.SelectedIndex)
            {
                case 0:
                    ext = ".jpg";
                    PhotoShow();
                    break;
                case 1:
                    ext = ".mp4";
                    VideoShow();
                    break;
                case 2:
                    ext = ".mp3";
                    AudioShow();
                    break;
                default:
                    ext = ".jpg";
                    PhotoShow();
                    break;
            }
        }

        private void ShowAttachments_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ChoiseAttach();
            }
            catch (Exception error)
            {
                MessageBox.Show("Ошибка открытия диалога. Причина: " + error.Message);
            }
    
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }
    }

}
