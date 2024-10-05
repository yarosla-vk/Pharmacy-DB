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
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Pharmacy
{
    public partial class EdirEm : Window
    {
        public Employee Employee { get; set; }

        public EdirEm(Employee employee)
        {
            InitializeComponent();
            Employee = employee;
            DataContext = Employee; // Bind employee data to the window
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Update the employee properties with the edited values
            Employee.Name = EmployeeNameTextBox.Text;
            Employee.Sex = EmployeeSexTextBox.Text;
            Employee.Phone = EmployeePhoneTextBox.Text;
            Employee.Address = EmployeeAddressTextBox.Text;
            Employee.Position = PositionTextBox.Text;
            if (EmployeeBirthDatePicker.SelectedDate.HasValue)
            {
                Employee.BirthDate = EmployeeBirthDatePicker.SelectedDate.Value;
            }
            else
            {
                System.Windows.MessageBox.Show("Оберіть правільну дату");
            }


            // Close the dialog
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            // Close the dialog without saving changes
            DialogResult = false;
            Close();
        }

        private void Image_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Close();
        }
    }
}

