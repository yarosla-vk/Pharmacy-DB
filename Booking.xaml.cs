using Pharmacy.MainWindow;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.ComponentModel;
using System.Xml.Linq;
using System.Xml.Serialization;
using LiveCharts.Geared;
using Pharmacy.UseControls;
using System.Windows.Controls.Primitives;
using static Org.BouncyCastle.Asn1.Cmp.Challenge;
using System.Security.RightsManagement;

namespace Pharmacy
{
    public partial class Booking : Window
    {
        private Random random;

        // Поле класу для збереження останнього номера резервації
        private int lastReservationNumber;



        private const string ReservationsFilePath = "C:\\Users\\aroslavtolkunov\\source\\repos\\Pharmacy\\Pharmacy\\XMLFile2.xml";
        private const string MedicinesFilePath = "C:\\Users\\aroslavtolkunov\\source\\repos\\Pharmacy\\Pharmacy\\XMLFile6.xml";
        private const string ReservationsFilePath1 = "C:\\Users\\aroslavtolkunov\\source\\repos\\Pharmacy\\Pharmacy\\XMLFile1.xml";

        public Booking()
        {
            InitializeComponent();
            LoadMedicinesFromXml();
            Reservations = LoadReservationsFromXml(ReservationsFilePath1);
            ReservationListView.ItemsSource = Reservations;
            UpdateTotalPrice();

            random = new Random();

            // Генеруємо номер резервації при створенні об'єкту класу Booking
            lastReservationNumber = 123 + random.Next(1, 100000);
        }

        public ObservableCollection<Reservation> Reservations { get; set; }
        List<Medicine> Medicines = new List<Medicine>();

