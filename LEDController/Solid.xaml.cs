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
    public partial class Solid : Window
    {
        public Solid()
        {
            InitializeComponent();

            navBar.SetMode(NavBar.Modes.Solid);

            string color = Global.controller.GetParam("color");
            ColorChoser.SelectedColor = LEDController.StringToColor(color);
            ColorText.Foreground = new SolidColorBrush((Color)ColorChoser.SelectedColor);
        }

        private void ColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            bool response = Global.controller.SetParam("color", LEDController.ColorToString((Color)e.NewValue));//Global.controller._Send("+setparam color " + LEDController.ColorToString((Color)e.NewValue));
            if(!response)
            {
                Debug.LogError("[Solid] Cannot sync color");
            }
            ColorText.Foreground = new SolidColorBrush((Color)ColorChoser.SelectedColor);
        }
    }
}
