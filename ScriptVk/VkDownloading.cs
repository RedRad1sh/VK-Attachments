using System;
using System.IO;
using System.Net;
using System.Windows;

namespace ScriptVk
{
    static class VkDownloading
    {
        /// <summary>
        /// <para>Принимает коллекцию с ссылками на вложения и название беседы (название папки в директории программы). </para>
        /// <para>Загружает в папку программы все вложения из ссылок.</para>
        /// </summary>
        /// <param name="collection">Коллекция с ссылками на вложения.</param>
        /// <param name="nameOfConversation">Название беседы.</param>
        public static void DownloadAttachments(System.Collections.IList collection, string nameOfConversation)
        {
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
                }
                catch (Exception error)
                {
                    MessageBox.Show("Ошибка при загрузке вложения: " + error);
                }
            }
        }

        /// <summary>
        /// <para>Принимает коллекцию с ссылками которые необходимо записать в текстовый документ и название беседы (название папки в директории программы). </para>
        /// <para>Создаёт в папке беседы текстовый документ со всеми ссылками в форме [ссылка;название].</para>
        /// </summary>
        /// <param name="collection">Коллекция с ссылками.</param>
        /// <param name="nameOfConversation">Название беседы.</param>
        
        public static void DownloadLinks(System.Collections.IList collection, string nameOfConversation)
        {
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
