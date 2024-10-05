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

namespace Pharmacy
{
    /// <summary>
    /// Interaction logic for AnnotationWindow.xaml
    /// </summary>
    public partial class AnnotationWindow : Window
    {
        public AnnotationWindow(string annotation)
        {
            InitializeComponent();
            textBoxAnnotation.Text = annotation;
        }
    }

}
