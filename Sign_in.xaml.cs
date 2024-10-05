using Pharmacy.MainWindow;
using Pharmacy.UseControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static Pharmacy.Log_in;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;
using System.IO;
using System.Xml.Linq;


namespace Pharmacy
{
    public partial class Sign_in : Window
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string Position { get; set; }
        public string Address { get; set; }
        public string Sex { get; set; }
        public string BirthDate { get; set; }
        public string Email { get; set; }

        public Sign_in()
        {
            InitializeComponent();
        }

        public Sign_in(Sign_in userInfo) : this()
        {
            FirstName = userInfo.FirstName;
            LastName = userInfo.LastName;
            Phone = userInfo.Phone;
            Position = userInfo.Position;
            Address = userInfo.Address;
            Email = userInfo.Email;
            Sex = userInfo.Sex;
            BirthDate = userInfo.BirthDate;
            // Додайте інші поля, якщо потрібно

            SetUserInfo();
            LoadEmployeeFromXml();
        }

        private void SetUserInfo()
        {
            FirstNameTextBox.TextBoxControl.Text = FirstName;
            LastNameTextBox.TextBoxControl.Text = LastName;
            PhoneNumberTextBox.TextBoxControl.Text = Phone;
            Password.TextBoxControl.Text = Position;
            AddressTextBox.TextBoxControl.Text = Address;
            EmailTextBox.TextBoxControl.Text = Email;
            SexTextBox.TextBoxControl.Text = Sex;
            BirthDateTextBox.TextBoxControl.Text = BirthDate;
        }

