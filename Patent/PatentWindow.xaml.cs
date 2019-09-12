using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
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
    /// Логика взаимодействия для PatentWindow.xaml
    /// Основное окно взаимодействия, редактирования, добавления патента
    /// Инициализируется в одном из четырех режимов: режим нового патента, режим редактирования автором
    /// режим работы администратора и гостевой режим просмотра содержимого 
    /// Основное окно имеет четыре вкладки, три для всех режимов доступа (но с разной доступностью полей):библиографические данные (основные данные патента, его описание),
    /// вкладка документов, дает возможность добавлять и удалять документы привязанные к патенту, вкладка схем, дает возможность добавлять, удалять и просматривать привязанные к патенту схемы
    /// Так же есть вкладка публикации куда имеют доступ только автор и администратор, там находится панель оплаты и панель администрирования
    /// </summary>
    public enum PatentStatus
    {
        NewPatent,
        Owner,
        Admin,
        Guest
    }
    public partial class PatentWindow : Window
    {
        PatentStatus status;
        Context db;
        Patent patent;
        Author author;
        Scheme currentScheme;
        Type currentType;
        Worker admin;
        List<Document> docs;
        List<Scheme> schemes;
        List<Document> docToAdd;
        List<Scheme> schToAdd;
        List<Type> types;
        public PatentWindow(PatentStatus st, int? patentId = null, int? userId = null, bool isAdmin= false)
        {
            InitializeComponent();
            status = st;
            db = new Context();
            this.Closing += PatentWindows_Closing;

            if (patentId != null) patent = db.Patents.Find(patentId);
            else patent = new Patent();

            if (isAdmin && userId != null) { admin = db.Workers.Find(userId); author = patent.Author;  }
            else if (userId != null) author = db.Authors.Find(userId);
            

            

            switch (status)// поля инициализируються по разному для разных видов доступа
            {
                case PatentStatus.NewPatent:
                    LoadNewPatent();
                    docToAdd = new List<Document>();
                    schToAdd = new List<Scheme>();
                    break;
                default:
                    LoadExistPatent();
                    LoadDocs(patent.Documents.ToList());
                    LoadSchemes();
                    break;
            }
        }
        private void PatentWindows_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            db.Dispose();
        }
        //Метод настраюющий поля в соответствие с типом доступа "новый патент" в основном обнуляются все поля
        private void LoadNewPatent()
        {
            PattentNumber.Content = "";
            PatentIDLabel.Content = "";
            PatentIDLabel.Content = "";

            patent.ApplicationDate = DateTime.Now;
            PatentDateLabel.Content = patent.ApplicationDate.ToShortDateString();
            FIOText.Text = author.FIO;

            PatentNameText.Text = "";
            PatentNameText.IsReadOnly = false;

            DescriptionRun.Text = "";
            DescriptionText.IsReadOnly = false;

            AddDocBttn.Visibility = Visibility.Visible;
            DelDocBttn.Visibility = Visibility.Visible;

            AddImgBttn.Visibility = Visibility.Visible;
            SetStatus(Status.NotSent);
        }
        // загружает данные с базы даных и заполняет ими поля в соотвествии с даными существующего патента
        private void LoadExistPatent()
        {
            PattentNumber.Content = patent.PublicationId;
            PatentIDLabel.Content = patent.Id;
            PatentPublLabel.Content = patent.PublicationId;

            PatentDateLabel.Content = patent.ApplicationDate.ToShortDateString();
            PatentPublDateLabel.Content = patent.PatentDate.HasValue ? patent.PatentDate.Value.ToShortDateString() :"";//если необязательное свойство хранит значение то присваиваем его, если же оно пустое то обнуляем

            FIOText.Text = patent.AuthorName;
            PatentNameText.Text = patent.Name;

            DescriptionRun.Text = patent.Description;
            SetStatus((Status)patent.Status);

            if (status != PatentStatus.Guest)//Настройка доступа к полям в режиме "Гость"
            {
                AddDocBttn.Visibility = Visibility.Visible;
                DelDocBttn.Visibility = Visibility.Visible;

                AddImgBttn.Visibility = Visibility.Visible;
                DelImgBttn.Visibility = Visibility.Visible;

                DescriptionText.IsReadOnly = false;
                PatentNameText.IsReadOnly = false;
                PatentStatusLabel.Content = patent.StatusStr;
            }
            else PublicationTab.Visibility = Visibility.Hidden;

            if (status == PatentStatus.Admin) AdminBox.Visibility = Visibility.Visible;// делвет доступным панель администратора
            if (status == PatentStatus.Owner && patent.Status == (int)Status.WaitingForPayment) LoadPayment();// если патент в состоянии "ожидается оплата" то активируется панель оплаты
            if (patent.Payment) { StatusPaymentLabel.Content = "Оплачено";  StatusPaymentLabel.Foreground = "#FF1DBD15".ToBrush(); PayLabel.Content = 0; }

        }
        // Настройка полей панели оплаты
        private void LoadPayment()
        {
            PayBox.IsEnabled = true;
            types = db.Types.ToList();
            if(patent.PublicationId.HasValue)
            {
                TypeBox.SelectedIndex = patent.PublicationId.Value;
                currentType = types[TypeBox.SelectedIndex];
                PayLabel.Content = currentType.Price;
                if(author.Card != "")
                {
                    CardNumberText.Text = author.Card;
                }
            }
        }
        //Метод связывания набора данных с таблицей документов
        private void LoadDocs(List<Document> source)
        {
            if(source.Count > 0)
            {
                docs = source;
                DocumentsGrid.ItemsSource = docs;
            }
            else
            {
                docs = new List<Document>();
            }
        }
        //Метод связывания набора данных с отображением схем
        private void LoadSchemes()
        {
            if(patent.Schemes.Count > 0)
            {
                schemes = patent.Schemes.ToList();
                currentScheme = schemes[0];
                ImagePanel.Source = LoadImage(currentScheme.Data);
                SchemeNameLabel.Content = currentScheme.Name;
            }
            else
            {
                schemes = new List<Scheme>();
                currentScheme = null;
                ImagePanel.Source = null;
                SchemeNameLabel.Content = "";
            }
        }
        //Метод для добавление нового документа
        private void AddDocBttn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog myDialog = new OpenFileDialog();//создание диалогового окна для выбора файла
            myDialog.Filter = "Документы(*.DOC;*.DOCX)|*.DOC;*.DOCX" + "|Все файлы (*.*)|*.* ";//формат загружаемого файла
            myDialog.CheckFileExists = true;
            if (myDialog.ShowDialog() == true)
            {
                string path = myDialog.FileName;
                FileInfo filePatent;
                byte[] docData = ReadFile(path, out filePatent);//вызов метода преобразования файла в масив байтов

                if (status != PatentStatus.NewPatent)
                {
                    Document doc = new Document() { Name = filePatent.Name, Data = docData, PatentId = patent.Id };
                    db.Documents.Add(doc);
                    db.SaveChanges();
                    LoadDocs(patent.Documents.ToList());
                }
                else //если режим доступа "новый патент" сохраняем добавленные документы в временный масив
                {
                    Document doc = new Document() { Name = filePatent.Name, Data = docData };
                    docToAdd.Add(doc);
                    LoadDocs(docToAdd);
                }
            }
        }
        //Аналогично документам, выбор и добавление новой схемы
        private void AddImgBttn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog myDialog = new OpenFileDialog();//создание диалогового окна для выбора картинки
            myDialog.Filter = "Image Files(*.BMP;*.JPG;*.GIF;*.PNG)|*.BMP;*.JPG;*.GIF;*.PNG|All files (*.*)|*.*";//формат загружаемой картинки
            myDialog.CheckFileExists = true;
            if (myDialog.ShowDialog() == true)
            {
                string path = myDialog.FileName;
                FileInfo filePatent;
                byte[] schemeData = ReadFile(path, out filePatent);

                if (status != PatentStatus.NewPatent)
                {
                    Scheme sch = new Scheme() { Name = filePatent.Name, Data = schemeData, PatentId = patent.Id };
                    db.Schemes.Add(sch);
                    db.SaveChanges();
                    LoadSchemes();
                }
                else
                {
                    Scheme sch = new Scheme() { Name = filePatent.Name, Data = schemeData };
                    schToAdd.Add(sch);
                    ImagePanel.Source = LoadImage(sch.Data);
                    SchemeNameLabel.Content = sch.Name;
                }
            }
        }

        //Превращает выбранный файл в массив байтов
        private Byte[] ReadFile(string path, out FileInfo file)
        {
            if (File.Exists(path))
            {
                byte[] fileData = null;
                file = new FileInfo(path);
                using (BinaryReader reader = new BinaryReader(File.Open(path, FileMode.Open)))
                {
                    fileData = reader.ReadBytes((int)file.Length);//побайтовое считывание с файла и сохранение в масиве
                }
                return fileData;
            }
            file = null;
            return null;
        }
        // Метод позволяющий удалять документы 
        private void DelDocBttn_Click(object sender, RoutedEventArgs e)
        {
            int index = DocumentsGrid.SelectedIndex;
            if (index != -1 && (index < docs.Count || index < docToAdd.Count))
            {
                MessageBoxResult res = MessageBox.Show("Вы уверены что хотите удалить файл?", "Проверка", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if(res == MessageBoxResult.Yes)
                {
                    if (status != PatentStatus.NewPatent)
                    {
                        db.Documents.Remove(docs[index]);
                        db.Entry(docs[index]).State = EntityState.Deleted;
                        db.SaveChanges();
                        LoadDocs(patent.Documents.ToList());
                    }
                    else
                    {
                        docToAdd.Remove(docs[index]);
                        LoadDocs(docToAdd);
                    }
                }
            }
        }
        //Аналогично метод для удаления схем
        private void DelImgBttn_Click(object sender, RoutedEventArgs e)
        {
            if (currentScheme != null)
            {
                int index = schemes.IndexOf(currentScheme);
                MessageBoxResult res = MessageBox.Show($"Вы уверены что хотите удалить {currentScheme.Name}?", "Проверка", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (res == MessageBoxResult.Yes)
                {
                    db.Schemes.Remove(currentScheme);
                    db.Entry(currentScheme).State = EntityState.Deleted;
                    db.SaveChanges();
                    LoadSchemes();
                }
            }
        }
        //Метод преобраховубщий массив байтов в BitmapImage
        private BitmapImage LoadImage(byte[] imageData)
        {
            if (imageData == null || imageData.Length == 0) return null;
            var image = new BitmapImage();
            using (var mem = new MemoryStream(imageData))
            {
                mem.Position = 0;
                image.BeginInit();
                image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = null;
                image.StreamSource = mem;
                image.EndInit();
            }
            image.Freeze();
            return image;
        }
        //Переключение на следуйщуюю схему
        private void PrevImgBttn_Click(object sender, RoutedEventArgs e)
        {
            if(schemes.Count > 1 && currentScheme != null)
            {
                int currentIndex = schemes.IndexOf(currentScheme);
                if (currentIndex == 0) currentScheme = schemes[schemes.Count - 1];
                else currentScheme = schemes[currentIndex - 1];
                ImagePanel.Source = LoadImage(currentScheme.Data);
                SchemeNameLabel.Content = currentScheme.Name;
            }
        }
        //Переключение на предыдущую схему 
        private void NextImgBttn_Click(object sender, RoutedEventArgs e)
        {
            if (schemes.Count > 1 && currentScheme != null)
            {
                int currentIndex = schemes.IndexOf(currentScheme);
                if (currentIndex+1 == schemes.Count) currentScheme = schemes[0];
                else currentScheme = schemes[currentIndex + 1];
                ImagePanel.Source = LoadImage(currentScheme.Data);
                SchemeNameLabel.Content = currentScheme.Name;
            }
        }
        //Изменение типа публикации
        private void TypeBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(TypeBox.SelectedIndex != -1)
            {
                currentType = types[TypeBox.SelectedIndex];
                PayLabel.Content = currentType.Price;
            }
        }
        //Проверка на валидность вводимого номера карты
        private void CardNumberText_TextChanged(object sender, TextChangedEventArgs e)
        {
            String text = CardNumberText.Text;
            if (text != "" && RegExpr.CheckCard(text))
            {
                CardNumberText.BorderBrush = "#FFABADB3".ToBrush();
                return;
            }
            CardNumberText.BorderBrush = "#FFB22323".ToBrush();
        }
        //Проверка на валидность вводимого кода CVV
        private void CvvPass_PasswordChanged(object sender, RoutedEventArgs e)
        {
            String text = CvvPass.Password;
            if (text != "" && RegExpr.CheckCVV(text))
            {
                CvvPass.BorderBrush = "#FFABADB3".ToBrush();
                return;
            }
            CvvPass.BorderBrush = "#FFB22323".ToBrush();
        }
        //Метод симулирующий производство транзакций
        private void PayBttn_Click(object sender, RoutedEventArgs e)
        {
            if (TypeBox.SelectedIndex != -1)
            {
                String card = CardNumberText.Text;
                String pass = CvvPass.Password;
                if (RegExpr.CheckCard(card) && RegExpr.CheckCVV(pass))
                {
                    MessageBoxResult res = MessageBox.Show($"Вы действительно хотите оплатить {currentType.Price} грн, используя карту ****-****-****-{card.Substring(card.Length - 4)}?",
                        "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if(res == MessageBoxResult.Yes)
                    {
                        Payment pay = new Payment() { PatentId = patent.Id, Sum = currentType.Price };
                        db.Payments.Add(pay);
                        db.Patents.Find(patent.Id).TypeId = currentType.Id;
                        db.Authors.Find(author.Id).Card = card;
                        db.SaveChanges();
                        
                        MessageBox.Show("Транзакция успешно совершена, ожидайте обработки нашими специалистами", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                        PayBox.IsEnabled = false;
                    }
                }
            }
        }
        // Метод провиряющий оплату за публикацию патента
        private void CheckPaymentBttn_Click(object sender, RoutedEventArgs e)
        {
            if(patent.Status == (int)Status.WaitingForPayment)
            {
                if(patent.Payments.Count > 0)
                {
                    decimal sum = 0;
                    decimal cost = patent.Type.Price;
                    foreach(var p in patent.Payments)
                    {
                        sum += p.Sum;
                    }
                    if(sum >= cost)
                    {
                        db.Patents.Find(patent.Id).Payment = true;
                        db.SaveChanges();
                        MessageBox.Show("Оплата успешно получена, патент может быть опубликован", "Проверка успешна", MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }
                }
                else { MessageBox.Show("Оплата еще не получена, ожидаем транзакции", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning); return; }
            }
            MessageBox.Show("Проверка оплаты возможна только при статусе *ОЖИДАЕТ ОПЛАТЫ*, для проверки смените стутус публикации", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        // Метод проверяющий юир нормы патента
        private void CheckDataBttn_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Все данные соответствуют юридическим нормам", "Проверка успешна", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        // Метод изменения статуса патента через панель администратора
        private void SaveBttn_Click(object sender, RoutedEventArgs e)
        {
            int statusIndex = StatusBox.SelectedIndex;
            if(statusIndex == (int)Status.Published)
            {
                if(!db.Patents.Find(patent.Id).Payment)//Если публикация патента не оплачена, он не может быть опубликован
                {
                    MessageBox.Show("Патент не может быть подтвержден до получения оплаты", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    StatusBox.SelectedIndex = patent.Status;
                    return;
                }
                patent.PatentDate = DateTime.Now;
                patent.PublicationId = db.Patents.Where(x => x.Status == 4).ToList().Count();//Присвоение идентификатора публикации
            }
            patent.Status = StatusBox.SelectedIndex;
            patent.WorkerId = admin.Id;
            db.SaveChanges();
            MessageBox.Show("Статус успешно был изменен", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        //Метод присвоение статуса в зависимости от записи бд
        private void SetStatus(Status status)
        {
            StatusBox.SelectedIndex = (int)status;
            switch (status)
            {
                case Status.NotSent:
                    PatentStatusLabel.Content = "НЕ ОТПРАВЛЕНО";
                    return;
                case Status.UnderConsideration:
                    PatentStatusLabel.Content = "НА РАССМОТРЕНИИ";
                    return;
                case Status.DoesntMeetRequirements:
                    PatentStatusLabel.Content = "НЕ СООТВЕСТВУЕТ НОРМАМ";
                    return;
                case Status.WaitingForPayment:
                    PatentStatusLabel.Content = "ОЖИДАЕТ ОПЛАТЫ";
                    return;
                case Status.Published:
                    PatentStatusLabel.Content = "ОПУБЛИКОВАНО";
                    PatentStatusLabel.Foreground = "#FF1DBD15".ToBrush();
                    return;
                case Status.Canceled:
                    PatentStatusLabel.Content = "ОТКЛОНЕНО";
                    return;
            }
        }
        //Сохранение всех изменений
        private void SaveAllBttn_Click(object sender, RoutedEventArgs e)
        {
            
            patent.Name = PatentNameText.Text;
            patent.Description = DescriptionRun.Text;
            if(status == PatentStatus.NewPatent)// создание и сохранение нового екземпляра класса патент
            {
                patent.AuthorId = author.Id;
                patent.Status = 0;
                db.Patents.Add(patent);
                db.SaveChanges();
                int patentId = db.Patents.First(x => x.AuthorId == patent.AuthorId && x.Name == patent.Name).Id;//получение доступа к новосозданной строке в бд и получение автосгенерированого ключа

                foreach(var doc in docToAdd)// привязка всех документов из временного массива к созданному патенту
                {
                    doc.PatentId = patentId;
                    db.Documents.Add(doc);
                    db.SaveChanges();
                }

                foreach(var scheme in schToAdd)// привязка всех схем из временного массива к созданному патенту
                {
                    scheme.PatentId = patentId;
                    db.Schemes.Add(scheme);
                    db.SaveChanges();
                }
            }
            db.SaveChanges();

            if(status == PatentStatus.Admin) { var wind = new AdminWindows(admin.Id); wind.Show(); this.Close(); return; }//Возвращение в окно администратора

            var win = new UserWindow();//либо же в окно пользователя
            win.Show();
            win.SetUser(author.Id);
            this.Close();
        }
    }
}
