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
using System.Windows.Shapes;

namespace FlowersStore
{
    /// <summary>
    /// Логика взаимодействия для Autorisation.xaml
    /// </summary>
    public partial class Autorisation : Window
    {
        FlowersStoreBDEntities BD = new FlowersStoreBDEntities();
        public Autorisation()
        {
            InitializeComponent();
        }

        private void SignInButton_Click(object sender, RoutedEventArgs e)
        {
            if (LoginTextBox.Text == "" || PasswordBox.Password == null)
            {
                MessageBox.Show("Для начала введите все данные");
            }
            else
            {
                var user = BD.User.Where(a => a.Login == LoginTextBox.Text && a.Password == PasswordBox.Password).FirstOrDefault();
                if(user != null)
                {
                    MainWindow MW = new MainWindow();
                    MW.Show();
                    Hide();
                }
                else
                {
                    MessageBox.Show("Не удалось найти такого пользователя");
                }
            }           
        }

        private void SignInButtonClient_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
