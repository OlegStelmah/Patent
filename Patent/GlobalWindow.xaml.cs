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
    /// Перечисление что представляет вариации поиска
    /// </summary>
    public enum SearchType
    {
        Number,
        Name,
        Author
    }
    /// <summary>
    /// Перечисление определяющее состояния публикации патентов.
    /// Незарегестрированные пользователи не могут видет состояния патентов
    /// Так же авторы патентов могут видеть состояния только своих патентов
    /// Администраторам доступно состояния всех патентов.
    /// Состояния выставляются админимтратором в ручную для каждого патента.
    /// </summary>
    public enum Status
    {
        NotSent,
        UnderConsideration,
        DoesntMeetRequirements,
        WaitingForPayment,
        Published,
        Canceled
    }
    /// <summary>
    /// Логика взаимодействия для GlobalWindow.xaml
    /// ОСновное окно поиска патента для незарегестрированых пользователей
    /// Есть возможность поиска по трем параметрам по номеру, названию патента либо же по ФИО автора
    /// </summary>
    public partial class GlobalWindow : Window
    {
        SearchType type = SearchType.Number;
        Context db;
        List<Patent> patents;
        public GlobalWindow()
        {
            InitializeComponent();
            db = new Context();
            patents = new List<Patent>();
            this.Closing += GlobalWindows_Closing;

            patents.AddRange(db.Patents.Where(x => x.Status == (int)Status.Published));
            PatentsGrid.ItemsSource = patents;
        }
        private void GlobalWindows_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            db.Dispose();
        }

        private void RegestrationBttn_Click(object sender, RoutedEventArgs e)
        {
            var wind = new RegistrationWindow();
            wind.Show();
            this.Close();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            var win = new LoginWindow();
            win.Show();
            this.Close();
        }

        private void SerchTypeCBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            type = (SearchType)SerchTypeCBox.SelectedIndex;
        }
        /// <summary>
        /// Основной метод что осуществляет выборку, для реализации выборки были использованы LINQ запросы
        /// </summary>
        private void SearchBttn_Click(object sender, RoutedEventArgs e)
        {
            string text = SearchText.Text;
            if(text != "")
            {
                switch (type)
                {
                    case SearchType.Number:
                        int number;
                        if (int.TryParse(text, out number))//конвертации строки в инт по средством tryParse, провальная конвертация не вызывает исключений
                        {
                            patents = db.Patents.Where(x => x.PublicationId == number && x.Status == (int)Status.Published).ToList();//выбрать все патенты у которых статус Published и идентификатор совпадает с поисковым значением 
                        }
                        break;
                    case SearchType.Name:
                        patents = db.Patents.Where(x => x.Name.Contains(text) && x.Status == (int)Status.Published).ToList();//аналогично выбираются все патенты у которых в названии присутсвует поисковая подстрока
                        break;
                    case SearchType.Author:
                        patents = db.Patents.Where(x => x.Author.FIO.Contains(text) && x.Status == (int)Status.Published).ToList();//аналогично проверяет подстроку в ФИО
                        break;
                }
            }
            else
            {
                patents = db.Patents.Where(x => x.Status == (int)Status.Published).ToList();//первоначальнвя выборка, все опублицырованные патенты
            }
            PatentsGrid.ItemsSource = patents;
        }

        private void SelectBttn_Click(object sender, RoutedEventArgs e)
        {
            int index = PatentsGrid.SelectedIndex;
            if (index > -1)
            {
                var win = new PatentWindow(PatentStatus.Guest, patents[index].Id);
                win.Show();
                this.Close();
            }
        }
    }
}
