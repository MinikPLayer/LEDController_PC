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

namespace Controller
{
    /// <summary>
    /// Logika interakcji dla klasy Solid.xaml
    /// </summary>
    public partial class Breathing : Window
    {
        public Breathing()
        {
            InitializeComponent();

            navBar.SetMode(NavBar.Modes.Breathing);

            UpdateSpeedValue();
        }

        void UpdateSpeedValue()
        {
            byte speed = Global.controller.GetParam<byte>("speed");
            SpeedNumber.Text = speed.ToString();
            SpeedSlider.Value = speed / 100.0;
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            byte speed = (byte)(e.NewValue * 100);
            if (Global.controller.GetParam<byte>("speed") == speed) return;
            Global.controller.SetParam("speed", speed.ToString());

            UpdateSpeedValue();
        }
    }
}
