﻿using System;
using System.Windows;
using System.Windows.Media;
using mshtml;

namespace ScriptVk
{
    /// <summary>
    /// Логика взаимодействия для AuthorizationForm.xaml
    /// </summary>
    public partial class AuthorizationForm : Window
    {
        private string accesstoken;
        private int numbrefr = 0;
        public AuthorizationForm()
        {
            InitializeComponent();
        }

        private void authorizationForm_Loaded(object sender, RoutedEventArgs e)
        {
            Browser.Navigate("https://oauth.vk.com/authorize?client_id=6146827&scope=4096&redirect_uri=https://oauth.vk.com/blank.html&display=page&response_type=token&revoke=1");

        }
        private void LoadAuthCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            try
            {
                if (((HTMLDocument)Browser.Document).title == "OAuth Blank")
                {
                    string URL = Browser.Source.ToString();
                    string token = (URL.Split('=')[1]).Split('&')[0];
                    accesstoken = token;
                    MainWindow.Instance.Token = accesstoken;
                    MainWindow.Instance.DoneAuth.Visibility = Visibility.Visible;
                    MainWindow.Instance.DoneAuth.Foreground = Brushes.DarkGreen;
                    MainWindow.Instance.DoneAuth.Text = "Успешно";
                    Close();

                }
            }
            catch (Exception error)
            {
                if (numbrefr > 5)
                {
                    MainWindow.Instance.DoneAuth.Visibility = Visibility.Visible;
                    MainWindow.Instance.DoneAuth.Foreground = Brushes.DarkRed;
                    MainWindow.Instance.DoneAuth.Text = "Ошибка";
                    MessageBox.Show("Возникла ошибка при авторизации: " + error);
                    Close();
                }
                else
                {
                    Browser.Refresh();
                    numbrefr++;
                }
            }
            
        }
    }
}
