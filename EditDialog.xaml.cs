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
using Pharmacy.Admin;
using MessageBox = System.Windows.MessageBox;

using System.Xml.Linq;


namespace Pharmacy
{
    public partial class EditDialog : Window
    {
        private Member1 originalMember { get; set; }

        public EditDialog(Member1 member)
        {
            InitializeComponent();
            originalMember = member;
            DataContext = originalMember;

            if (originalMember == null)
            {
                MessageBox.Show("Помилка: Об'єкт Member не ініціалізований.");
                DialogResult = false;
                Close();
            }
            else
            {
                // Встановлення значень текстових полів на основі вхідного об'єкта Member1
                NameTextBox.Text = originalMember.Name;
                PositionTextBox.Text = originalMember.Position;
                EmailTextBox.Text = originalMember.Email;
                PhoneTextBox.Text = originalMember.Phone;
                PriceTextBox.Text = originalMember.Price.ToString(); // Перетворення в рядок для встановлення значення
                CharacterTextBox.Text = originalMember.Character;
                FormAmountTextBox.Text = originalMember.FormAmount.ToString();
                Type.Text = originalMember.Type;// Перетворення в рядок для встановлення значення
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Оновлення властивостей оригінального об'єкта member1 зі значеннями, введеними користувачем
                originalMember.Name = NameTextBox.Text;
                originalMember.Position = PositionTextBox.Text;
                originalMember.Email = EmailTextBox.Text;
                originalMember.Phone = PhoneTextBox.Text;
                originalMember.Price = decimal.Parse(PriceTextBox.Text);
                originalMember.Character = CharacterTextBox.Text;
                originalMember.FormAmount = int.Parse(FormAmountTextBox.Text);
                originalMember.Type = Type.Text;
                // Закриття діалогового вікна з DialogResult, встановленим в true
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при збереженні даних: {ex.Message}");
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            // Закриття діалогового вікна без збереження змін, встановлення DialogResult в false
            DialogResult = false;
            Close();
        }
    }
}




