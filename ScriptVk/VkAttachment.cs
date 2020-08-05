using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VkNet.Model;


namespace ScriptVk
{
    public class VkAttachment
    {
        public string Title;
        public string Url;
        public string NameOfDownloadedFile;
        public string Extension;
        private string IllegalSymbols = @"\/?:*""><|+";

        public string PreviewUrl = null;

        private void ClearNameFromIllegalSymbols()
        {
            foreach (var item in IllegalSymbols)
            {
                NameOfDownloadedFile = NameOfDownloadedFile.Replace(item.ToString(), "");
            }
        }
        
        public VkAttachment(string title, string url, string extension)
        {
            Title = title;
            Url = url;
            Extension = extension;
            NameOfDownloadedFile = Title + Extension;
        }

        public class VkPhoto : VkAttachment
        {
            public VkPhoto(string title, string url, string extension = ".jpg") : base(title, url, extension)
            {
                PreviewUrl = url;
                ClearNameFromIllegalSymbols();
            }
        }
        public class VkVideo : VkAttachment
        {
            public VideoFiles VideoUrls;


            public VkVideo(string title, VideoFiles videoFiles, string previewUrl, string url, string extension = ".mp4") : base(title, url, extension)
            {
                VideoUrls = videoFiles;
                PreviewUrl = previewUrl;
                UpdateResolutionLinks();
                ClearNameFromIllegalSymbols();
            }

            public enum Resolution
            {
                Mp4_240,
                Mp4_360,
                Mp4_480,
                Mp4_720,
                Mp4_108
            }

            public Uri[] VideoQualityUrls = new Uri[5];

            public void UpdateResolutionLinks()
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
        public class VkAudio : VkAttachment
        {
            public VkAudio(string title, string url, AudioCover thumb, string extension = ".mp3") : base(title, url, extension)
            {
                PreviewUrl = thumb.Photo300;
                ClearNameFromIllegalSymbols();
            }
        }
        public class VkLink: VkAttachment
        {
            public VkLink(string title, string url, string extension = ".txt") : base(title, url, extension)
            {
                ClearNameFromIllegalSymbols();
            }
        }
        public class VkDocument : VkAttachment
        {
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
    }
}
