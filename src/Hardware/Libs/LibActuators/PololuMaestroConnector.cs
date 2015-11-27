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
using System.Linq;
using System.Text;

// Pololu "Mini Maestro 12" is used here to control servos. See Usc\bin folder for dlls. Need to install driver too from Pololu.
using Pololu.Usc;
using Pololu.UsbWrapper;
using TrackRoamer.Robotics.LibBehavior;


namespace TrackRoamer.Robotics.LibActuators
{
    public class ChannelValuePair
    {
        public byte Channel { get; set; }       // Channel number - from 0 to 23, crosses over connected devices.

        //   Target, in units of quarter microseconds.  For typical servos,
        //   6000 is neutral and the acceptable range is 4000-8000.
        //   A good servo will take 880 to 2200 us (3520 to 8800 in quarters)
        public ushort Target { get; set; }

        public override string ToString()
        {
            return string.Format("{0}:{1}", Channel, Target);
        }
    }

    /// <summary>
    /// A Pololu Maestro Device Command
    /// <remarks>Use with SendPololuMaestroCommand)</remarks>
    /// </summary>
    public class PololuMaestroCommand
    {
        public string Command { get; set; }     // "set"

        public List<ChannelValuePair> ChannelValues { get; set; }

        public override string ToString()
        {
            StringBuilder sbValues = new StringBuilder();
            foreach (ChannelValuePair cvp in ChannelValues)
            {
                sbValues.AppendFormat("{0} ", cvp.ToString());
            }
            return string.Format("{0} - {1}", Command, sbValues.ToString().Trim());
        }
    }

    public class PololuMaestroConnector
    {
        // channels are 0...11 for the first "Pololu Maestro 12" controller, 12...23 for the second and so on.

        public double? currentPanKinect;
        public double currentPanGunLeft;
        public double currentTiltGunLeft;
        public double currentPanGunRight;
        public double currentTiltGunRight;

        public int panKinectMksLast = 0;
        public int panMksLastGunLeft = 0;
        public int tiltMksLastGunLeft = 0;
        public int panMksLastGunRight = 0;
        public int tiltMksLastGunRight = 0;

        private bool isShooting = false;

        /// <summary>
        /// preferred method for pan/tilt control
        /// </summary>
        /// <param name="panDegreesFromCenterGunLeft"></param>
        /// <param name="tiltDegreesFromCenterGunLeft"></param>
        public void setPanTilt(double panDegreesFromCenterGunLeft, double tiltDegreesFromCenterGunLeft, double panDegreesFromCenterGunRight, double tiltDegreesFromCenterGunRight, double? panKinectDegreesFromCenter)
        {
            currentPanGunLeft = panDegreesFromCenterGunLeft;

            double mksPanGunLeft = PanTiltAlignment.getInstance().mksPanGunLeft(panDegreesFromCenterGunLeft);

            int panMksGunLeft = (int)mksPanGunLeft;


            currentTiltGunLeft = tiltDegreesFromCenterGunLeft;

            double mksTiltGunLeft = PanTiltAlignment.getInstance().mksTiltGunLeft(tiltDegreesFromCenterGunLeft);

            int tiltMksGunLeft = (int)mksTiltGunLeft;


            currentPanGunRight = panDegreesFromCenterGunRight;

            double mksPanGunRight = PanTiltAlignment.getInstance().mksPanGunRight(panDegreesFromCenterGunRight);

            int panMksGunRight = (int)mksPanGunRight;


            currentTiltGunRight = tiltDegreesFromCenterGunRight;

            double mksTiltGunRight = PanTiltAlignment.getInstance().mksTiltGunRight(tiltDegreesFromCenterGunRight);

            int tiltMksGunRight = (int)mksTiltGunRight;


            currentPanKinect = panKinectDegreesFromCenter;

            double? mksPanKinect = panKinectDegreesFromCenter.HasGoodValue() ? (double?)PanTiltAlignment.getInstance().mksPanKinect(panKinectDegreesFromCenter.Value) : null;

            int panKinectMks = (int)mksPanKinect.GetValueOrDefault();


            if (   panMksGunLeft != panMksLastGunLeft || tiltMksGunLeft != tiltMksLastGunLeft
                || panMksGunRight != panMksLastGunRight || tiltMksGunRight != tiltMksLastGunRight
                || panKinectDegreesFromCenter.HasGoodValue() && panKinectMks != panKinectMksLast
               )
            {
                panMksLastGunLeft = panMksGunLeft;
                tiltMksLastGunLeft = tiltMksGunLeft;
                panMksLastGunRight = panMksGunRight;
                tiltMksLastGunRight = tiltMksGunRight;

                List<ChannelValuePair> channelValues = new List<ChannelValuePair>();

                // Pololu servo target is in units of quarter microseconds.
                // For typical servos, 6000 is neutral and the acceptable range is 4000-8000.

                channelValues.Add(new ChannelValuePair() { Channel = ServoChannelMap.leftGunPan, Target = (UInt16)(panMksGunLeft << 2) });
                channelValues.Add(new ChannelValuePair() { Channel = ServoChannelMap.leftGunTilt, Target = (UInt16)(tiltMksGunLeft << 2) });
                channelValues.Add(new ChannelValuePair() { Channel = ServoChannelMap.rightGunPan, Target = (UInt16)(panMksGunRight << 2) });
                channelValues.Add(new ChannelValuePair() { Channel = ServoChannelMap.rightGunTilt, Target = (UInt16)(tiltMksGunRight << 2) });

                if (mksPanKinect.HasValue)
                {
                    channelValues.Add(new ChannelValuePair() { Channel = ServoChannelMap.panKinect, Target = (UInt16)(panKinectMks << 2) });
                }

                TrySetTarget(channelValues);
            }
        }

