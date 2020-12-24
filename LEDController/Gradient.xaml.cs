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
    public partial class Gradient : Window
    {
        public Gradient()
        {
            InitializeComponent();

            navBar.SetMode(NavBar.Modes.Gradient);

            //string color = Global.controller.GetParam("color");
            //ColorChoser.SelectedColor = LEDController.StringToColor(color);
            //ColorText.Foreground = new SolidColorBrush((Color)ColorChoser.SelectedColor);

            string color1, color2;
            color1 = Global.controller.GetParam("color1");
            color2 = Global.controller.GetParam("color2");

            Color1Choser.SelectedColor = LEDController.StringToColor(color1);
            Color2Choser.SelectedColor = LEDController.StringToColor(color2);
        }

        void ChangeColor(int number, Color newColor)
        {
            /*bool response = Global.controller.SetParam("color1", LEDController.ColorToString(newColor));//Global.controller._Send("+setparam color " + LEDController.ColorToString((Color)e.NewValue));
            if (!response)
            {
                Debug.LogError("[Solid] Cannot sync color");
            }*/
            if (number == 1)
            {
                bool response = Global.controller.SetParam("color1", LEDController.ColorToString(newColor));
                if (!response)
                    Debug.LogError("[Solid] Cannot sync color");

                Color1Text.Foreground = new SolidColorBrush((Color)Color1Choser.SelectedColor);
            }
            else if (number == 2)
            {
                bool response = Global.controller.SetParam("color2", LEDController.ColorToString(newColor));
                if (!response)
                    Debug.LogError("[Solid] Cannot sync color");

                Color2Text.Foreground = new SolidColorBrush((Color)Color2Choser.SelectedColor);
            }
            else
                Debug.LogError("[Gradient.cs] Cannot find color number");
            
        }

        private void Color1Changed(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            ChangeColor(1, (Color)e.NewValue);
        }

        private void Color2Changed(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            ChangeColor(2, (Color)e.NewValue);
        }
    }
}
