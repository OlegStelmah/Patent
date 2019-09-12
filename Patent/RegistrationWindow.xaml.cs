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
    /// Логика взаимодействия для RegistrationWindow.xaml
    /// Форма для регистрации, имеится 4 поря, 3 обязательны к заполнению, одно по желанию
    /// По мере заполнения каждого поля вызывается соответствующий ему метод из класса RegExpr для проверки валидности данных
    /// Если данные были заполнены верно цвет границы поля ввода остается синим, если же данные не валидны, цвет меняется на крассный и появляется уведомление.
    /// </summary>
    public partial class RegistrationWindow : Window
    {
        Context db;
        public RegistrationWindow()
        {
            InitializeComponent();
            db = new Context();

            this.Closing += RegistrationWindows_Closing;
        }

        private void RegistrationWindows_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            db.Dispose();
        }

        private void FIOText_TextChanged(object sender, TextChangedEventArgs e)
        {
            String text = FIOText.Text;
            if(text != "" && RegExpr.CheckFIO(text))
            {
                FIOText.BorderBrush = "#FFABADB3".ToBrush();
                FIOWarnLabel.Visibility = Visibility.Hidden;
                return;
            }
            FIOText.BorderBrush = "#FFB22323".ToBrush();
            FIOWarnLabel.Visibility = Visibility.Visible;
        }

        private void EmailText_TextChanged(object sender, TextChangedEventArgs e)
        {
            String text = EmailText.Text;
            if(text != "" && RegExpr.CheckEmail(text))
            {
                EmailText.BorderBrush = "#FFABADB3".ToBrush();
                EmailWarnLabel.Visibility = Visibility.Hidden;
                return;
            }
            EmailText.BorderBrush = "#FFB22323".ToBrush();
            EmailWarnLabel.Visibility = Visibility.Visible;
        }

        private void CardText_TextChanged(object sender, TextChangedEventArgs e)
        {
            String text = CardText.Text;
            if (text != "" && RegExpr.CheckCard(text))
            {
                CardText.BorderBrush = "#FFABADB3".ToBrush();
                CardWarnLabel.Visibility = Visibility.Hidden;
                return;
            }
            CardText.BorderBrush = "#FFB22323".ToBrush();
            CardWarnLabel.Visibility = Visibility.Visible;
        }
        private void PasswordText_PasswordChanged(object sender, RoutedEventArgs e)
        {
            String text = PasswordText.Password;
            if (text != "" && RegExpr.CheckPassword(text))
            {
                PasswordText.BorderBrush = "#FFABADB3".ToBrush();
                PasswordWarnLabel.Visibility = Visibility.Hidden;
                return;
            }
            PasswordText.BorderBrush = "#FFB22323".ToBrush();
            PasswordWarnLabel.Visibility = Visibility.Visible;
        }

        private void CancelBttn_Click(object sender, RoutedEventArgs e)
        {
            var wind = new MainWindow();
            wind.Show();
            this.Close();
        }

        private void ReadyBttn_Click(object sender, RoutedEventArgs e)
        {
            string fio = FIOText.Text;
            string email = EmailText.Text;
            string pass = PasswordText.Password;
            if(RegExpr.CheckFIO(fio) && RegExpr.CheckEmail(email) && RegExpr.CheckPassword(pass))
            {
                int sameEmail = db.Authors.Where(p => p.Mail == email).Count();
                if(sameEmail == 0)
                {
                    Author auth = new Author() { FIO = fio, Mail = email, Password = pass };
                    string card = CardText.Text;
                    if (RegExpr.CheckCard(card)) auth.Card = card;
                    db.Authors.Add(auth);
                    db.SaveChanges();
                    MessageBox.Show("Регистрация нового пользователя прошла успешно!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    var win = new UserWindow();
                    win.SetUser(db.Authors.First(x => x.Mail == auth.Mail && x.Password == auth.Password).Id);
                    win.Show();
                    this.Close();
                    return;
                }
                else
                {
                    MessageBox.Show($"Емейл {email} уже используется, введите новый или выполните вход", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                    EmailText.BorderBrush = "#FFB22323".ToBrush();
                    EmailWarnLabel.Visibility = Visibility.Visible;
                    return;
                }
            }
            MessageBox.Show("Проверьте корректность введенных данныхб следуйте инструкциям", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
        }


    }

    public static class Extensions
    {
        public static SolidColorBrush ToBrush(this string HexColorString)
        {
            return (SolidColorBrush)(new BrushConverter().ConvertFrom(HexColorString));
        }
    }
}
