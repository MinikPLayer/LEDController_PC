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
    public partial class LogViewer : Window
    {

        public LogViewer()
        {
            InitializeComponent();

            MessageUpdateThread();
        }

        void MessageUpdateThread()
        {
            Thread t = new Thread(() =>
            {
                try
                {
                    while (true)
                    {
                        MessagesPanel.Dispatcher.Invoke(() =>
                        {
                            UpdateMessages();
                        });


                        Thread.Sleep(500);
                    }
                }
                catch(System.Threading.Tasks.TaskCanceledException e)
                {
                    return;
                }
            });

            t.Start();
        }


        void UpdateMessages()
        {
            int start = MessagesPanel.Children.Count;
            if(start > Debug.messages.Count) // Log was probably cleared
            {
                MessagesPanel.Children.Clear();
                start = 0;
            }
            for(int i = start;i<Debug.messages.Count;i++)
            {
                var msg = Debug.messages[i];
                Color clr = Color.FromRgb(255, 255, 255);
                if(msg.color == ConsoleColor.Red)
                {
                    clr = Color.FromRgb(255, 0, 0);
                }
                else if(msg.color == ConsoleColor.Yellow)
                {
                    clr = Color.FromRgb(255, 0, 255);
                }
                TextBlock block = new TextBlock() { Text = "[" + msg.prefix + "] " + msg.message, Foreground = new SolidColorBrush(clr) };

                MessagesPanel.Children.Add(block);
            }
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

    }

}
