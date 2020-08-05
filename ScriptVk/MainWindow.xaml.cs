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
using System.IO;
using System.Collections.ObjectModel;
using System.Threading;

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
        private string imgNotAvailable = AppDomain.CurrentDomain.BaseDirectory + "\\" + "No_Image_Available.jpg";

        public VkConversation Conversation;
        public MainWindow()
        {
            InitializeComponent();
            Instance = this;
        }

        public void Autorization(string token)
        {
            try
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
                    Settings = settings
                });
                Console.WriteLine(api.Token);
                SecondStep.IsEnabled = true;
                ConvName.IsEnabled = false;
            }
            catch (Exception error)
            {
                DoneAuth.Visibility = Visibility.Visible;
                DoneAuth.Foreground = Brushes.DarkRed;
                DoneAuth.Text = "Ошибка";
                MessageBox.Show("Ошибка авторизации: " + error.HResult);
                SecondStep.IsEnabled = false;
                ThirdStep.IsEnabled = false;
                FourthStep.IsEnabled = false;
            }
        }

        private void PhotoShow()
        {
            AttachmentsList.Items.Clear();

            var getHistoryAttachments = GettingHistoryAttachments(VkNet.Enums.SafetyEnums.MediaType.Photo);

            foreach (var photo in getHistoryAttachments)
            {
                var photos = photo.Attachment.Instance as VkNet.Model.Attachments.Photo;
                var url = photos.Sizes[photos.Sizes.Count - 1].Url.AbsoluteUri;
                var attachment = new VkAttachment.VkPhoto(url.Split('/').LastOrDefault(), url);
                AttachmentsList.Items.Add(attachment);
            }
        }

        private void DocumentShow()
        {
            AttachmentsList.Items.Clear();
            var getHistoryAttachments = GettingHistoryAttachments(VkNet.Enums.SafetyEnums.MediaType.Doc);


            foreach (var doc in getHistoryAttachments)
            {
                VkNet.Model.Attachments.Document document;
                if ((document = doc.Attachment.Instance as VkNet.Model.Attachments.Document) != null)
                {
                    var preview = imgNotAvailable;
                    try { preview = document.Preview.Photo.Url.AbsoluteUri; } catch { }
                    var attachment = new VkAttachment.VkDocument(document.Title, document.Uri, "." + document.Ext, preview);
                    AttachmentsList.Items.Add(attachment);
                }
            }
        }

        private void LinkShow()
        {
            AttachmentsList.Items.Clear();
            var getHistoryAttachments = GettingHistoryAttachments(VkNet.Enums.SafetyEnums.MediaType.Link);


            foreach (var link in getHistoryAttachments)
            {
                var links = link.Attachment.Instance as VkNet.Model.Attachments.Link;
                var preview = imgNotAvailable;
                try { preview = links.Image; } catch { }
                var attachment = new VkAttachment.VkLink(links.Title, links.Uri.AbsoluteUri, preview);
                AttachmentsList.Items.Add(attachment);
            }
        }

        private void VideoShow()
        {
            AttachmentsList.Items.Clear();

            var getHistoryAttachments = GettingHistoryAttachments(VkNet.Enums.SafetyEnums.MediaType.Video);

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
            foreach (var item in videos)
            {
                var attachment = new VkAttachment.VkVideo(item.Title, item.Files, item.Image.LastOrDefault().Url.AbsoluteUri ?? imgNotAvailable, item.Files.Mp4_240 != null ? item.Files.Mp4_240.AbsoluteUri : imgNotAvailable);
                AttachmentsList.Items.Add(attachment);
            }
        }

        private void AudioShow()
        {
            AttachmentsList.Items.Clear();

            var getHistoryAttachments = GettingHistoryAttachments(VkNet.Enums.SafetyEnums.MediaType.Audio);

            foreach (var audio in getHistoryAttachments)
            {
                var aud = audio.Attachment.Instance as VkNet.Model.Attachments.Audio;
                // Если ссылка указывает на файл типа m3u8, переводим её в ссылку mp3
                string url = aud.Url.AbsoluteUri.Contains(".mp3") ? aud.Url.AbsoluteUri : M3U8ToMp3(aud.Url.AbsoluteUri);

                var preview = new AudioCover() { Photo300 = imgNotAvailable };

                try { preview = aud.Album.Thumb; } catch { }
                var attachment = new VkAttachment.VkAudio(aud.Title, url, preview);
                AttachmentsList.Items.Add(attachment);
            }
        }

        private ReadOnlyCollection<HistoryAttachment> GettingHistoryAttachments(VkNet.Enums.SafetyEnums.MediaType type)
        {
            return api.Messages.GetHistoryAttachments(new MessagesGetHistoryAttachmentsParams
            {
                MediaType = type,
                PeerId = Convert.ToInt64(Conversation.ID),
                Count = Convert.ToInt32(NumberOfAttach.Text)
            }, out string str);
        }

        private string M3U8ToMp3(string url)
        {
            // Метод перевода m3u8 ссылки на mp3 ссылку
            int ind = url.IndexOf("/index.m3u8");
            url = url.Replace("/index.m3u8", ".mp3");
            int firstindex = url.LastIndexOf('/', ind);
            int secondindex = url.LastIndexOf('/', firstindex - 1);
            url = url.Remove(secondindex, firstindex - secondindex);
            return url;
        }

        private void AttachmentsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AttachmentsList.SelectedIndex != -1)
            {
                var attachment = AttachmentsList.SelectedItem as VkAttachment;
                img.Source = new BitmapImage(new Uri(attachment.PreviewUrl ?? imgNotAvailable));
            }
        }

        private void Autorization_Click(object sender, RoutedEventArgs e)
        {
            AuthorizationForm GettingToken = new AuthorizationForm();
            GettingToken.ShowDialog();
            Autorization(token);
        }

        private void DownloadAll_Click(object sender, RoutedEventArgs e)
        {
            if (!(AttachmentsList.Items[0] is VkAttachment.VkLink))
                DownloadAttachments(AttachmentsList.Items);
            else
                DownloadLinks(AttachmentsList.Items);
        }


        private void DownloadCurrent_Click(object sender, RoutedEventArgs e)
        {
            if (!(AttachmentsList.Items[0] is VkAttachment.VkLink))
                DownloadAttachments(AttachmentsList.SelectedItems);
            else
                DownloadLinks(AttachmentsList.SelectedItems);
        }

        private void DownloadAttachments(System.Collections.IList collection)
        {
            int i = 0;
            if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\" + Conversation.Title))
            {
                Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "\\" + Conversation.Title);
            }
            foreach (var item in collection)
            {
                try
                {
                    using (WebClient client = new WebClient())
                    {
                        var attachment = (item as VkAttachment);
                        client.DownloadFile(attachment.Url, AppDomain.CurrentDomain.BaseDirectory + "\\" + Conversation.Title + "\\" + attachment.NameOfDownloadedFile);
                    }
                }
                catch (Exception error)
                {
                    MessageBox.Show("Ошибка при загрузке вложения: " + error);
                }
                i++;
            }
        }

        private void DownloadLinks(System.Collections.IList collection)
        {
            int i = 0;
            if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\" + Conversation.Title))
            {
                Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "\\" + Conversation.Title);
            }
            string date = "" + DateTime.Now.Year + '-' + DateTime.Now.Month + '-' + DateTime.Now.Day;
            using (StreamWriter streamWriter = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "\\" + Conversation.Title + "\\" + date + "_VKLinks.txt"))
            {
                string saveString = "";
                foreach (var item in collection)
                {
                    saveString += (item as VkAttachment).Url + ';' + (item as VkAttachment).Title + '\n';
                }
                streamWriter.Write(saveString);
            }
        }

        private void ChoiseAttach()
        {
            switch (AttachmentType.SelectedIndex)
            {
                case 0:
                    PhotoShow();
                    break;
                case 1:
                    VideoShow();
                    break;
                case 2:
                    AudioShow();
                    break;
                case 3:
                    DocumentShow();
                    break;
                case 4:
                    LinkShow();
                    break;
            }
        }

        private void ShowAttachments_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ChoiseAttach();
                FourthStep.IsEnabled = true;
            }
            catch (Exception error)
            {
                MessageBox.Show("Ошибка открытия диалога: " + error);
                FourthStep.IsEnabled = false;
            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var window = new SelectConversationWindow(api, int.Parse(Count.Text));
                window.ShowDialog();
                ConvName.Text = Conversation.Title;
                ThirdStep.IsEnabled = true;
                ShowAttachments.IsEnabled = true;
            }
            catch
            {
                MessageBox.Show("Введите количество выводимых бесед");
            }
        }

    }
}