        public void setPanKinect(double degreesFromCenter)
        {
            currentPanKinect = degreesFromCenter;

            double mks = PanTiltAlignment.getInstance().mksPanKinect(degreesFromCenter);

            setPanKinectMks((int)mks);
        }

        public void setPanKinectMks(int panKinectMks)
        {
            if (panKinectMks != panKinectMksLast)
            {
                panKinectMksLast = panKinectMks;

                TrySetTarget(ServoChannelMap.panKinect, (ushort)(panKinectMks << 2));

                //Tracer.Trace(string.Format("Kinect Pan: {0:0}", currentPanKinect));
                //Tracer.Trace(string.Format("{0,4} mks {1,4:0} degrees", panKinectMksLast, currentPanKinect));
            }
        }

        public void setPan(double degreesFromCenter, bool rightGun)
        {
            double mks = 0.0d;

            if (rightGun)
            {
                currentPanGunRight = degreesFromCenter;

                mks = PanTiltAlignment.getInstance().mksPanGunRight(degreesFromCenter);
            }
            else
            {
                currentPanGunLeft = degreesFromCenter;

                mks = PanTiltAlignment.getInstance().mksPanGunLeft(degreesFromCenter);
            }

            setPanMks((int)mks, rightGun);
        }

        public void setPanMks(int panMks, bool rightGun)
        {
            if (rightGun && panMks != panMksLastGunRight)
            {
                panMksLastGunRight = panMks;

                TrySetTarget(ServoChannelMap.rightGunPan, (ushort)(panMks << 2));

                //Tracer.Trace(string.Format("Right Pan: {0:0}\r\nTilt: {1:0}", currentPan, currentTilt));
                //Tracer.Trace(string.Format("{0,4} mks {1,4:0} degrees", panMksLast, currentPan));
            }
            else if (! rightGun && panMks != panMksLastGunLeft)
            {
                panMksLastGunLeft = panMks;

                TrySetTarget(ServoChannelMap.leftGunPan, (ushort)(panMks << 2));

                //Tracer.Trace(string.Format("Left Pan: {0:0}\r\nTilt: {1:0}", currentPan, currentTilt));
                //Tracer.Trace(string.Format("{0,4} mks {1,4:0} degrees", panMksLast, currentPan));
            }
        }

        public void setTilt(double degreesFromCenter, bool rightGun)
        {
            double mks = 0.0d;

            if (rightGun)
            {
                currentTiltGunRight = degreesFromCenter;

                mks = PanTiltAlignment.getInstance().mksTiltGunRight(degreesFromCenter);
            }
            else
            {
                currentTiltGunLeft = degreesFromCenter;

                mks = PanTiltAlignment.getInstance().mksTiltGunLeft(degreesFromCenter);
            }

            setTiltMks((int)mks, rightGun);
        }

        public void setTiltMks(int tiltMks, bool rightGun)
        {
            if (rightGun && tiltMks != tiltMksLastGunRight)
            {
                tiltMksLastGunRight = tiltMks;

                TrySetTarget(ServoChannelMap.rightGunTilt, (ushort)(tiltMks << 2));

                //Tracer.Trace(string.Format("Right Pan: {0:0}\r\nTilt: {1:0}", currentPan, currentTilt));
                //Tracer.Trace(string.Format("{0,4} mks {1,4:0} degrees", tiltMksLast, currentTilt));
            }
            else if (!rightGun && tiltMks != tiltMksLastGunLeft)
            {
                tiltMksLastGunLeft = tiltMks;

                TrySetTarget(ServoChannelMap.leftGunTilt, (ushort)(tiltMks << 2));

                //Tracer.Trace(string.Format("Left Pan: {0:0}\r\nTilt: {1:0}", currentPan, currentTilt));
                //Tracer.Trace(string.Format("{0,4} mks {1,4:0} degrees", tiltMksLast, currentTilt));
            }
        }

