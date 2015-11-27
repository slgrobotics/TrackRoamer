/*
 * Copyright (c) 2013..., Sergei Grichine   http://trackroamer.com
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at 
 *    http://www.apache.org/licenses/LICENSE-2.0
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *    
 * this is a no-warranty no-liability permissive license - you do not have to publish your changes,
 * although doing so, donating and contributing is always appreciated
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Trackroamer.Library.LibArduinoComm
{
    public class ArduinoComm
    {
        private BackgroundWorker worker;
        private static bool isWorkerRunning = false;

        /// <summary>
        /// The low level serial port (Arduino COMxx over USB)
        /// </summary>
        private SerialPort _serialPort;
        private string ComPortName = "";
        private int ComBaudRate;

        public Queue<ToArduino> outputQueue = new Queue<ToArduino>();
        DateTime lastLineReceived;

        public void Open(string comPort, int baudRate = 57600)
        {
            ComPortName = comPort;
            ComBaudRate = baudRate;

            //lock (outputQueue)
            //{
            //    outputQueue.Clear();
            //}

            // create our background worker and support cancellation
            worker = new BackgroundWorker();
            worker.WorkerSupportsCancellation = true;

            worker.DoWork += delegate(object s, DoWorkEventArgs args)
            {
                Debug.WriteLine("IP: ArduinoComm RunWorker started");

                isWorkerRunning = true;

                lastLineReceived = DateTime.Now;
                int i = 0;
                while (!worker.CancellationPending)
                {
                    Debug.WriteLine("********* ArduinoComm Try: " + (++i).ToString());

                    using (_serialPort = new SerialPort(ComPortName, ComBaudRate, Parity.None, 8, StopBits.One))
                    {
                        _serialPort.Handshake = Handshake.RequestToSendXOnXOff; //.None;
                        _serialPort.Encoding = Encoding.ASCII;      // that's only for text read, not binary
                        _serialPort.NewLine = "\r\n"; 
                        _serialPort.ReadTimeout = 1100;
                        _serialPort.WriteTimeout = 10000;
                        _serialPort.DtrEnable = false;
                        _serialPort.RtsEnable = false;
                        //p.ParityReplace = 0;

                        try
                        {
                            _serialPort.Open();
                            _serialPort.DataReceived += new SerialDataReceivedEventHandler(serialPort_DataReceived);

                            Debug.WriteLine("OK: ArduinoComm Open() success!");

                            while (!worker.CancellationPending)
                            {
                                bool needToSleep = true;

                                lock (outputQueue)
                                {
                                    while (outputQueue.Count > 0)
                                    {
                                        ToArduino toArduino = outputQueue.Dequeue();
                                        string toWrite = toArduino.ToString();
                                        Debug.WriteLine("========> " + toWrite);
                                        _serialPort.WriteLine(toWrite);
                                        Thread.Sleep(10);
                                        needToSleep = false;
                                    }
                                }

                                if(needToSleep)
                                {
                                    Thread.Sleep(20);
                                }

                            }
                            Debug.WriteLine("IP: ArduinoComm RunWorker Cancellation Pending");

                            _serialPort.Close();
                        }
                        catch (Exception e)
                        {
                            Debug.WriteLine(e.GetType().Name + ": " + e.Message);
                        }
                    }

                    Debug.WriteLine("********* ArduinoComm finished try " + i);
                }

                args.Cancel = true;
                Debug.WriteLine("OK: ArduinoComm RunWorker Cancellation sequence completed");
                isWorkerRunning = false;
            };

            worker.RunWorkerCompleted += delegate(object s, RunWorkerCompletedEventArgs args)
            {
                Debug.WriteLine("OK: ArduinoComm RunWorker Completed");
            };

            //run the process:
            worker.RunWorkerAsync();
        }

        public void Close()
        {
            Debug.WriteLine("IP: ArduinoComm Close()");

            // cancel the worker process:
            if (worker != null)
            {
                worker.CancelAsync();

                while (isWorkerRunning)
                {
                    Thread.Sleep(100);
                }
            }
            Debug.WriteLine("OK: ArduinoComm Close() done");
        }

        /// <summary>
        /// Serial Port data event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (e.EventType == SerialData.Chars)
            {
                int i = 0;
                string line;
                try
                {
                    i = _serialPort.BytesToRead;
                    while (_serialPort.BytesToRead > 25 && i > 10)
                    {
                        line = string.Empty;
                        line = _serialPort.ReadLine();
                        i -= (line.Length + 2);
                        //Debug.WriteLine(line);
                        int ix = line.IndexOf('*');
                        if (ix < 0)
                        {
                            Debug.WriteLine("Error: ArduinoComm - Invalid Stream");
                        }
                        else
                        {
                            DateTime now = DateTime.Now;
                            double msSinceLastReceived = (now - lastLineReceived).TotalMilliseconds;
                            lastLineReceived = now;
                            Debug.WriteLine("OK: '" + line + "'      ms: " + Math.Round(msSinceLastReceived));
                        }
                    }
                }
                catch (ArgumentException ex)
                {
                    if (ex.Message == "Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.")
                    {
                        Debug.WriteLine("Error: ArduinoComm - Invalid Baud Rate");
                    }
                    else
                    {
                        Debug.WriteLine("Error: ArduinoComm - Error Reading From Serial Port");
                    }
                }
                catch (TimeoutException ex)
                {
                    Debug.WriteLine("Error: ArduinoComm - TimeoutException: " + ex);
                }
                catch (IOException ex)
                {
                    Debug.WriteLine("Error: ArduinoComm - IOException: " + ex);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Error: ArduinoComm - Exception: " + ex);
                }
            }
        }
    }
}
