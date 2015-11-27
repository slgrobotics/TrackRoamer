using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO.Ports;
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

namespace WpfLidarLiteTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static string currentPort;
        private LidarLiteProcessor lidarLiteProcessor;
        private BackgroundWorker worker;
        private static bool isWorkerRunning = false;

        public MainWindow()
        {
            InitializeComponent();

            FillSerialPortComboBox();

            PrepareBackgroundWorker();
        }

        /// <summary>
        /// initialize SerialPortComboBox with names of all available serial ports
        /// </summary>
        void FillSerialPortComboBox()
        {
            string[] ports = SerialPort.GetPortNames();

            SerialPortComboBox.ItemsSource = ports;
        }

        /// <summary>
        /// prepares Background Worker but does not start it yet.
        /// </summary>
        void PrepareBackgroundWorker()
        {
            // get our dispatcher
            System.Windows.Threading.Dispatcher dispatcher = this.Dispatcher;
 
            // create our background worker and support cancellation
            worker = new BackgroundWorker();
            worker.WorkerSupportsCancellation = true;

            worker.DoWork += delegate(object s, DoWorkEventArgs args)
            {
                Debug.WriteLine("IP: RunWorker started");

                try
                {
                    isWorkerRunning = true;
                    // open currentPort
                    lidarLiteProcessor = new LidarLiteProcessor();
                    lidarLiteProcessor.Open(new string[] { currentPort });
                    lidarLiteProcessor.DataReceivedEvent += lidarLiteProcessor_DataReceived;

                    Dispatcher.Invoke(new Action<object>(EnableOpenCloseButton), "");

                    while (true)
                    {
                        lidarLiteProcessor.StartedLoop();   // helps keeping 20ms cycle steady

                        if (worker.CancellationPending)
                        {
                            Debug.WriteLine("IP: RunWorker Cancellation Pending, closing serial port");

                            lidarLiteProcessor.Close();

                            args.Cancel = true;

                            Debug.WriteLine("OK: RunWorker Cancellation sequence completed");

                            isWorkerRunning = false;

                            return;
                        }

                        lidarLiteProcessor.Process();

                        DisplayAll();

                        lidarLiteProcessor.WaitInLoop();  // we won't wait here longer than lidarLiteProcessor.desiredLoopTimeMs - about 20ms-<already elapsed time>
                    }
                }
                catch (Exception exc)
                {
                    Debug.WriteLine("Error: RunWorker: " + exc);

                    // create a new delegate for updating our status text
                    UpdateStatusDelegate update = new UpdateStatusDelegate(UpdateStatusText);

                    // invoke the dispatcher and pass the error data:
                    Dispatcher.BeginInvoke(update, "Error: RunWorker: " + exc.Message);

                    lidarLiteProcessor.Close();     // close communication to the serial port

                    Dispatcher.Invoke(new Action<object>(ResetOpenCloseButton), "");
                }
            };

            worker.RunWorkerCompleted += delegate(object s, RunWorkerCompletedEventArgs args)
            {
                isWorkerRunning = false;

                Dispatcher.Invoke(new Action<object>(EnableOpenCloseButton), "");

                Debug.WriteLine("OK: RunWorker Completed");
            };
        }

        void lidarLiteProcessor_DataReceived(object sender, LaserDataSerializable data)
        {
            Debug.WriteLine("OK: lidarLiteProcessor_DataReceived");
            Dispatcher.Invoke(new Action<LaserDataSerializable>(SetCurrentLaserData), data);
        }

        private void SetCurrentLaserData(LaserDataSerializable data)
        {
            this.LidarViewControl.CurrentLaserData = data;
        }

        /// <summary>
        /// all logging and displaying in UI is handled here.
        /// </summary>
        private void DisplayAll()
        {
        }

        private void EnableOpenCloseButton(object obj)
        {
            OpenCloseButton.IsEnabled = true;
        }

        private void ResetOpenCloseButton(object obj)
        {
            SerialPortComboBox.IsEnabled = true;
            OpenCloseButton.Content = "Open";
        }

        void StartWorker()
        {
            //run the worker process:
            worker.RunWorkerAsync();
        }

        void StopWorker()
        {
            // cancel the worker process:
            worker.CancelAsync();
        }

        void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            Debug.WriteLine("IP: MainWindow_Closing");

            // cancel the worker process:
            StopWorker();

            while (isWorkerRunning)
            {
                Thread.Sleep(100);
            }

            Debug.WriteLine("OK: MainWindow_Closing done");
        }

        void MainWindow_Unloaded(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("OK: MainWindow_Unloaded");

            // cancel the worker process:
            worker.CancelAsync();
        }

        // a delegate used for updating the UI "status" panel
        public delegate void UpdateStatusDelegate(string stateMessage);

        // this is the method that the delegate will execute
        public void UpdateStatusText(string stateMessage)
        {
            StatusLabel.Content = " " + stateMessage;
        }

        /// <summary>
        /// closes application when error popup button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExitPopupButton_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(1);
        }

        /// <summary>
        /// click on the button that selects the serial port and then starts backround worker, thus starting the robot.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenCloseButton_Click(object sender, RoutedEventArgs e)
        {
            OpenCloseButton.IsEnabled = false;

            if (isWorkerRunning)
            {
                // cancel the worker process:
                StopWorker();

                SerialPortComboBox.IsEnabled = true;
                OpenCloseButton.Content = "Open";
            }
            else
            {
                currentPort = "" + SerialPortComboBox.SelectedValue;

                if (!string.IsNullOrWhiteSpace(currentPort))
                {
                    // start the worker process:
                    StartWorker();

                    SerialPortComboBox.IsEnabled = false;
                    OpenCloseButton.Content = "Close";
                }
                else
                {
                    StatusLabel.Content = "Please select the port to connect to LIDAR Leonardo Board";
                }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
        }
    }
}
