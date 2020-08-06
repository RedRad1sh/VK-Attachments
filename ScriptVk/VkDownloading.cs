using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ScriptVk
{
    static class VkDownloading
    {
        public static void DownloadAttachments(System.Collections.IList collection, string nameOfConversation)
        {
            int i = 0;
            string downloadDirectory = AppDomain.CurrentDomain.BaseDirectory + "\\" + nameOfConversation;
            if (!Directory.Exists(downloadDirectory))
            {
                Directory.CreateDirectory(downloadDirectory);
            }
            foreach (var item in collection)
            {
                try
                {
                    using (WebClient client = new WebClient())
                    {
                        var attachment = (item as VkAttachment);
                        client.DownloadFile(attachment.Url, downloadDirectory + "\\" + attachment.NameOfDownloadedFile);
                    }
                    i++;
                }
                catch (Exception error)
                {
                    MessageBox.Show("Ошибка при загрузке вложения: " + error);
                }
            }
        }

        public static void DownloadLinks(System.Collections.IList collection, string nameOfConversation)
        {
            int i = 0;
            string downloadDirectory = AppDomain.CurrentDomain.BaseDirectory + "\\" + nameOfConversation;
            if (!Directory.Exists(downloadDirectory))
            {
                Directory.CreateDirectory(downloadDirectory);
            }
            string date = "" + DateTime.Now.Year + '-' + DateTime.Now.Month + '-' + DateTime.Now.Day;
            using (StreamWriter streamWriter = new StreamWriter(downloadDirectory + "\\" + date + "_VKLinks.txt"))
            {
                string saveString = "";
                foreach (var item in collection)
                {
                    saveString += (item as VkAttachment).Url + ';' + (item as VkAttachment).Title + '\n';
                }
                streamWriter.Write(saveString);
            }
        }


    }
}
