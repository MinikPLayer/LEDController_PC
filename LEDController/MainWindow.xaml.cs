using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
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
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            Debug.Log("Initializing");
            InitializeComponent();

            SerialCommunicationEngine.InitializeStatus status = SerialCommunicationEngine.InitializeStatus.IOError;
            for(int i = 0;i<5;i++) // Try 5 times if IO error
            {
                status = Global.controller.Connect();
                Debug.Log("Connecting...");
                if (!Global.controller.initialized || status != SerialCommunicationEngine.InitializeStatus.Success)
                {

                    if(status == SerialCommunicationEngine.InitializeStatus.IOError)
                    {
                        Thread.Sleep(100);
                        continue;
                    }

                    if (Global.controller.lastError.Length == 0)
                        MessageBox.Show("Cannot find the device, try to reconnect\n", "Cannot find the device", MessageBoxButton.OK, MessageBoxImage.Error);
                    else if (status == SerialCommunicationEngine.InitializeStatus.NotFound)
                        MessageBox.Show("Device not connected", "Error connecting", MessageBoxButton.OK, MessageBoxImage.Error);
                    else
                        MessageBox.Show(Global.controller.lastError, "Error connecting", MessageBoxButton.OK, MessageBoxImage.Error);
                    Environment.Exit(1);
                }
                else
                {
                    break;
                }
            }
            if(status == SerialCommunicationEngine.InitializeStatus.IOError)
            {
                MessageBox.Show("IO Error", "Error connecting", MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(2);
            }


            //PageManager.SetPage(new Solid().Content);
            Debug.Log("Getting actual state");
            string state = Global.controller.GetState();
            Debug.Log("Actual State: " + state);
            if(state.StartsWith("#!"))
            {
                MessageBox.Show("Cannot get current state", "Cannot get current state", MessageBoxButton.OK, MessageBoxImage.Error);
                Debug.FatalError("Cannot get current state", 200);        
            }
            bool result = Global.controller.SetState(state);
            if(!result)
            {
                Debug.LogError("Cannot set state \"" + state + "\"");
                if(!Global.controller.SetState("Static"))
                {
                    MessageBox.Show("Unknown fatal error communicating with the device, cannot set the default state", "Unknown error", MessageBoxButton.OK, MessageBoxImage.Error);
                    Debug.FatalError("Unknown fatal error communicating with the device, cannot set the default state", 201, 5000);
                }
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            Global.controller.Disconnect();

            base.OnClosing(e);

            Application.Current.Shutdown();
        }

    }

    public static class PageManager
    {
        public static void SetPage(object page)
        {
            Application.Current.MainWindow.Content = page;
        }
    }
}
