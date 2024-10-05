using Pharmacy.MainWindow;
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
using Pharmacy.Admin;

namespace Pharmacy
{
    public partial class UserInfo : Window
    {
        private MainWindow1 _previousWindow;

        ObservableCollection<Member30> members = new ObservableCollection<Member30>();

        public UserInfo()
        {

            InitializeComponent();
            LoadEmployeesFromXml();
            membersDataGrid.ItemsSource = members;

        }
        private void LoadEmployeesFromXml()
        {
            string filePath = "C:\\Users\\aroslavtolkunov\\source\\repos\\Pharmacy\\Pharmacy\\XMLFile4.xml";

            if (File.Exists(filePath))
            {
                XDocument doc = XDocument.Load(filePath);

                var employeesElement = doc.Element("Employees");
                if (employeesElement == null)
                {
                    MessageBox.Show("Елемент 'Employees' не знайдено у файлі XML.");
                    return;
                }

                var employeeElements = employeesElement.Elements("Employee");
                if (!employeeElements.Any())
                {
                    MessageBox.Show("Елементи 'Employee' не знайдено у файлі XML.");
                    return;
                }

                members = new ObservableCollection<Member30>(
    from employee in employeeElements
    where (string)employee.Attribute("Position") != "Посада"  // Assuming you have an attribute to mark clients
    select new Member30
    {
        Name = (string)employee.Attribute("Name"),
        Position = (string)employee.Attribute("Position"),
        Phone = (string)employee.Attribute("Phone"),
        Email = (string)employee.Attribute("Phone"),
        Address = (string)employee.Attribute("Address"),
        Sex = (string)employee.Attribute("Sex"),
        BirthDate = DateTime.Parse((string)employee.Attribute("BirthDate"))
    }
);

            }
            else
            {
                MessageBox.Show("Файл Employees.xml не знайдено.");
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainWindow1 mainWindow = new MainWindow1
            {
                Owner = this
            };

            this.Hide();
            mainWindow.ShowDialog();
            this.Close();
            GC.Collect();
        }

        private void RegistrationWindow_Closed(object sender, EventArgs e)
        {
            if (_previousWindow != null)
            {
                _previousWindow.Show();
                GC.Collect();
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Booking mainWindow = new Booking
            {
                Owner = this
            };

            this.Hide();
            mainWindow.ShowDialog();
            this.Close();
            GC.Collect();
        }
        private bool IsMaximize = false;
        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                if (IsMaximize)
                {
                    this.WindowState = WindowState.Normal;
                    this.Width = 1080;
                    this.Height = 720;

                    IsMaximize = false;
                }
                else
                {
                    this.WindowState = WindowState.Maximized;

                    IsMaximize = true;
                }
            }
        }
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            Cart mainWindow = new Cart
            {
                Owner = this
            };

            this.Hide();
            mainWindow.ShowDialog();
            this.Close();
            GC.Collect();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
            GC.Collect();
        }

        private void textBoxFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            string filterText = textBoxFilter.Text.ToLower();
            ICollectionView view = CollectionViewSource.GetDefaultView(membersDataGrid.ItemsSource);

            if (view != null)
            {
                view.Filter = item =>
                {
                    var member = item as Member30;
                    if (member == null) return false;

                    // Criteria for filtering as the user types
                    return member.Name.ToLower().Contains(filterText) ||
                           member.Position.ToLower().Contains(filterText) ||
                           member.Address.ToLower().Contains(filterText) ||
                           member.Email.ToLower().Contains(filterText) ||
                           member.Sex.ToLower().Contains(filterText) ||
                           member.Phone.ToLower().Contains(filterText);
                };

                view.Refresh();
            }
        }
    }
    public class Member30
    {
        public string Name { get; set; }
        public string Position { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Sex { get; set; }
        public string Email { get; set; }
        public DateTime BirthDate { get; set; }
    }
}