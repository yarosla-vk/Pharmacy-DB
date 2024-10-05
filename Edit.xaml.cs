using Pharmacy.Admin;
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

using System;
using System.Windows;


namespace Pharmacy
{
    /// <summary>
    /// Interaction logic for Edit.xaml
    /// </summary>
    public partial class Edit : Window
    {
        public Reservation1 Reservation { get; private set; }

        public Edit(Reservation1 reservation)
        {
            InitializeComponent();
            Reservation = reservation;

            // Populate the fields with existing data if editing
            DesiredPickupDatePicker.SelectedDate = Reservation.DesiredPickupDate;
            StatusTextBox.Text = Reservation.Status;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Validate and save the data
            if (DesiredPickupDatePicker.SelectedDate.HasValue)
            {
                Reservation.DesiredPickupDate = DesiredPickupDatePicker.SelectedDate.Value;
                Reservation.Status = StatusTextBox.Text;

                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show("Будь ласка, введіть коректну дату отримання.", "Помилка валідації", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
        private void Image_MouseUp(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }
    }
}