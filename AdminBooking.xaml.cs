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

namespace Pharmacy.Admin
{
    /// <summary>
    /// Interaction logic for AdminBooking.xaml
    /// </summary>
    public partial class AdminBooking : Window
    {
        public ObservableCollection<Reservation1> Reservations { get; set; }
        public ObservableCollection<Reservation1> SortedReservations { get; set; }
        public ObservableCollection<Reservation1> Orders { get; set; }

        public AdminBooking()
        {
            InitializeComponent();

            // Ініціалізуємо колекції
            Reservations = new ObservableCollection<Reservation1>();
            SortedReservations = new ObservableCollection<Reservation1>();
            Orders = new ObservableCollection<Reservation1>();

            // Завантажуємо дані з XML
            LoadReservationsFromXml();

            // Встановлюємо Reservations як джерело даних для membersDataGrid
            membersDataGrid.ItemsSource = Reservations;
        }

        private void LoadReservationsFromXml()
        {
            string filePath = "C:\\Users\\aroslavtolkunov\\source\\repos\\Pharmacy\\Pharmacy\\XMLFile2.xml";

            if (File.Exists(filePath))
            {
                try
                {
                    XDocument doc = XDocument.Load(filePath);
                    var reservationsElement = doc.Element("Reservations");

                    if (reservationsElement == null)
                    {
                        MessageBox.Show("Резервацій не знайдено.");
                        return;
                    }

                    var reservationElements = reservationsElement.Elements("Reservation");

                    if (!reservationElements.Any())
                    {
                        MessageBox.Show("Помилка.");
                        return;
                    }

                        Reservations = new ObservableCollection<Reservation1>(
                        from reservation in reservationElements
                        let status = (string)reservation.Attribute("Status")
                        orderby status == "Видано" ? 1 : 0, (DateTime)reservation.Attribute("ReservationDate") descending
                        select new Reservation1
                        {
                            Name = (string)reservation.Attribute("Name"), 
                            Num = (int)reservation.Attribute("Num"),
                            Number = (int)reservation.Attribute("Number"),
                            Quantity = (int)reservation.Attribute("Quantity"),
                            FormAmount = (int)reservation.Attribute("FormAmount"),
                            ReservationDate = (DateTime)reservation.Attribute("ReservationDate"),
                            TotalPrice = (decimal)reservation.Attribute("TotalPrice"),
                            Price = (decimal)reservation.Attribute("Price"),
                            DesiredPickupDate = (DateTime)reservation.Attribute("DesiredPickupDate"),
                            Type = (string)reservation.Attribute("Type"),
                            Phone = (string)reservation.Attribute("Phone"),
                            Character = (string)reservation.Attribute("Character"),
                            Email = (string)reservation.Attribute("Email"),
                            Position = (string)reservation.Attribute("Position"),
                            Status = (string)reservation.Attribute("Status")
                        }
                    );

                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Помилка: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("Помилка.");
            }
        }
        private void SortReservations_Click(object sender, RoutedEventArgs e)
        {
            // Сортування даних
            SortedReservations = new ObservableCollection<Reservation1>(
                Reservations
                .OrderByDescending(reservation => reservation.Status == "Зарезервовано")
                .ThenByDescending(reservation => reservation.ReservationDate)
                .ToList());

            // Оновлення даних в датагріді
            membersDataGrid.ItemsSource = SortedReservations;
        }
        private void SaveReservationsToXml(List<Reservation1> reservations)
        {
            string filePath = "C:\\Users\\aroslavtolkunov\\source\\repos\\Pharmacy\\Pharmacy\\XMLFile2.xml";

            // Використовуємо той самий об'єкт XDocument, який був використаний для завантаження даних
            XDocument doc = XDocument.Load(filePath);

            // Отримуємо елемент "Reservations"
            var reservationsElement = doc.Element("Reservations");

            // Очищаємо всі попередні дані
            reservationsElement?.RemoveNodes();

            // Додаємо нові дані
            foreach (var reservation in reservations)
            {
                reservationsElement?.Add(new XElement("Reservation",
                    new XAttribute("Number", reservation.Number),
                    new XAttribute("Character", reservation.Character),
                    new XAttribute("Name", reservation.Name),
                    new XAttribute("Position", reservation.Position),
                    new XAttribute("Email", reservation.Email),
                    new XAttribute("Type", reservation.Type),
                    new XAttribute("Phone", reservation.Phone),
                    new XAttribute("Price", reservation.Price),
                    new XAttribute("FormAmount", reservation.FormAmount),
                    new XAttribute("Num", reservation.Num),
                    new XAttribute("Quantity", reservation.Quantity),
                    new XAttribute("TotalPrice", reservation.TotalPrice),
                    new XAttribute("Status", reservation.Status),
                    new XAttribute("ReservationDate", reservation.ReservationDate),
                    new XAttribute("DesiredPickupDate", reservation.DesiredPickupDate)
                ));
            }

            // Зберігаємо зміни у тому самому файлі
            doc.Save(filePath);
        }
        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var reservation = button?.DataContext as Reservation1;

            if (reservation != null)
            {
                Edit editDialog = new Edit(reservation);
                if (editDialog.ShowDialog() == true)
                {
                    // Оновлення елементів списку Reservations
                    membersDataGrid.Items.Refresh();
                        SaveReservationsToXml(Reservations.ToList());
                }
            }
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

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Administration sign = new Administration();
            sign.Owner = this;

            this.Hide();

            sign.ShowDialog();

            this.Close();
            GC.Collect();
        }
        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            AdminMain sign = new AdminMain();
            sign.Owner = this;

            this.Hide();

            sign.ShowDialog();

            this.Close();
            GC.Collect();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            AdminBooking1 sign = new AdminBooking1();
            sign.Owner = this;

            this.Hide();

            sign.ShowDialog();

            this.Close();
            GC.Collect();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            AdminBooking2 sign = new AdminBooking2();
            sign.Owner = this;

            this.Hide();

            sign.ShowDialog();

            this.Close();
            GC.Collect();
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            AdminBooking3 sign = new AdminBooking3();
            sign.Owner = this;

            this.Hide();

            sign.ShowDialog();

            this.Close();
            GC.Collect();
        }
        private void TextBoxFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            string filterText = textBoxFilter.Text.ToLower();
            ICollectionView view = CollectionViewSource.GetDefaultView(membersDataGrid.ItemsSource);

            if (view != null)
            {
                view.Filter = item =>
                {
                    var reservation = item as Reservation1;
                    if (reservation == null) return false;

                    // Criteria for filtering as the user types
                    return reservation.Name.ToLower().Contains(filterText) ||
       reservation.Num.ToString().Contains(filterText) ||
       reservation.Status.ToLower().Contains(filterText);
                };

                view.Refresh();
            }
        }
        private void Button_Click_8(object sender, RoutedEventArgs e)
        {
            AdminEmployees sign = new AdminEmployees();
            sign.Owner = this;

            this.Hide();

            sign.ShowDialog();

            this.Close();
            GC.Collect();
        }
    }

    public class Reservation1
    {
        public int Number { get; set; }
        public string Phone { get; set; }
        public string Character { get; set; }
        public string Email { get; set; }
        public string Position { get; set; }
        public decimal Price { get; set; }
        public int Num { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public DateTime ReservationDate { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime DesiredPickupDate { get; set; }
        public string Status { get; set; }
        public string Type { get; set; }
        public int FormAmount { get; set; }
    }

}