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
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using System.Data;
using MongoDB.Driver;
using MongoDB.Bson;
using DnsClient;
using Pharmacy.MainWindow;
using System.Xml.Linq;
using System.IO;

namespace Pharmacy
{
    public partial class Log_in : Window
    {
        private Dictionary<string, string> validCredentials = new Dictionary<string, string>
        {
            { "diana@gmail.com", "1234" },
            { "lera@gmail.com", "1234" },
            { "kiril@gmail.com", "1234" },
            { "misha@gmail.com", "1234" }
        };

        public Log_in()
        {
            InitializeComponent();
            txtEmail.GotFocus += RemovePlaceholderText;
            txtEmail.LostFocus += AddPlaceholderText;
            txtEmail.TextChanged += UpdateEmailHint;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }


        private void RemovePlaceholderText(object sender, RoutedEventArgs e)
        {
            if (txtEmail.Text == "Пошта")
            {
                txtEmail.Text = "";
                txtEmail.Foreground = Brushes.Black;
            }
        }
        private void AddPlaceholderText(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                txtEmail.Text = "Пошта";
                txtEmail.Foreground = Brushes.Gray;
            }
        }
        private void UpdateEmailHint(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            string text = txtEmail.Text;
            if (!text.Contains("@") && !text.EndsWith("@gmail.com"))
            {
                txtEmail.Text = text + "@gmail.com";
                txtEmail.SelectionStart = text.Length;
                txtEmail.SelectionLength = 0;
                txtEmail.Foreground = Brushes.Gray;
            }
            else
            {
                txtEmail.Foreground = Brushes.Black;
            }
        }

        private void textEmail_MouseDown(object sender, MouseButtonEventArgs e)
        {
            txtEmail.Focus();
        }
        private void txtEmail_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(!string.IsNullOrEmpty(txtEmail.Text) && txtEmail.Text.Length > 0)
            {
            textEmail.Visibility = Visibility.Collapsed;
            }
            else
            {
                textEmail.Visibility= Visibility.Visible;
            }
        }
        private void textPassword_MouseDown(object sender, MouseButtonEventArgs e)
        {
            txtPassword.Focus();
        }
        private void txtPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtPassword.Password) && txtPassword.Password.Length > 0)
            {
                textPassword.Visibility = Visibility.Collapsed;
            }
            else
            {
                textPassword.Visibility = Visibility.Visible;
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string email = txtEmail.Text;
            string password = txtPassword.Password;
            if (email == "yaroslav@gmail.com" && password == "1234")
            {
                MessageBox.Show("Login as Admin successful!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                Administration main = new Administration();

                main.Owner = this;

                this.Hide();

                main.ShowDialog();

                this.Close();
                GC.Collect();
            }
            else if (validCredentials.ContainsKey(email) && validCredentials[email] == password)
            {
                MessageBox.Show("Login successful!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                MainWindow1 main = new MainWindow1();
                main.Owner = this;

                this.Hide();

                main.ShowDialog();

                this.Close();
                GC.Collect();
            }
            else
            {
                MessageBox.Show("Login failed. Please check your email and password.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }
        private void Image_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Application.Current.Shutdown();
            GC.Collect();
        }
        private void Sign_in_Click(object sender, RoutedEventArgs e)
        {
            string email = txtEmail.Text;
            string password = txtPassword.Password;
            if (validCredentials.ContainsKey(email) && validCredentials[email] == password)
            {
                // Отримання інформації про користувача з файлу XML
                Sign_in userInfo = GetUserInfoFromXML(email);

                if (userInfo != null)
                {
                    // Передача інформації про користувача до наступного вікна
                    Sign_in signInWindow = new Sign_in(userInfo); // Передача userInfo як аргумента конструктора Sign_in
                    signInWindow.Owner = this;
                    this.Hide();
                    signInWindow.ShowDialog();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("User information not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Login failed. Please check your email and password.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private Sign_in GetUserInfoFromXML(string email)
        {
            string filePath = "C:\\Users\\aroslavtolkunov\\source\\repos\\Pharmacy\\Pharmacy\\XMLFile4.xml";

            try
            {
                if (File.Exists(filePath))
                {
                    XDocument doc = XDocument.Load(filePath);
                    var employeeElement = doc.Descendants("Employee").FirstOrDefault(emp => emp.Attribute("Email")?.Value == email);

                    if (employeeElement != null)
                    {
                        // Отримання значень атрибутів з елемента Employee
                        string name = employeeElement.Attribute("Name")?.Value;
                        string phone = employeeElement.Attribute("Phone")?.Value;
                        string position = employeeElement.Attribute("Position")?.Value;
                        string address = employeeElement.Attribute("Address")?.Value;
                        string sex = employeeElement.Attribute("Sex")?.Value;
                        string birthDate = employeeElement.Attribute("BirthDate")?.Value;

                        // Розділення імені та прізвища
                        string[] nameParts = name?.Split(' ');
                        string firstName = nameParts?[0];
                        string lastName = nameParts?.Length > 1 ? nameParts[1] : "";

                        // Повернення об'єкту Sign_in з отриманими значеннями
                        return new Sign_in
                        {
                            FirstName = firstName,
                            LastName = lastName,
                            Phone = phone,
                            Position = position,
                            Address = address,
                            Sex = sex,
                            Email = email,
                            BirthDate = birthDate
                            // Додайте інші поля, якщо потрібно
                        };
                    }
                    else
                    {
                        MessageBox.Show("Дані не знайдено у файлі XML.");
                        return null;
                    }
                }
                else
                {
                    MessageBox.Show("Файл XMLFile4.xml не знайдено.");
                    return null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при завантаженні даних: {ex.Message}");
                return null;
            }
        }
    }
}
