using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using VkNet;
using VkNet.Model;
using VkNet.Enums.Filters;
using VkNet.Model.RequestParams;
using VkNet.AudioBypassService.Extensions;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using MahApps.Metro.Controls;
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
        private VkApi Api;
        public string Token;
        private readonly string ImgNotAvailable = AppDomain.CurrentDomain.BaseDirectory + "\\" + "No_Image_Available.jpg";
        /// <summary>
        /// Объект выбранной беседы.
        /// </summary>
        public VkConversation Conversation;
        private ObservableCollection<VkAttachment> attachmentList;

        public MainWindow()
        {
            InitializeComponent();
            Instance = this;
            VideoQualityChange.ItemsSource = Enum.GetValues(typeof(VkAttachment.VkVideo.Resolution));
            attachmentList = new ObservableCollection<VkAttachment>();
            AttachmentsList.ItemsSource = attachmentList;
        }

        private void Autorization_Click(object sender, RoutedEventArgs e)
        {
            AuthorizationForm GettingToken = new AuthorizationForm();
            GettingToken.ShowDialog();
            Autorization(Token);
        }

        public void Autorization(string token)
        {
            try
            {
                var service = new ServiceCollection();
                service.AddAudioBypass();
                Api = new VkApi(service);
                ulong appID = 2685278;
                Settings settings = Settings.All;
                Api.Authorize(new ApiAuthParams
                {
                    AccessToken = token,
                    ApplicationId = appID,
                    Settings = settings
                });
                Console.WriteLine(Api.Token);
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

        private ReadOnlyCollection<HistoryAttachment> GettingHistoryAttachments(VkNet.Enums.SafetyEnums.MediaType type)
        {
            return Api.Messages.GetHistoryAttachments(new MessagesGetHistoryAttachmentsParams
            {
                MediaType = type,
                PeerId = Convert.ToInt64(Conversation.ID),
                Count = Convert.ToInt32(NumberOfAttach.Text)
            }, out string str);
        }

        /// <summary>
        /// Метод перевода m3u8 ссылки в mp3 ссылку.
        /// </summary>
        /// <param name="url">Ссылка на m3u8 аудио.</param>
        /// <returns></returns>
        public string M3U8ToMp3(string url)
        {
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
                img.Source = new BitmapImage(new Uri(attachment.PreviewUrl ?? ImgNotAvailable));
            }
        }
        CancellationTokenSource cts = new CancellationTokenSource();
        private void ChoiseAttach()
        {
            QualityLabel.Visibility = Visibility.Hidden;
            VideoQualityChange.Visibility = Visibility.Hidden;
            cts.Dispose();
            cts = new CancellationTokenSource();
            switch (AttachmentType.SelectedIndex)
            {
                case 0:
                    PhotoShow();
                    break;
                case 1:
                    VideoShow();
                    QualityLabel.Visibility = Visibility.Visible;
                    VideoQualityChange.Visibility = Visibility.Visible;
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
                cts.Cancel();
                ChoiseAttach();
                FourthStep.IsEnabled = true;
            }
            catch (Exception error)
            {
                MessageBox.Show("Ошибка открытия диалога: " + error);
                FourthStep.IsEnabled = false;
            }

        }

        private void SelectConversationButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var window = new SelectConversationWindow(Api, int.Parse(Count.Text));
                window.ShowDialog();
                if (Conversation == null)
                {
                    throw new Exception();
                }
                ConvName.Text = Conversation.Title;
                ThirdStep.IsEnabled = true;
                ShowAttachments.IsEnabled = true;
            }
            catch
            {
                MessageBox.Show("Введите количество выводимых бесед");
            }
        }

        private void VideoQualityChange_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (AttachmentsList.Items[0] is VkAttachment.VkVideo && VideoQualityChange.SelectedIndex != -1)
                {
                    foreach (VkAttachment.VkVideo item in AttachmentsList.Items)
                    {
                        item.SelectResolution((VkAttachment.VkVideo.Resolution)VideoQualityChange.SelectedItem);
                    }
                }
            }
            catch (Exception error)
            {
                MessageBox.Show("Произошла непредвиденная ошибка: " + error);
            }
        }

        #region Методы вывода вложений

        class ListAttachment
        {
            public VkAttachment Attachment { get; set; }
            public string ImgSource { get; set; }
        }

        private void AddAttachment(VkAttachment attachment)
        {
            attachmentList.Add(attachment);
        }

        private void PhotoShow()
        {

            attachmentList.Clear();
            var getHistoryAttachments = GettingHistoryAttachments(VkNet.Enums.SafetyEnums.MediaType.Photo);
            Dispatcher.BeginInvoke((Action)(async () =>
            {
                foreach (var photo in getHistoryAttachments)
                {
                    if (cts.IsCancellationRequested)
                        return;
                    var photos = photo.Attachment.Instance as VkNet.Model.Attachments.Photo;
                    var url = photos.Sizes[photos.Sizes.Count - 1].Url.AbsoluteUri;
                    var attachment = new VkAttachment.VkPhoto(url.Split('/').LastOrDefault(), url);
                    AddAttachment(attachment);
                    await Task.Delay(1);
                }
            }));

        }

        private void DocumentShow()
        {
            AttachmentsList.Items.Clear();
            var getHistoryAttachments = GettingHistoryAttachments(VkNet.Enums.SafetyEnums.MediaType.Doc);

            Dispatcher.BeginInvoke((Action)(async () =>
            {
                foreach (var doc in getHistoryAttachments)
                {
                    VkNet.Model.Attachments.Document document;
                    if ((document = doc.Attachment.Instance as VkNet.Model.Attachments.Document) != null)
                    {
                        var preview = ImgNotAvailable;
                        try { preview = document.Preview != null ? document.Preview.Photo.Sizes[0].Src.AbsoluteUri : preview; } catch { }
                        var attachment = new VkAttachment.VkDocument(document.Title, document.Uri, "." + document.Ext, preview);
                        AddAttachment(attachment);
                    }
                }
                await Task.Delay(1);
            }));
        }

        private void LinkShow()
        {
            AttachmentsList.Items.Clear();
            var getHistoryAttachments = GettingHistoryAttachments(VkNet.Enums.SafetyEnums.MediaType.Link);

            Dispatcher.BeginInvoke((Action)(async () =>
            {
                foreach (var link in getHistoryAttachments)
                {
                    var links = link.Attachment.Instance as VkNet.Model.Attachments.Link;
                    var preview = ImgNotAvailable;
                    try { preview = links.Image; } catch { }
                    var attachment = new VkAttachment.VkLink(links.Title, links.Uri.AbsoluteUri, preview);
                    AddAttachment(attachment);
                    await Task.Delay(1);
                }
            }));
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
            var videos = Api.Video.Get(new VideoGetParams
            {
                Videos = vidlist
            });
            Dispatcher.BeginInvoke((Action)(async () =>
            {
                foreach (var item in videos)
                {
                    var attachment = new VkAttachment.VkVideo(item.Title, item.Files, item.Image.LastOrDefault().Url.AbsoluteUri ?? ImgNotAvailable, item.Files.Mp4_240 != null ? item.Files.Mp4_240.AbsoluteUri : ImgNotAvailable);
                    AddAttachment(attachment);
                    await Task.Delay(1);
                }
            }));
        }

        private void AudioShow()
        {
            AttachmentsList.Items.Clear();

            var getHistoryAttachments = GettingHistoryAttachments(VkNet.Enums.SafetyEnums.MediaType.Audio);
            Dispatcher.BeginInvoke((Action)(async () =>
            {
                foreach (var audio in getHistoryAttachments)
                {
                    try
                    {
                        var aud = audio.Attachment.Instance as VkNet.Model.Attachments.Audio;
                        // Если ссылка указывает на файл типа m3u8, переводим её в ссылку mp3
                        string url = aud.Url.AbsoluteUri.Contains(".mp3") ? aud.Url.AbsoluteUri : M3U8ToMp3(aud.Url.AbsoluteUri);

                        var preview = new AudioCover() { Photo300 = ImgNotAvailable };

                        try { preview = aud.Album != null ? aud.Album.Thumb : preview; } catch (Exception) { }
                        var attachment = new VkAttachment.VkAudio(aud.Title, url, preview);
                        AddAttachment(attachment);
                        await Task.Delay(1);
                    }
                    catch (Exception error)
                    {
                        cts.Cancel();
                        MessageBox.Show("Непредвиденная ошибка при загрузке аудио" + error);
                    }
                }
            }));
        }

        #endregion

        #region Методы скачивания вложений

        private async void DownloadAll_Click(object sender, RoutedEventArgs e)
        {
            var list = new List<VkAttachment>();
            foreach (VkAttachment item in AttachmentsList.Items)
            {
                list.Add(item);
            }
            await Task.Run(() =>
            {
                if (!(AttachmentsList.Items[0] is VkAttachment.VkLink))
                    VkDownloading.DownloadAttachments(list, Conversation.Title);
                else
                    VkDownloading.DownloadLinks(list, Conversation.Title);
            });

        }

        private async void DownloadCurrent_Click(object sender, RoutedEventArgs e)
        {
            var list = new List<VkAttachment>();
            foreach (VkAttachment item in AttachmentsList.SelectedItems)
            {
                list.Add(item);
            }
            await Task.Run(() =>
        {
            if (!(AttachmentsList.Items[0] is VkAttachment.VkLink))
                VkDownloading.DownloadAttachments(list, Conversation.Title);
            else
                VkDownloading.DownloadLinks(list, Conversation.Title);
        });
        }

        #endregion
    }
}
