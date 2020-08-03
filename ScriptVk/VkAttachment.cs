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
                NameOfDownloadedFile = NameOfDownloadedFile.Replace(item, '\0');
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
                SelectResolution();
                ClearNameFromIllegalSymbols();
            }

            public enum Resolution
            {
                Mp4_240,
                Mp4_360,
                Mp4_480,
                Mp4_720,
                Mp4_1080
            }

            public void SelectResolution(Resolution res = Resolution.Mp4_240)
            {
                try
                {
                    switch (res)
                    {
                        case Resolution.Mp4_240:
                            Url = VideoUrls.Mp4_240.ToString();
                            break;
                        case Resolution.Mp4_360:
                            Url = VideoUrls.Mp4_360.ToString();
                            break;
                        case Resolution.Mp4_480:
                            Url = VideoUrls.Mp4_480.ToString();
                            break;
                        case Resolution.Mp4_720:
                            Url = VideoUrls.Mp4_720.ToString();
                            break;
                        case Resolution.Mp4_1080:
                            Url = VideoUrls.Mp4_1080.ToString();
                            break;
                    }
                } catch { Url = "empty"; }
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
