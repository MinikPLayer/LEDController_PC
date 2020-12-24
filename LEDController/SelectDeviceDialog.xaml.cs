using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Controller
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class SelectDeviceDialog : Window
    {
        Action<string> OnDeviceChosen;

        List<string> devices;

        public SelectDeviceDialog(List<string> devices, Action<string> OnDeviceChosenAction)
        {
            OnDeviceChosen = OnDeviceChosenAction;
            this.devices = new List<string>(devices);

            InitializeComponent();
        }

        private void DevicePicker_Initialized(object sender, EventArgs e)
        {
            DevicePicker.Dispatcher.Invoke(() =>
            {
                DevicePicker.ItemsSource = devices;
            });
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            if(OnDeviceChosen != null)
            {
                string text = DevicePicker.Text;
                if (text.Length > 0)
                    text = "NSEL";    
                    
                OnDeviceChosen(DevicePicker.Text);

                Close();
            }
            else
            {
                MessageBox.Show("Wystąpił wewnętrzny błąd, kod błędu: (801)", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }

}
