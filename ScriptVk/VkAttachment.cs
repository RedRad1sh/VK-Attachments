using System;
using VkNet.Model;


namespace ScriptVk
{
    /// <summary>
    /// Вложение.
    /// </summary>
    public class VkAttachment
    {
        /// <summary>
        /// Название вложения.
        /// </summary>
        public string Title { get; private set; }

        /// <summary>
        /// Ссылка на вложение.
        /// </summary>
        public string Url { get; private set; }

        /// <summary>
        /// Имя скачиваемого файла.
        /// </summary>
        public string NameOfDownloadedFile { get; private set; }

        /// <summary>
        /// Расширение скачиваемого файла.
        /// </summary>
        public string Extension { get; private set; }

        /// <summary>
        /// Ссылка на превью.
        /// </summary>
        public string PreviewUrl { get; private set; }

    /// <summary>
    /// <para>Инициализирует экземпляр класса VkAttachment, представляющий собой вложение беседы.</para>
    /// </summary>
    /// <param name="title">Название вложения.</param>
    /// <param name="url">Ссылка на вложение.</param>
    /// <param name="extension">Расширение.</param>
    public VkAttachment(string title, string url, string extension)
        {
            Title = title;
            Url = url;
            Extension = extension;
            NameOfDownloadedFile = Title + Extension;
        }

        /// <summary>
        /// Фото.
        /// </summary>
        public class VkPhoto : VkAttachment
        {
            /// <summary>
            /// <para>Инициализирует экземпляр класса VkPhoto, наследующий VkAttachment.</para>
            /// </summary>
            /// <param name="title">Название вложения.</param>
            /// <param name="url">Ссылка на вложение.</param>
            /// <param name="extension">Расширение (по умолчанию для фото: ".jpg").</param>
            public VkPhoto(string title, string url, string extension = ".jpg") : base(title, url, extension)
            {
                PreviewUrl = url;
                ClearNameFromIllegalSymbols();
            }
        }

        /// <summary>
        /// Видео.
        /// </summary>
        public class VkVideo : VkAttachment
        {
            public VideoFiles VideoUrls;

            /// <summary>
            /// <para>Инициализирует экземпляр класса VkVideo, наследующий VkAttachment.</para>
            /// </summary>
            /// <param name="videoFiles">Соддержит в себе ссылки на видео.</param>
            /// <param name="previewUrl">Ссылка на превью.</param>
            /// <param name="title">Название вложения.</param>
            /// <param name="url">Ссылка на вложение.</param>
            /// <param name="extension">Расширение (по умолчанию для видео: ".mp4").</param>
            public VkVideo(string title, VideoFiles videoFiles, string previewUrl, string url, string extension = ".mp4") : base(title, url, extension)
            {
                VideoUrls = videoFiles;
                PreviewUrl = previewUrl;
                UpdateResolutionLinks();
                ClearNameFromIllegalSymbols();
            }

            /// <summary>
            /// Разрешение видео.
            /// </summary>
            public enum Resolution
            {
                Mp4_240,
                Mp4_360,
                Mp4_480,
                Mp4_720,
                Mp4_1080
            }

            /// <summary>
            /// Ссылки на видео разного разрешения.
            /// </summary>
            public Uri[] VideoQualityUrls = new Uri[5];

            /// <summary>
            /// Выбор разрешения для скачиваемого видео.
            /// </summary>
            /// <param name="res">Разрешение.</param>
            public void SelectResolution(Resolution res)
            {
                Url = VideoQualityUrls[(int)res] != null ? VideoQualityUrls[(int)res].AbsoluteUri : Url;
            }

            /// <summary>
            /// Получение ссылок на видео разного рарешения из объекта VideoFiles.
            /// </summary>
            private void UpdateResolutionLinks()
            {
                try {
                    VideoQualityUrls[0] = VideoUrls.Mp4_240;
                    VideoQualityUrls[1] = VideoUrls.Mp4_360;
                    VideoQualityUrls[2] = VideoUrls.Mp4_480;
                    VideoQualityUrls[3] = VideoUrls.Mp4_720;
                    VideoQualityUrls[4] = VideoUrls.Mp4_1080;

                    foreach (var item in VideoQualityUrls)
                    {
                        Url = item != null ? Url = item.ToString() : Url; 
                    }
                } catch { Url = null; }
            }
        }
        /// <summary>
        /// Аудио.
        /// </summary>
        public class VkAudio : VkAttachment
        {
            /// <summary>
            /// <para>Инициализирует экземпляр класса VkAudio, наследующий VkAttachment.</para>
            /// </summary>
            /// <param name="thumb">Объект обложки аудизоаписи.</param>
            /// <param name="title">Название вложения.</param>
            /// <param name="url">Ссылка на вложение.</param>
            /// <param name="extension">Расширение (по умолчанию для видео: ".mp4").</param>

            public VkAudio(string title, string url, AudioCover thumb, string extension = ".mp3") : base(title, url, extension)
            {
                PreviewUrl = thumb.Photo300;
                ClearNameFromIllegalSymbols();
            }
        }

        /// <summary>
        /// Ссылка.
        /// </summary>
        public class VkLink: VkAttachment
        {
            /// <summary>
            /// <para>Инициализирует экземпляр класса VkLink, наследующий VkAttachment.</para>
            /// </summary>
            /// <param name="title">Название ссылки</param>
            /// <param name="url">Ссылка.</param>
            /// <param name="extension"></param>

            public VkLink(string title, string url, string extension = ".txt") : base(title, url, extension)
            {
                ClearNameFromIllegalSymbols();
            }
        }


        /// <summary>
        /// Класс документов.
        /// </summary>
        public class VkDocument : VkAttachment
        {
            /// <summary>
            /// <para>Инициализирует экземпляр класса VkDocument, наследующий VkAttachment.</para>
            /// </summary>
            /// <param name="title">Название документа.</param>
            /// <param name="url">Ссылка на документ.</param>
            /// <param name="extension">Расширение документа.</param>
            /// <param name="previewUrl">Ссылка на превью.</param>
            public VkDocument(string title, string url, string extension, string previewUrl) : base(title, url, extension)
            {
                PreviewUrl = previewUrl;
                NameOfDownloadedFile = Title;
                ClearNameFromIllegalSymbols();
            }
        }

        public override string ToString()
        {
            return Title;
        }

        private const string IllegalSymbols = @"\/?:*""><|+";
        private void ClearNameFromIllegalSymbols()
        {
            foreach (var item in IllegalSymbols)
            {
                NameOfDownloadedFile = NameOfDownloadedFile.Replace(item.ToString(), "");
            }
        }
    }
}
