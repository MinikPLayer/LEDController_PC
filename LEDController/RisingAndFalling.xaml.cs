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
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Xceed.Wpf.Toolkit;

namespace Controller
{
    /// <summary>
    /// Logika interakcji dla klasy Solid.xaml
    /// </summary>
    public partial class RisingAndFalling : Window
    {
        public RisingAndFalling()
        {
            InitializeComponent();

            navBar.SetMode(NavBar.Modes.RisingAndFalling);

            UpdateDelayValue();

            textTemplate = XamlWriter.Save(colorNumberTemplate);
            colorsStackPanel.Children.Clear();

            GetColors();
        }

        string textTemplate = "";

        List<Color> colors = new List<Color>();
        int colorSelected = 0;


        bool speedSynced = false;
        void UpdateDelayValue()
        {
            //if (SpeedNumber == null) return;

            int speed = Global.controller.GetParam<int>("delay");
            SpeedNumber.Text = speed.ToString();
            SpeedSlider.Value = speed / 100.0;

            speedSynced = true;
        }

        void GetColors()
        {
            string data = Global.controller.GetParam("colors");

            string[] splits = data.Split(';');

            for(int i = 0;i<splits.Length;i++)
            {
                if (splits[i].Length == 0) continue;

                AddColor();

                //colors[colors.Count - 1] = LEDController.StringToColor(splits[i]);
                colorPicker.SelectedColor = LEDController.StringToColor(splits[i]);
            }
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!speedSynced)
            {
                UpdateDelayValue();
                return;
            }

            int delay = (int)(e.NewValue * 100);
            if (Global.controller.GetParam<int>("delay") == delay) return;
            Global.controller.SetParam("delay", delay.ToString());

            UpdateDelayValue();
        }

        void UpdateColorPicker()
        {
            if (colorSelected < 0 || colorSelected >= colors.Count)
            {
                Debug.LogWarning("Bad colorSelected value");
                return;
            }
            colorPicker.SelectedColor = colors[colorSelected];

            ColorsText.Foreground = new SolidColorBrush(colors[colorSelected]);

            TextBlock textBlock = (TextBlock)colorsStackPanel.Children[colorSelected];
            textBlock.Background = new SolidColorBrush(new Color() { A = 128, R = 0, G = 0, B = 0 });
        }

        private void ColorClicked(object sender, MouseButtonEventArgs e)
        {
            TextBlock text;
            try
            {
                text = (TextBlock)sender;
            }
            catch(Exception ex)
            {
                Debug.LogError("Sender is not a TextBlock");
                return;
            }
            Debug.Log("Clicked on color "  + text.Text);

            int pos = int.Parse(text.Text) - 1;
            TextBlock textBlock = (TextBlock)colorsStackPanel.Children[colorSelected];
            textBlock.Background = new SolidColorBrush(new Color() { A = 0 });

            colorSelected = pos;
            


            UpdateColorPicker();
        }

        void UpdateLEDs()
        {
            string value = "";
            for(int i = 0;i<colors.Count;i++)
            {
                value += LEDController.ColorToString(colors[i]) + ";";
            }

            Global.controller.SetParam("colors", value);
        }

        private void colorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (colorSelected < 0 || colorSelected >= colors.Count)
            {
                Debug.LogWarning("Bad colorSelected value");
                return;
            }
            colors[colorSelected] = (Color)e.NewValue;

            TextBlock text = (TextBlock)colorsStackPanel.Children[colorSelected];
            Color c = colors[colorSelected];
            c.A = 255;
            text.Foreground = new SolidColorBrush(c);
        }

        void AddColor()
        {
            colors.Add(new Color() { R = 255, G = 255, B = 255, A = 255 });
            TextBlock text = (TextBlock)XamlReader.Parse(textTemplate);
            text.Text = colors.Count.ToString();
            text.MouseLeftButtonDown += ColorClicked;
            colorsStackPanel.Children.Add(text);

            if (colors.Count > 1)
            {
                TextBlock textBlock = (TextBlock)colorsStackPanel.Children[colorSelected];
                textBlock.Background = new SolidColorBrush(new Color() { A = 0 });
            }

            colorSelected = colors.Count - 1;

            TextBlock txtBlock = (TextBlock)colorsStackPanel.Children[colorSelected];
            txtBlock.Background = new SolidColorBrush(new Color() { A = 128, R = 0, G = 0, B = 0 });

            UpdateColorPicker();
        }

        private void AddColorButton_Click(object sender, RoutedEventArgs e)
        {
            AddColor();
        }

        private void RemoveColorButton_Click(object sender, RoutedEventArgs e)
        {
            if (colors.Count == 0) return;

            colorsStackPanel.Children.RemoveAt(colorSelected);
            colors.RemoveAt(colorSelected);

            if (colorSelected >= colors.Count)
            {
                colorSelected = colors.Count - 1;
            }

            UpdateColorPicker();
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateLEDs();
        }
    }
}
