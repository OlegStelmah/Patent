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
    /// Логика взаимодействия для UserWindow.xaml
    /// Класс описывающий логику роботы окна поиска патентов для зарегестрированых пользователей
    /// По анологии с GlobalWindow так же имеет строку поиска по 3 параметрах
    /// Есть возможность переключиться на свои патенты, там отображаются все патенты конкретного пользователя
    /// с строкой статуса и возможностю доступа к ним
    /// Строка поиска становится не активна при переходе на режим "Мои патенты"
    /// </summary>
    
    public partial class UserWindow : Window
    {
        SearchType type = SearchType.Number;
        Context db;
        List<Patent> patents;
        Author currentUser;
        bool isOwn = false;
        public UserWindow()
        {
            InitializeComponent();
            db = new Context();
            patents = new List<Patent>();
            this.Closing += UserWindows_Closing;

            patents.AddRange(db.Patents.Where(x => x.Status == (int)Status.Published));
            PatentsGrid.ItemsSource = patents;
        }
        private void UserWindows_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            db.Dispose();
        }

        public void SetUser(int id)
        {
            currentUser = db.Authors.Find(id);
            UserNameRun.Text = currentUser.FIO;
            this.Title = currentUser.FIO;
        }

        private void SearchTypeCBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            type = (SearchType)SearchTypeCBox.SelectedIndex;
        }
        //Аналогично с таким же методом в GlobalWindow осуществляет механизм выборки
        private void SearchBttn_Click(object sender, RoutedEventArgs e)
        {
            string text = SearchText.Text;
            if (text != "")
            {
                switch (type)
                {
                    case SearchType.Number:
                        int number;
                        if (int.TryParse(text, out number))
                        {
                            patents = db.Patents.Where(x => x.PublicationId == number && x.Status == (int)Status.Published).ToList();
                        }
                        break;
                    case SearchType.Name:
                        patents = db.Patents.Where(x => x.Name.Contains(text) && x.Status == (int)Status.Published).ToList();
                        break;
                    case SearchType.Author:
                        patents = db.Patents.Where(x => x.Author.FIO.Contains(text) && x.Status == (int)Status.Published).ToList();
                        break;
                }
            }
            else
            {
                patents = db.Patents.Where(x => x.Status == (int)Status.Published).ToList();
            }
            PatentsGrid.ItemsSource = patents;
        }
        /// <summary>
        /// Метод отвественный за переключение между режимамы поиска "Все патенты" и "мои патенты"
        /// изменяется параметр отображения некоторых елементов, и текст кнопки
        /// </summary>
        private void OwnPatentMenu_Click(object sender, RoutedEventArgs e)
        {
            if (!isOwn)
            {
                SearchGrid.Visibility = Visibility.Hidden;
                PatentsGrid.Visibility = Visibility.Hidden;
                OwnPatentsGrid.Visibility = Visibility.Visible;
                OwnPatentMenu.Header = "Все патенты";

                patents = currentUser.Patents.ToList();//отображаются все патенты принадлежащие данному пользователю
                OwnPatentsGrid.ItemsSource = patents;
                isOwn = true;
            }
            else
            {
                SearchGrid.Visibility = Visibility.Visible;
                PatentsGrid.Visibility = Visibility.Visible;
                OwnPatentsGrid.Visibility = Visibility.Hidden;
                OwnPatentMenu.Header = "Мои патенты";

                patents = db.Patents.Where(x => x.Status == (int)Status.Published).ToList();//стандартная выборка все опубликованные патенты
                OwnPatentsGrid.ItemsSource = patents;
                SearchTypeCBox.SelectedIndex = 0;
                SearchText.Text = "";
                isOwn = false;
            }
        }

        private void ExitBttn_Click(object sender, RoutedEventArgs e)
        {
            var win = new GlobalWindow();
            win.Show();
            this.Close();
        }

        private void NewPatentBttn_Click(object sender, RoutedEventArgs e)
        {
            var win = new PatentWindow(PatentStatus.NewPatent, null, currentUser.Id);
            win.Show();
            this.Close();
        }
        /// <summary>
        /// Метод реализующий два возможных перехода к окну PatentWindow 
        /// </summary>
        private void SelectBttn_Click(object sender, RoutedEventArgs e)
        {
            if(isOwn)//Первая возможность, открытие патента автором которого и является пользователь
            {
                int index = OwnPatentsGrid.SelectedIndex;
                if (index > -1)
                {
                    var win = new PatentWindow(PatentStatus.Owner, patents[index].Id, currentUser.Id);//Передаваемые параметры определяют права доступа до патента
                    win.Show();
                    this.Close();
                }
            }
            else//либо же открытие патента как гость
            {
                int index = PatentsGrid.SelectedIndex;
                if(index > -1)
                {
                    var win = new PatentWindow(PatentStatus.Guest, patents[index].Id);
                    win.Show();
                }
            }
        }
    }
}
