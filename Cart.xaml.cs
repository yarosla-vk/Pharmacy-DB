using Pharmacy.MainWindow;
using Pharmacy.UseControls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using System.Xml.Linq;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using static Org.BouncyCastle.Asn1.Cmp.Challenge;
using Pharmacy.Admin;


namespace Pharmacy
{
    /// <summary>
    /// Interaction logic for Cart.xaml
    /// </summary>
    public partial class Cart : Window
    {
        private ObservableCollection<Member40> members = new ObservableCollection<Member40>();
        private readonly string cartFilePath = "C:\\Users\\aroslavtolkunov\\source\\repos\\Pharmacy\\Pharmacy\\XMLFile7.xml";
        private readonly string membersFilePath = "C:\\Users\\aroslavtolkunov\\source\\repos\\Pharmacy\\Pharmacy\\XMLFile6.xml";
        private readonly string ordersFilePath = "C:\\Users\\aroslavtolkunov\\source\\repos\\Pharmacy\\Pharmacy\\XMLFile5.xml";

        public Cart()
        {
            InitializeComponent();
            LoadMembersFromXml();
            membersDataGrid.ItemsSource = members;
            UpdateTotal();
        }

        private void LoadMembersFromXml()
        {
            if (!File.Exists(cartFilePath))
            {
                MessageBox.Show("Файл XMLFile7.xml не знайдено.");
                return;
            }

            try
            {
                XDocument doc = XDocument.Load(cartFilePath);
                var membersElement = doc.Element("Cart");

                if (membersElement == null)
                {
                    MessageBox.Show("Елемент 'Members' не знайдено у файлі XML.");
                    return;
                }

                var memberElements = membersElement.Elements("Member");
                if (!memberElements.Any())
                {
                    MessageBox.Show("Елементи 'Member' не знайдено у файлі XML.");
                    return;
                }

                members.Clear();
                foreach (var employee in memberElements)
                {
                    members.Add(new Member40
                    {
                        Number = (string)employee.Attribute("Number"),
                        Character = (string)employee.Attribute("Character"),
                        Name = (string)employee.Attribute("Name"),
                        Position = (string)employee.Attribute("Position"),
                        Email = (string)employee.Attribute("Email"),
                        Phone = (string)employee.Attribute("Phone"),
                        Price = (decimal)employee.Attribute("Price"),
                        Status = (string)employee.Attribute("Status"),
                        Quantity = (decimal?)employee.Attribute("Quantity") ?? 1
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при завантаженні XML: {ex.Message}");
            }
        }

        private void UpdateTotal()
        {
            totalTextBlock.Text = $"{CalculateTotal()} UAH";
        }

        private decimal CalculateTotal()
        {
            return members.Sum(member => member.Total);
        }

        private void IncreaseQuantity_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Member40 member)
            {
                member.Quantity++;
                UpdateMemberInXml(member);
                membersDataGrid.Items.Refresh();
                UpdateTotal();
            }
        }

        private void DecreaseQuantity_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Member40 member)
            {
                if (member.Quantity > 1)
                {
                    member.Quantity--;
                    UpdateMemberInXml(member);
                }
                else
                {
                    var result = MessageBox.Show("Ви впевнені, що хочете видалити цей товар?", "Підтвердження видалення", MessageBoxButton.YesNo);
                    if (result == MessageBoxResult.Yes)
                    {
                        members.Remove(member);
                        UpdateMemberInXml(member, true);
                    }
                }
                membersDataGrid.Items.Refresh();
                UpdateTotal();
            }
        }