        #region Servo controls

        /// <summary>
        /// Attempts to set the target (width of pulses sent) of a channel.
        /// </summary>
        /// <param name="channel">Channel number - from 0 to 23.</param>
        /// <param name="target">
        ///   Target, in units of quarter microseconds.  For typical servos,
        ///   6000 is neutral and the acceptable range is 4000-8000.
        ///   A good servo will take 880 to 2200 us (3520 to 8800 in quarters)
        /// </param>
        public void TrySetTarget(Byte channel, UInt16 target)
        {
            try
            {
                int index = channel / ServoChannelMap.CHANNELS_PER_DEVICE;
                channel = (byte)(channel % ServoChannelMap.CHANNELS_PER_DEVICE);

                using (Usc device = connectToDevice(index))  // Find a device and temporarily connect.
                {

                    //Console.WriteLine("    (s) device: {0}    channel: {1}    target: {2}", device.getSerialNumber(), channel, target);

                    device.setTarget(channel, target);

                    // device.Dispose() is called automatically when the "using" block ends,
                    // allowing other functions and processes to use the device.
                }
            }
            catch (Exception exception)  // Handle exceptions by displaying them to the user.
            {
                Console.WriteLine(exception);
            }
        }

        public void TrySetTarget(List<ChannelValuePair> channelValues)
        {
            try
            {
                var groupedByDevice = from a in channelValues
                                      group a by a.Channel / ServoChannelMap.CHANNELS_PER_DEVICE into g
                                      select new { deviceIndex = g.Key, deviceChannelValues = g };

                foreach (var devGrp in groupedByDevice)
                {

                    using (Usc device = connectToDevice(devGrp.deviceIndex))  // Find a device and temporarily connect.
                    {
                        foreach (ChannelValuePair cvp in devGrp.deviceChannelValues)
                        {
                            byte channel = (byte)(cvp.Channel % ServoChannelMap.CHANNELS_PER_DEVICE);
                            //Console.WriteLine("    (m) device: {0}    channel: {1}    target: {2}", device.getSerialNumber(), channel, cvp.Target);
                            device.setTarget(channel, cvp.Target);
                        }
                        // device.Dispose() is called automatically when the "using" block ends,
                        // allowing other functions and processes to use the device.
                    }
                }
            }
            catch (Exception exception)  // Handle exceptions by displaying them to the user.
            {
                Console.WriteLine(exception);
            }
        }

        public int TryGetTarget(Byte channel)
        {
            int ret = 0;

            try
            {
                int index = channel / ServoChannelMap.CHANNELS_PER_DEVICE;
                channel = (byte)(channel % ServoChannelMap.CHANNELS_PER_DEVICE);

                using (Usc device = connectToDevice(index))  // Find a device and temporarily connect.
                {
                    ServoStatus[] servos;
                    device.getVariables(out servos);
                    ret = servos[channel].position;

                    // device.Dispose() is called automatically when the "using" block ends,
                    // allowing other functions and processes to use the device.
                }
            }
            catch (Exception exception)  // Handle exceptions by displaying them to the user.
            {
                Console.WriteLine(exception);
            }

            return ret;
        }

        // we assume that devices serial numbers are in increasing sequence:
        Comparison<DeviceListItem> deviceComparer = new Comparison<DeviceListItem>((a, b) => int.Parse(a.serialNumber) - int.Parse(b.serialNumber));

        /// <summary>
        /// Connects to a Maestro using native USB and returns the Usc object
        /// representing that connection.  When you are done with the
        /// connection, you should close it using the Dispose() method so that
        /// other processes or functions can connect to the device later.  The
        /// "using" statement can do this automatically for you.
        /// </summary>
        public Usc connectToDevice(int index)
        {
            // Get a list of all connected devices of this type.
            List<DeviceListItem> connectedDevices = Usc.getConnectedDevices();
            connectedDevices.Sort(deviceComparer);

            DeviceListItem dli = connectedDevices[index];

            //foreach (DeviceListItem dli in connectedDevices)
            {
                // If you have multiple devices connected and want to select a particular
                // device by serial number, you could simply add a line like this:
                //   if (dli.serialNumber != "00012345"){ continue; }

                Usc device = new Usc(dli); // Connect to the device.
                return device;             // Return the device.
            }
            throw new Exception("Could not find Pololu device.\r\nMake sure it's plugged into USB\r\nCheck your Device Manager.\r\nPololu Mini Maestro 12 needed.");
        }

        #endregion // Servo controls
    }
}