        private void LoadEmployeeFromXml()
        {
            string filePath = "C:\\Users\\aroslavtolkunov\\source\\repos\\Pharmacy\\Pharmacy\\XMLFile4.xml";

            try
            {
                if (File.Exists(filePath))
                {
                    XDocument doc = XDocument.Load(filePath);
                    var employeeElement = doc.Descendants("Employee").FirstOrDefault(emp => emp.Attribute("Email")?.Value == Email);

                    if (employeeElement != null)
                    {
                        FirstNameTextBox.TextBoxControl.Text = employeeElement.Attribute("Name").Value.Split(' ')[0];
                        LastNameTextBox.TextBoxControl.Text = employeeElement.Attribute("Name").Value.Split(' ')[1];
                        PhoneNumberTextBox.TextBoxControl.Text = employeeElement.Attribute("Phone").Value;
                        Password.TextBoxControl.Text = employeeElement.Attribute("Position").Value;
                        AddressTextBox.TextBoxControl.Text = employeeElement.Attribute("Address").Value;
                        EmailTextBox.TextBoxControl.Text = employeeElement.Attribute("Email").Value;
                        SexTextBox.TextBoxControl.Text = employeeElement.Attribute("Sex").Value;
                        BirthDateTextBox.TextBoxControl.Text = employeeElement.Attribute("BirthDate").Value;
                    }
                    else
                    {
                        System.Windows.MessageBox.Show("Дані не знайдено у файлі XML.");
                    }
                }
                else
                {
                    System.Windows.MessageBox.Show("Файл XMLFile4.xml не знайдено.");
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Помилка при завантаженні даних: {ex.Message}");
            }
        }
    




        private void Border_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left) this.DragMove();
        }

        private void Image_MouseUp(object sender, MouseButtonEventArgs e)
        {
            this.Close();
            GC.Collect();
        }

        private void Go_to_Main(object sender, RoutedEventArgs e)
        {
            MainWindow1 sign = new MainWindow1();
            sign.Owner = this;

            this.Hide();

            sign.ShowDialog();

            this.Close();
            GC.Collect();
        }

        private void RemovePlaceholderText(object sender, RoutedEventArgs e)
        {
            if (EmailTextBox.TextBoxControl.Text == "email@gmail.com")
            {
                EmailTextBox.TextBoxControl.Text = "";
                EmailTextBox.TextBoxControl.Foreground = Brushes.Black;
            }
        }

        private void AddPlaceholderText(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(EmailTextBox.TextBoxControl.Text))
            {
                EmailTextBox.TextBoxControl.Text = "email@gmail.com";
                EmailTextBox.TextBoxControl.Foreground = Brushes.Gray;
            }
        }

        private void UpdateEmailHint(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            string text = EmailTextBox.TextBoxControl.Text;
            if (!text.Contains("@") && !text.EndsWith("@gmail.com"))
            {
                EmailTextBox.TextBoxControl.Text = text + "@gmail.com";
                EmailTextBox.TextBoxControl.SelectionStart = text.Length;
                EmailTextBox.TextBoxControl.SelectionLength = 0;
                EmailTextBox.TextBoxControl.Foreground = Brushes.Gray;
            }
            else
            {
                EmailTextBox.TextBoxControl.Foreground = Brushes.Black;
            }
        }

        private void textEmail_MouseDown(object sender, MouseButtonEventArgs e)
        {
            EmailTextBox.TextBoxControl.Focus();
        }

        private void txtEmail_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(EmailTextBox.TextBoxControl.Text) && EmailTextBox.TextBoxControl.Text.Length > 0)
            {
                EmailTextBox.TextBoxControl.Visibility = Visibility.Collapsed;
            }
            else
            {
                EmailTextBox.TextBoxControl.Visibility = Visibility.Visible;
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            // Змінити колір тексту в TextBox на чорний та збільшити розмір шрифту на 2 після натискання кнопки "Зберегти"
            try
            {
                if (FirstNameTextBox.TextBoxControl.Text.Length >= 3)
                {
                    string firstName = char.ToUpper(FirstNameTextBox.TextBoxControl.Text[0]) + FirstNameTextBox.TextBoxControl.Text.Substring(1);
                    FirstNameTextBox.TextBoxControl.Text = firstName;
                    FirstNameTextBox.TextBoxControl.Foreground = Brushes.Black;
                }
            }
            catch (Exception ex)
            {
                // Обробка помилок, якщо вони виникли при роботі з іменем
                Console.WriteLine("Помилка під час обробки імені: " + ex.Message);
            }
            try
            {
                if (LastNameTextBox.TextBoxControl.Text.Length >= 3)
                {
                    string lastName = char.ToUpper(LastNameTextBox.TextBoxControl.Text[0]) + LastNameTextBox.TextBoxControl.Text.Substring(1);
                    LastNameTextBox.TextBoxControl.Text = lastName;
                    LastNameTextBox.TextBoxControl.Foreground = Brushes.Black;
                }
            }
            catch (Exception ex)
            {
                // Обробка помилок, якщо вони виникли при роботі з прізвищем
                Console.WriteLine("Помилка під час обробки прізвища: " + ex.Message);
            }
            AddressTextBox.TextBoxControl.Foreground = Brushes.Black;

            try
            {
                string birthDate = BirthDateTextBox.TextBoxControl.Text;

                // Перевірка, чи введено дату у форматі "00/00/0000"
                if (System.Text.RegularExpressions.Regex.IsMatch(birthDate, @"^\d{2}/\d{2}/\d{4}$"))
                {
                    BirthDateTextBox.TextBoxControl.Foreground = Brushes.Black;
                }
                else
                {
                    // Обробка помилки, якщо дата не у вказаному форматі
                    throw new Exception("Дата народження повинна бути у форматі '00/00/0000'.");
                }
            }
            catch (Exception ex)
            {
                // Обробка помилок, якщо вони виникли при роботі з датою народження
                Console.WriteLine("Помилка під час обробки дати народження: " + ex.Message);
            }

            try
            {
                if (EmailTextBox.TextBoxControl.Text.EndsWith("@gmail.com"))
                {
                    EmailTextBox.TextBoxControl.Foreground = Brushes.Black;
                }
            }
            catch (Exception ex)
            {
                // Обробка помилок, якщо вони виникли при роботі з електронною поштою
                Console.WriteLine("Помилка під час обробки електронної пошти: " + ex.Message);
            }
            try
            {
                if (PhoneNumberTextBox.TextBoxControl.Text.StartsWith("+380") && PhoneNumberTextBox.TextBoxControl.Text.Length == 13)
                {
                    PhoneNumberTextBox.TextBoxControl.Foreground = Brushes.Black;
                }
            }
            catch (Exception ex)
            {
                // Обробка помилок, якщо вони виникли при роботі з номером телефону
                Console.WriteLine("Помилка під час обробки номера телефону: " + ex.Message);
            }
            try
            {
                int digitCount = Password.TextBoxControl.Text.Count(char.IsDigit);
                if (digitCount >= 4 && Password.TextBoxControl.Text.Length >= 4)
                {
                    Password.TextBoxControl.Foreground = Brushes.Black;

                    // Приховати введений пароль
                    string maskedPassword = new string('*', Password.TextBoxControl.Text.Length);
                    Password.TextBoxControl.Text = maskedPassword;

                }
            }
            catch (Exception ex)
            {
                // Обробка помилок, якщо вони виникли при роботі з паролем
                Console.WriteLine("Помилка під час обробки пароля: " + ex.Message);
            }

            bool allFieldsBlack =
            FirstNameTextBox.TextBoxControl.Foreground == Brushes.Black &&
            LastNameTextBox.TextBoxControl.Foreground == Brushes.Black &&
            BirthDateTextBox.TextBoxControl.Foreground == Brushes.Black &&
               EmailTextBox.TextBoxControl.Foreground == Brushes.Black &&
               PhoneNumberTextBox.TextBoxControl.Foreground == Brushes.Black &&
            AddressTextBox.TextBoxControl.Foreground == Brushes.Black &&
            Password.TextBoxControl.Foreground == Brushes.Black;

            if (allFieldsBlack)
            {
                FirstNameTextBox.TextBoxControl.FontSize += 2;
                LastNameTextBox.TextBoxControl.FontSize += 2;
                BirthDateTextBox.TextBoxControl.FontSize += 2;
                EmailTextBox.TextBoxControl.FontSize += 2;
                PhoneNumberTextBox.TextBoxControl.FontSize += 2;
                AddressTextBox.TextBoxControl.FontSize += 2;
                Password.TextBoxControl.FontSize += 2;
            }

            SaveEmployeeToXml(); // Виклик методу для збереження даних у XML
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            FirstNameTextBox.TextBoxControl.Text = string.Empty;
            LastNameTextBox.TextBoxControl.Text = string.Empty;
            BirthDateTextBox.TextBoxControl.Text = string.Empty;
            EmailTextBox.TextBoxControl.Text = string.Empty;
            PhoneNumberTextBox.TextBoxControl.Text = string.Empty;
            AddressTextBox.TextBoxControl.Text = string.Empty;
            Password.TextBoxControl.Text = string.Empty;
        }

        private void SaveEmployeeToXml()
        {
            string filePath = "C:\\Users\\aroslavtolkunov\\source\\repos\\Pharmacy\\Pharmacy\\XMLFile4.xml";

            // Перевірка, чи існує файл
            XDocument doc;
            if (File.Exists(filePath))
            {
                doc = XDocument.Load(filePath);
            }
            else
            {
                doc = new XDocument(new XElement("Employees"));
            }

            XElement newEmployee = new XElement("Employee",
                new XAttribute("Name", $"{FirstNameTextBox.TextBoxControl.Text} {LastNameTextBox.TextBoxControl.Text}"),
                new XAttribute("Position", "Клієнт"),
                new XAttribute("Phone", PhoneNumberTextBox.TextBoxControl.Text),
                new XAttribute("Address", AddressTextBox.TextBoxControl.Text),
                new XAttribute("Sex", "Не визначено"), // Замініть "Не визначено" на відповідний атрибут, якщо є
                new XAttribute("BirthDate", BirthDateTextBox.TextBoxControl.Text)
            );

            // Додавання нового співробітника до кореневого елемента
            doc.Root.Add(newEmployee);

            // Збереження змін до файлу
            doc.Save(filePath);
        }
    }
}
