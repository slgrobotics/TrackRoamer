using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WpfLidarLiteTest
{
   	public delegate void ElementEventHandler(Object sender, LaserDataSerializable data);

    public class LidarLiteProcessor
    {
        public event ElementEventHandler DataReceivedEvent;

        private SerialPort serialPort = new SerialPort();

        private Stopwatch stopWatch = new Stopwatch();
        private double loopStartTime = 0.0d;
        private double desiredLoopTimeMs = 50.0d;   // will be used as encoders Sampling Interval

        public bool Open(string[] args)
        {
            string portName = args[0];  // we must pass serial port name here
            int baudRate = 115200;

            Debug.WriteLine("Trying serial port: " + portName);

            serialPort = new SerialPort(portName, baudRate, Parity.None, 8, StopBits.One);
            serialPort.PortName = portName;
            serialPort.BaudRate = baudRate;
            serialPort.Parity = Parity.None;
            serialPort.DataBits = 8;
            serialPort.StopBits = StopBits.One;
            serialPort.DtrEnable = true;    // Arduino Leonardo requires this
            //serialPort.RtsEnable = false;
            //serialPort.ReadTimeout = 300;
            //serialPort.WriteTimeout = 10000;
            serialPort.DataReceived += serialPort_DataReceived;
            serialPort.ErrorReceived += serialPort_ErrorReceived;
            //serialPort.
            //serialPort.NewLine = "\r";

            Debug.WriteLine("IP: serial port - opening...");
            serialPort.Open();

            Debug.WriteLine("OK: opened");

            // Clear receive buffer out, since the bootloader can send
            // some junk characters, which might hose subsequent command responses:
            serialPort.DiscardInBuffer();

            stopWatch.Start();

            return true;
        }

        void serialPort_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            Debug.WriteLine("serialPort_ErrorReceived");
        }

        void serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            //Debug.WriteLine("serialPort_DataReceived");
            try
            {
                string resp = serialPort.ReadLine();
                //Debug.WriteLine("OK: resp='" + resp + "'");
                string[] parts = resp.Split(new char[] { '|' });
                if (parts.Length == 4 && parts[0].StartsWith("*"))
                {
                    int count;
                    if (int.TryParse(parts[1], out count) && count < 250 && count > 150)
                    {
                        string[] sVals = parts[3].Split(new char[] { ' ' });
                        if (sVals.Length > 150)
                        {
                            List<int> values = new List<int>();
                            foreach (string s in sVals)
                            {
                                if ("0123456789".IndexOf(s[0]) >= 0)
                                {
                                    int val = int.Parse(s);
                                    values.Add(val == 0 ? 4000 : val);
                                }
                            }
                            //Debug.WriteLine("OK: resp parsed to " + values.Count + " readings");
                            if (count == values.Count)
                            {
                                //Debug.WriteLine("OK: good readings");
                                if (DataReceivedEvent != null)
                                {
                                    LaserDataSerializable data = new LaserDataSerializable() { TimeStamp = DateTime.Now.Ticks, DistanceMeasurements = values.ToArray() };
                                    DataReceivedEvent(this, data);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                Debug.WriteLine("Error: serialPort_DataReceived - exception " + exc);
            }
        }

        public void Close()
        {
            Debug.WriteLine("IP: serial port - closing...");
            serialPort.Close();
            Debug.WriteLine("OK: closed");
        }

        /// <summary>
        /// mark the beginning of the processing cycle in the worker loop
        /// </summary>
        public void StartedLoop()
        {
            loopStartTime = stopWatch.ElapsedMilliseconds;  // mark the start time of the cycle.
        }

        public void PumpEvents()
        {
        }

        /// <summary>
        /// waits before starting the next cycle in the worker loop.
        /// we need to keep Element events pump working and let other threads take over while we are waiting.
        /// we won't wait here longer than desiredLoopTimeMs
        /// </summary>
        public void WaitInLoop()
        {
            this.PumpEvents();

            // Wait here - use a time fixed loop:
            if (stopWatch.ElapsedMilliseconds - loopStartTime < desiredLoopTimeMs)
            {
                while ((stopWatch.ElapsedMilliseconds - loopStartTime) < desiredLoopTimeMs)
                {
                    this.PumpEvents();
                    if ((stopWatch.ElapsedMilliseconds - loopStartTime) < desiredLoopTimeMs)
                    {
                        Thread.Sleep(1);
                    }
                }
            }
        }

        /// <summary>
        /// called often, must return promptly
        /// </summary>
        public void Process()
        {
            //string resp = serialPort.ReadLine();
            //Debug.WriteLine("OK: resp='" + resp + "'");
        }
    }
}
