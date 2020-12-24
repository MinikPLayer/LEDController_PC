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
    public partial class Rainbow : Window
    {
        public Rainbow()
        {
            InitializeComponent();

            navBar.SetMode(NavBar.Modes.Rainbow);

            UpdateWidthValue();
            UpdateSpeedValue();
        }


        bool widthSynced = false;
        void UpdateWidthValue()
        {
            byte width = Global.controller.GetParam<byte>("width");
            WidthNumber.Text = width.ToString();
            WidthSlider.Value = width;

            widthSynced = true;
        }

        bool speedSynced = false;
        void UpdateSpeedValue()
        {
            //if (SpeedNumber == null) return;

            byte speed = Global.controller.GetParam<byte>("speed");
            SpeedNumber.Text = speed.ToString();
            SpeedSlider.Value = speed / 100.0;

            speedSynced = true;
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!speedSynced)
            {
                UpdateSpeedValue();
                return;
            }

            byte speed = (byte)(e.NewValue * 100);
            if (Global.controller.GetParam<byte>("speed") == speed) return;
            Global.controller.SetParam("speed", speed.ToString());

            UpdateSpeedValue();
        }

        private void WidthSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if(!widthSynced)
            {
                UpdateWidthValue();
                return;
            }

            byte width = (byte)(e.NewValue);
            if (Global.controller.GetParam<byte>("width") == width) return;
            Global.controller.SetParam("width", width.ToString());

            UpdateWidthValue();
        }
    }
}