        private void UpdateMemberInXml(Member40 member, bool delete = false)
        {
            if (!File.Exists(cartFilePath)) return;

            try
            {
                XDocument doc = XDocument.Load(cartFilePath);
                var memberElement = doc.Descendants("Member")
                                       .FirstOrDefault(x => (string)x.Attribute("Number") == member.Number);
                if (memberElement != null)
                {
                    if (delete)
                    {
                        memberElement.Remove();
                    }
                    else
                    {
                        memberElement.SetAttributeValue("Quantity", member.Quantity);
                    }
                    doc.Save(cartFilePath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при оновленні XML: {ex.Message}");
            }
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainWindow1 sign = new MainWindow1();
            sign.Owner = this;

            this.Hide();

            sign.Closed += (s, args) => this.Show();

            sign.ShowDialog();
            GC.Collect();
        }


        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(cardNumberTextBox.Text) || !IsCardNumberValid(cardNumberTextBox.Text))
                {
                    MessageBox.Show("Будь ласка, введіть номер картки.");
                    return;
                }

                // Оплата всіх товарів разом
                foreach (var member in members)
                {
                    // Оновлення кількості товарів на складі
                    // Додавання товару до замовлень
                    AddOrderToXml(member);
                }

                // Очищення даних у кошику після оплати
                members.Clear();
                membersDataGrid.ItemsSource = null;
                UpdateTotal();

                MessageBox.Show("Оплата всіх товарів успішно проведена і дані переміщено до замовлення.");
                try
                {
                    XDocument doc = XDocument.Load(cartFilePath);
                    doc.Root.Elements().Remove(); // Видаляємо всі елементи з кореневого елемента
                    doc.Save(cartFilePath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Помилка при очищенні файлу: {ex.Message}");
                }


                var mainWindow = new MainWindow1 { Owner = this };
                Hide();
                mainWindow.ShowDialog();
                Close();
                GC.Collect();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при обробці платежу: {ex.Message}");
            }
        }




        // Метод для додавання товару до замовлення
        private void AddOrderToXml(Member40 member)
        {
            if (!File.Exists(ordersFilePath))
            {
                MessageBox.Show("Файл XMLFile5.xml не знайдено.");
                return;
            }

            try
            {
                // Перевірка на null для всіх властивостей Member40
                if (member == null || string.IsNullOrEmpty(member.Number) || string.IsNullOrEmpty(member.Name) ||
                    string.IsNullOrEmpty(member.Price.ToString()) || string.IsNullOrEmpty(member.Quantity.ToString()))
                {
                    MessageBox.Show("Помилка: Недостатньо даних для додавання ордеру до XML.");
                    return;
                }

                XDocument ordersDoc = XDocument.Load(ordersFilePath);
                var ordersElement = ordersDoc.Root.Element("Members") ?? new XElement("Members");

                ordersElement.Add(new XElement("Member",
                    new XAttribute("Number", member.Number),
                    new XAttribute("Character", member.Character ?? ""),
                    new XAttribute("Name", member.Name),
                    new XAttribute("Position", member.Position ?? ""),
                    new XAttribute("Status", member.Status ?? ""),
                    new XAttribute("Email", member.Email ?? ""),
                    new XAttribute("Phone", member.Phone ?? ""),
                    new XAttribute("Price", member.Price),
                    new XAttribute("Quantity", member.Quantity)));

                ordersDoc.Save(ordersFilePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при додаванні ордеру до XML: {ex.Message}");
            }
        }


        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateButtonState();
        }

        private void UpdateButtonState()
        {
            paymentButton.IsEnabled = IsCardNumberValid(cardNumberTextBox.Text) &&
                                      IsExpireDateValid(expireDateTextBox.Text) &&
                                      IsCVVValid(cvvTextBox.Text);
        }

        private bool IsCardNumberValid(string cardNumber)
        {
            // Add card number validation logic here
            return !string.IsNullOrWhiteSpace(cardNumber);
        }

        private bool IsExpireDateValid(string expireDate)
        {
            return Regex.IsMatch(expireDate, @"^(0[1-9]|1[0-2])\/\d{2}$");
        }

        private bool IsCVVValid(string cvv)
        {
            return Regex.IsMatch(cvv, @"^\d{3}$");
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
            GC.Collect();
        }

        private void MainMenuButton_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = new MainWindow1 { Owner = this };
            Hide();
            mainWindow.ShowDialog();
            Close();
            GC.Collect();
        }
    }

    public class Member40
    {
        // Оголошуємо змінну для зберігання випадкового числа для поля Numb
        private string randomNumber;

        public Member40()
        {
            // Генеруємо випадкове значення для поля Numb
            randomNumber = GenerateRandomNumber();
        }

        public string Character { get; set; }
        public string Numb { get { return randomNumber; } } // Поле Numb тепер буде повертати згенероване раніше значення
        public string Name { get; set; }
        public string Number { get; set; }
        public string Position { get; set; }
        public string Status { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public decimal Price { get; set; }
        public decimal Quantity { get; set; }
        public decimal Total => Price * Quantity;

        private string GenerateRandomNumber()
        {
            // Генеруємо випадкове число
            Random random = new Random();
            int randomNumber = random.Next(1000, 9999); // Генеруємо випадкове число від 1000 до 9999

            // Повертаємо значення в форматі рядка
            return randomNumber.ToString();
        }
    }
}