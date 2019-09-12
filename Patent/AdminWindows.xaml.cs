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
    /// Логика взаимодействия для AdminWindows.xaml
    /// Основное окно для работников патентного бюро (администраторов)
    /// так же есть поиск по трем параметрам, никаких ограничений по просмотрк патентов
    /// в таблице добавляется дополнительная информация по патентам, их статус и ИД администратора что его курирует
    /// Два режима просмотра, все патенты и патенты требующие контроля
    /// </summary>
    public partial class AdminWindows : Window
    {
        SearchType type = SearchType.Number;
        Context db;
        Worker admin;
        List<Patent> patents;
        bool isOnlyWork = false;
        public AdminWindows(int adminId)
        {
            InitializeComponent();
            db = new Context();
            patents = new List<Patent>();
            admin = db.Workers.Find(adminId);
            this.Closing += UserWindows_Closing;

            patents.AddRange(db.Patents.ToList());
            AllPatentsGrid.ItemsSource = patents;
        }
        private void UserWindows_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            db.Dispose();
        }
        //Обычная выборка с небольшим отличием, нет ограничения по статусу 
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
                            patents = db.Patents.Where(x => x.PublicationId == number).ToList();
                        }
                        break;
                    case SearchType.Name:
                        patents = db.Patents.Where(x => x.Name.Contains(text)).ToList();
                        break;
                    case SearchType.Author:
                        patents = db.Patents.Where(x => x.Author.FIO.Contains(text)).ToList();
                        break;
                }
            }
            else
            {
                patents = db.Patents.ToList();
            }
            if (isOnlyWork) patents = patents.Where(x => x.Status != 5 && x.Status != 4).ToList();//условие для второго режима роботы, отображаются только те патенты, которые требуют работы с ними
            AllPatentsGrid.ItemsSource = patents;
        }

        private void SearchTypeCBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            type = (SearchType)SearchTypeCBox.SelectedIndex;
        }
    
        private void OnlyWorkMenu_Click(object sender, RoutedEventArgs e)
        {
            isOnlyWork = true;
            AllPatentsGrid.ItemsSource = db.Patents.Where(x => x.Status != 5 && x.Status != 4).ToList();
            SearchTypeCBox.SelectedIndex = 0;
            SearchText.Text = "";
        }

        private void AllPatentMenu_Click(object sender, RoutedEventArgs e)
        {
            isOnlyWork = false;
            AllPatentsGrid.ItemsSource = db.Patents.ToList();
            SearchTypeCBox.SelectedIndex = 0;
            SearchText.Text = "";
        }

        private void ExitBttn_Click(object sender, RoutedEventArgs e)
        {
            var win = new GlobalWindow();
            win.Show();
            this.Close();
        }
        //Получение доступу до выбраного патента от имени администратора
        private void SelectBttn_Click(object sender, RoutedEventArgs e)
        {
            int index = AllPatentsGrid.SelectedIndex;
            if (index > -1)
            {
                var win = new PatentWindow(PatentStatus.Admin, patents[index].Id, admin.Id, true);// задаем права администратора
                win.Show();
                this.Close();
            }
        }
    }
}
