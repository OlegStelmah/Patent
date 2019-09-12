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

namespace PatentNS
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// Входная точка приложения, служит для подальшего перенаправления на другие окна
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void NewPattentBttn_Click(object sender, RoutedEventArgs e)
        {
            var win = new RegistrationWindow();
            win.Show();
            this.Close();
        }

        private void SearchBttn_Click(object sender, RoutedEventArgs e)
        {
            var win = new GlobalWindow();
            win.Show();
            this.Close();
        }

        private void LoginBttn_Click(object sender, RoutedEventArgs e)
        {
            var win = new LoginWindow();
            win.Show();
            this.Close();
        }
    }
}
