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

namespace PatentNS
{
    /// <summary>
    /// Логика взаимодействия для LoginWindow.xaml
    /// Простая форма авторизации с двумя полями логин и пароль и двумя кнопками готово и отмена
    /// Сравниваються поля сначала с данными обычных пользователей, а потом и администраторов
    /// </summary>
    public partial class LoginWindow : Window
    {
        Context db;
        public LoginWindow()
        {
            InitializeComponent();
            db = new Context();
            this.Closing += GlobalWindows_Closing;
        }
        private void GlobalWindows_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            db.Dispose();
        }

        private void CancelBttn_Click(object sender, RoutedEventArgs e)
        {
            var win = new MainWindow();
            win.Show();
            this.Close();
        }

        private void LoginBttn_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginText.Text;
            string pass = PasswordText.Password;
            if(login != "" && pass != "")
            {
                Author user = db.Authors.FirstOrDefault(x => x.Mail == login && x.Password == pass);
                if(user != null)
                {
                    MessageBox.Show($"Добро пожаловать, {user.FIO}!", "Вход выполнен", MessageBoxButton.OK, MessageBoxImage.Information);
                    var win = new UserWindow();
                    win.Show();
                    win.SetUser(user.Id);
                    this.Close();
                    return;
                }
                Worker admin = db.Workers.FirstOrDefault(x => x.Login == login && x.Password == pass);
                if(admin != null)
                {
                    MessageBox.Show("Добро пожаловать, Admin!", "Вход выполнен", MessageBoxButton.OK, MessageBoxImage.Information);
                    var win = new AdminWindows(admin.Id);
                    win.Show();
                    this.Close();
                    return;
                }
            }
            MessageBox.Show("Данные были введены неверно", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }
}
