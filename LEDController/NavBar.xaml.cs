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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Controller
{
    /// <summary>
    /// Logika interakcji dla klasy NavBar.xaml
    /// </summary>
    public partial class NavBar : UserControl
    {
        public NavBar()
        {
            InitializeComponent();

            UpdateBrightnessSlider();

            int physicalBrightness = Global.controller.GetGlobalParam<int>("brightness -physical");
            if(physicalBrightness == 1)
            {
                Debug.Log("Enabled");
                PhysicalBrightnessCheckbox.IsChecked = true;
            }
            else if(physicalBrightness == 0)
            {
                Debug.Log("Disabled");
                PhysicalBrightnessCheckbox.IsChecked = false;
            }
            else
            {
                Debug.LogError("PhysicalBrightness response: " + physicalBrightness);
            }

            for(int i = 0;i<LEDController.states.Length;i++)
            {
                TextBlock text = new TextBlock();
                text.Text = LEDController.states[i].name;

                StateChooser.Items.Add(text);

                
            }

            StateChooser.SelectionChanged += StateClicked;
        }

        void StateClicked(object sender, SelectionChangedEventArgs e)
        {
            Debug.Log("State clicked");

            try
            {
                ComboBox combo = (ComboBox)e.Source;
                TextBlock text = (TextBlock)combo.SelectedItem;

                LEDController.State state = Global.controller.FindState(FindStateIndex(text.Text));
                if (Global.controller.GetState() == state.paramName) return;
                if(!Global.controller.SetState(state.paramName))
                {
                    Debug.LogError("[NavBar] Cannot change state");
                    return;
                }
            }
            catch(Exception ex)
            {
                Debug.Exception(ex, "Changing state");
            }
            
        }

        void UpdateBrightnessSlider()
        {
            byte brightness = Global.controller.GetBrightness();
            //Debug.Log("Brightness: " + brightness);
            BrightnessSlider.Value = brightness;
        }

        public enum Modes
        {
            none,
            Solid,
            Breathing,
            Save,
            Rainbow,
            RisingAndFalling,
            BurningDot,
            Gradient,
        }

        int FindStateIndex(string name)
        {
            for(int i = 0;i<StateChooser.Items.Count;i++)
            {
                try
                {
                    TextBlock b = (TextBlock)StateChooser.Items[i];

                    if (b.Text == name) return i;
                }catch(Exception e)
                {
                    Debug.LogError("[NavBar] ComboBox item " + i + " is not a TextBlocK");
                }
            }
            return -1;
        }

        void ChangeSelectedState(string name)
        {
            int pos = FindStateIndex(name);
            if (pos == -1)
            {
                Debug.LogError("[NavBar] Cannot find state \"" + name + "\"");
                return;
            }
            StateChooser.SelectedIndex = pos;
        }

        public void SetMode(Modes mode)
        {

            switch (mode)
            {
                case Modes.none:
                    break;
                case Modes.Solid:
                    {
                        //SolidColorBrush brush = (SolidColorBrush)SolidButton.Background;
                        //brush.Color = Color.FromArgb(50, brush.Color.R, brush.Color.G, brush.Color.B);
                        ChangeSelectedState("Static");
                        break;
                    }
                case Modes.Breathing:
                    {
                        //SolidColorBrush brush = (SolidColorBrush)BreathingButton.Background;
                        //brush.Color = Color.FromArgb(50, brush.Color.R, brush.Color.G, brush.Color.B);
                        ChangeSelectedState("Breathing");
                        break;
                    }
                case Modes.Save:
                    {
                        SolidColorBrush brush = (SolidColorBrush)SaveButton.Background;
                        //brush.Color = Color.FromArgb(50, brush.Color.R, brush.Color.G, brush.Color.B);
                        break;
                    }
                case Modes.Rainbow:
                    {
                        //SolidColorBrush brush = (SolidColorBrush)RainbowButton.Background;
                        //brush.Color = Color.FromArgb(50, brush.Color.R, brush.Color.G, brush.Color.B);
                        ChangeSelectedState("Rainbow");
                        break;
                    }

                case Modes.RisingAndFalling:
                    {
                        ChangeSelectedState("Rising And Falling");
                        break;
                    }

                case Modes.BurningDot:
                    {
                        ChangeSelectedState("Burning dot");
                        break;
                    }

                case Modes.Gradient:
                    {
                        ChangeSelectedState("Gradient");
                        break;
                    }

                default:
                    {
                        Debug.LogWarning("[NavBar] No logic for " + mode);
                    }
                    break;
            }
        }

        private void BrightnessSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Global.controller.SetBrightness((byte)e.NewValue);
        }

        private void SolidButton_Click(object sender, RoutedEventArgs e)
        {
            Global.controller.SetState("Static");
        }

        private void BreathingButton_Click(object sender, RoutedEventArgs e)
        {
            Global.controller.SetState("Breathing");
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            //Debug.FatalError("Błąd zapisu xd", 0, 0);

            if(Global.controller.Save())
            {
                MessageBox.Show("Zapisano!", "Zapis", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                Debug.FatalError("Nie udało się zapisać trybu");
            }
        }

        private void RainbowButton_Click(object sender, RoutedEventArgs e)
        {
            Global.controller.SetState("Rainbow");
        }

        private void PhysicalBrightnessCheckbox_Checked(object sender, RoutedEventArgs e)
        {

            Global.controller.SetGlobalParam("bright -physical", "1");
            UpdateBrightnessSlider();
        }

        private void PhysicalBrightnessCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            Global.controller.SetGlobalParam("bright -physical", "0");
            UpdateBrightnessSlider();
        }
    }
}