        private void LoadMedicinesFromXml()
        {
            if (File.Exists(MedicinesFilePath))
            {
                try
                {
                    XDocument doc = XDocument.Load(MedicinesFilePath);
                    var medicinesElement = doc.Element("Members");

                    if (medicinesElement == null)
                    {
                        Console.WriteLine("Error: 'Members' element not found in XML file.");
                        return;
                    }

                    Medicines = medicinesElement.Elements("Member")
                        .Select(m => new Medicine
                        {
                            Id = (int)m.Attribute("Number"),
                            Name = (string)m.Attribute("Name"),
                            FormInfo = (string)m.Attribute("Phone"),
                            Character = (string)m.Attribute("Character"),
                            Position = (string)m.Attribute("Position"),
                            Email = (string)m.Attribute("Email"),
                            FormAmount = (int)m.Attribute("FormAmount"),
                            FormPrice = (decimal)m.Attribute("Price"),
                            Type = (string)m.Attribute("Type")
                        }).ToList();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("Error: Medicines file not found.");
            }
        }

        private void ReserveButton_Click(object sender, RoutedEventArgs e)
        {
            string searchText = textBoxFilter.Text.ToLower();
            Medicine foundMedicine = Medicines.FirstOrDefault(m => m.Name.ToLower().Contains(searchText));

            if (foundMedicine == null)
            {
                Console.WriteLine("Select a medicine.");
                return;
            }

            if (!int.TryParse(QuantityTextBox.Text, out int quantity) || quantity <= 0)
            {
                Console.WriteLine("Enter a valid quantity.");
                return;
            }

            if (quantity > foundMedicine.FormAmount)
            {
                Console.WriteLine($"Only {foundMedicine.FormAmount} available.");
                return;
            }

            int totalQuantityForSelectedMedicine = Reservations.Where(r => r.Name == foundMedicine.Name).Sum(r => r.Quantity) + quantity;

            if (totalQuantityForSelectedMedicine > foundMedicine.FormAmount)
            {
                Console.WriteLine($"Only {foundMedicine.FormAmount} available.");
                return;
            }

            DateTime desiredPickupDate = DateTime.Now.Hour < 15 ? DateTime.Today : DateTime.Today.AddDays(1);
            decimal totalPrice = CalculateTotalPrice(foundMedicine.FormPrice, quantity);

            Reservation reservation = new Reservation
            {
                Number = foundMedicine.Id,
                Name = foundMedicine.Name,
                Type = foundMedicine.Type,
                Price = foundMedicine.FormPrice,
                Phone = foundMedicine.FormInfo,
                Character = foundMedicine.Character,
                Email = foundMedicine.Email,
                Position = foundMedicine.Position,
                Num = lastReservationNumber,
                Quantity = quantity,
                ReservationDate = DateTime.Now,
                DesiredPickupDate = desiredPickupDate,
                TotalPrice = totalPrice,
                Status = "Зарезервовано",
                FormAmount = foundMedicine.FormAmount
            };


            foundMedicine.FormAmount -= quantity;
            Reservations.Add(reservation);
            UpdateTotalPrice();
            Update();
            SaveReservationsToXml1(Reservations);
            SaveMedicinesToXml();
            MessageBox.Show("Товар успішно додано!", "Інформація", MessageBoxButton.OK, MessageBoxImage.Information);
            RefreshListView();
        }
        private void ReserveButton1_Click(object sender, RoutedEventArgs e)
        {
            // Load reservations from the first file
            var reservationsFromFirstFile = LoadReservationsFromXml(ReservationsFilePath1);


            foreach (var reservation in reservationsFromFirstFile)
            {
                if (!Reservations.Any(r => r.Name == reservation.Name && r.Quantity == reservation.Quantity && r.ReservationDate == reservation.ReservationDate))
                {
                    reservation.Number = lastReservationNumber;
                    Reservations.Add(reservation);
                }
            }

            SaveReservationsToXml(ReservationsFilePath, Reservations);


            reservationsFromFirstFile.Clear();

            SaveReservationsToXml1(reservationsFromFirstFile);
            MessageBox.Show($"Резервації успішно завершено! Ваш номер замовлення - {lastReservationNumber}", "Інформація", MessageBoxButton.OK, MessageBoxImage.Information);
            RefreshListView();
            Update();
        }

        private void SaveReservationsToXml1(ObservableCollection<Reservation> reservations)
        {
            try
            {
                XDocument doc;
                if (File.Exists(ReservationsFilePath1))
                {
                    doc = XDocument.Load(ReservationsFilePath1);
                    var reservationsElement = doc.Element("Reservations");

                    if (reservationsElement == null)
                    {
                        reservationsElement = new XElement("Reservations");
                        doc.Add(reservationsElement);
                    }
                    else
                    {
                        reservationsElement.RemoveAll();
                    }

                    foreach (var reservation in reservations)
                    {
                        reservationsElement.Add(new XElement("Reservation",
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
                }
                else
                {
                    doc = new XDocument(
                        new XElement("Reservations",
                            from reservation in reservations
                            select new XElement("Reservation",
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
                            )
                        )
                    );
                }

                doc.Save(ReservationsFilePath1);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при збереженні резервацій у файл {ReservationsFilePath1}: {ex.Message}");
            }
        }

        private void RefreshListView()
        {
            ReservationListView.ItemsSource = null;
            ReservationListView.ItemsSource = Reservations;
        }

        private void TextBoxFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = textBoxFilter.Text.ToLower();
            Medicine foundMedicine = Medicines.FirstOrDefault(m => m.Name.ToLower().Contains(searchText));

            if (foundMedicine != null)
            {
                Name.Text = foundMedicine.Name;
                // Assign other properties of the medicine to corresponding UI elements if needed
            }
            else
            {
                Name.Text = string.Empty;
            }
            Update();
        }


        private void UpdateTotalPrice()
        {
            decimal totalPrice = Reservations.Sum(reservation => reservation.TotalPrice);
            totalTextBlock.Text = totalPrice.ToString("C");
        }

        private void Update()
        {
            string searchText = textBoxFilter.Text.ToLower();
            Medicine foundMedicine = Medicines.FirstOrDefault(m => m.Name.ToLower().Contains(searchText));
            if (foundMedicine != null)
            {
                QuantityTextBox.Text = foundMedicine.FormAmount.ToString();
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateMedicinesFormAmount();
            SaveMedicinesToXml();
            // Очистити всю інформацію у файлі1
            Reservations.Clear();

            SaveReservationsToXml1(Reservations);
            UpdateTotalPrice();
            Update();

            // Оновити інтерфейс
            RefreshListView();
            MessageBox.Show("Всю інформацію було успішно видалено.", "Інформація", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        private void UpdateMedicinesFormAmount()
        {
            // Оновити кількість FormAmount у Medicines на основі даних у Reservations
            foreach (var reservation in Reservations)
            {
                var medicine = Medicines.FirstOrDefault(m => m.Name == reservation.Name);
                if (medicine != null)
                {
                    medicine.FormAmount += reservation.Quantity;
                }
            }
        }

        private decimal CalculateTotalPrice(decimal price, int quantity)
        {
            return price * quantity;
        }

        private void SaveMedicinesToXml()
        {
            try
            {
                XDocument doc;
                if (File.Exists(MedicinesFilePath))
                {
                    doc = XDocument.Load(MedicinesFilePath);
                    var medicinesElement = doc.Element("Members");

                    if (medicinesElement == null)
                    {
                        MessageBox.Show("Елемент 'Members' не знайдено у файлі XML.");
                        return;
                    }

                    // Оновлюємо інформацію про ліки в XML-файлі
                    foreach (var medicine in Medicines)
                    {
                        var medicineElement = medicinesElement.Elements("Member").FirstOrDefault(m => (string)m.Attribute("Name") == medicine.Name);
                        if (medicineElement != null)
                        {
                            medicineElement.Attribute("FormAmount").Value = medicine.FormAmount.ToString();
                        }
                    }
                }
                else
                {
                    doc = new XDocument(
                        new XElement("Members",
                            from medicine in Medicines
                            select new XElement("Member",
                                new XAttribute("Name", medicine.Name),
                                new XAttribute("Phone", medicine.FormInfo),
                                new XAttribute("FormAmount", medicine.FormAmount),
                                new XAttribute("Type", medicine.Type),
                                new XAttribute("Price", medicine.FormPrice)

                            )
                        )
                    );
                }

                doc.Save(MedicinesFilePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при збереженні даних про ліки у файл XML: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveReservationsToXml(string filePath, IEnumerable<Reservation> reservations)
        {
            try
            {
                XDocument doc;
                if (File.Exists(filePath))
                {
                    doc = XDocument.Load(filePath);
                    var reservationsElement = doc.Element("Reservations");

                    if (reservationsElement == null)
                    {
                        MessageBox.Show($"Елемент 'Reservations' не знайдено у файлі {filePath}.");
                        return;
                    }

                    foreach (var reservation in reservations)
                    {
                            reservationsElement.Add(new XElement("Reservation",
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
                }
                else
                {
                    doc = new XDocument(
                        new XElement("Reservations",
                            from reservation in reservations
                            select new XElement("Reservation",
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
                            )
                        )
                    );
                }

                doc.Save(filePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при збереженні резервацій у файл {filePath}: {ex.Message}");
            }
        }

        private ObservableCollection<Reservation> LoadReservationsFromXml(string filePath)
        {
            ObservableCollection<Reservation> loadedReservations = new ObservableCollection<Reservation>();

            if (File.Exists(filePath))
            {
                try
                {
                    XDocument doc = XDocument.Load(filePath);
                    var reservationsElement = doc.Element("Reservations");

                    var reservationElements = reservationsElement.Elements("Reservation");

                    loadedReservations = new ObservableCollection<Reservation>(
                        from reservation in reservationElements
                        select new Reservation
                        {
                            Name = (string)reservation.Attribute("Name"),
                            Num = (int)reservation.Attribute("Num"),
                            Number = (int)reservation.Attribute("Number"),
                            Quantity = (int)reservation.Attribute("Quantity"),
                            FormAmount = (int)reservation.Attribute("FormAmount"),
                            ReservationDate = (DateTime)reservation.Attribute("ReservationDate"),
                            TotalPrice = (decimal)reservation.Attribute("TotalPrice"),
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
                    MessageBox.Show($"Помилка при завантаженні резервацій: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("Файл резервацій не знайдено.");
            }

            return loadedReservations;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
            GC.Collect();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            MainWindow1 sign = new MainWindow1();
            sign.Owner = this;

            this.Hide();

            sign.Closed += (s, args) => this.Show();

            sign.ShowDialog();
            GC.Collect();
        }
        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null && textBox.Foreground == Brushes.LightGray)
            {
                textBox.Text = string.Empty; // Clear the placeholder text
                textBox.Foreground = Brushes.Black; // Change text color to black
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null && string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Foreground = Brushes.LightGray; // Change text color to light gray
                if (textBox == QuantityTextBox)
                {
                    textBox.Text = "Enter Quantity"; // Set placeholder text
                }
            }
        }
    }

    public class Reservation
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

    public class Medicine
    {
        public int Id { get; set; } 
        public string Name { get; set; } 
        public string FormInfo { get; set; } 
        public string Type { get; set; } 
        public int FormAmount { get; set; } 
        public string Character { get; set; } 
        public string Email { get; set; } 
        public string Position { get; set; } 
        public decimal FormPrice { get; set; }
        public override string ToString()
        {
            return Name;
        }
    }
}

