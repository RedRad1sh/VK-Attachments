using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.HtmlControls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using mshtml;

namespace ScriptVk
{
    /// <summary>
    /// Логика взаимодействия для AuthorizationForm.xaml
    /// </summary>
    public partial class AuthorizationForm : Window
    {
        private string accesstoken;
        public string login;
        public string pass;
        private int numbrefr = 0;
        public AuthorizationForm()
        {
            InitializeComponent();
        }

        private void authorizationForm_Loaded(object sender, RoutedEventArgs e)
        {
            Browser.Navigate("https://oauth.vk.com/authorize?client_id=2685278&scope=1073737727&redirect_uri=https://api.vk.com/blank.html&display=page&response_type=token&revoke=1");

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
                    MainWindow.Instance.token = accesstoken;
                    MainWindow.Instance.DoneAuth.Visibility = Visibility.Visible;
                    Close();

                }
                else if (((HTMLDocument)Browser.Document).title == "ВКонтакте | Вход")
                {
                    ((HTMLDocument)Browser.Document).getElementById("email").setAttribute("value", login);
                    ((HTMLDocument)Browser.Document).getElementById("pass").setAttribute("value", pass);
                }
            }
            catch (Exception)
            {
                if (numbrefr > 5)
                {
                    MessageBox.Show("Возникла неизвестная ошибка при авторизации. Попробуйте перезапустить программу");
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
