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
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Trackroamer.Library.LibArduinoComm;

namespace Trackroamer.Library.LibHandHardware
{
    public class BrickConnectorArduino : IBrickConnector
    {
        public double currentShoulderPan { get; private set; }
        public double currentShoulderTilt { get; private set; }
        public double currentShoulderTurn { get; private set; }
        public double currentElbowAngle { get; private set; }
        public double currentThumb { get; private set; }
        public double currentIndexFinger { get; private set; }
        public double currentMiddleFinger { get; private set; }
        public double currentPinky { get; private set; }
        public double currentWristTurn { get; private set; }

        public int shoulderPanMksLast { get; private set; }
        public int shoulderTiltMksLast { get; private set; }
        public int shoulderTurnMksLast { get; private set; }
        public int elbowAngleMksLast { get; private set; }
        public int thumbMksLast { get; private set; }
        public int indexFingerMksLast { get; private set; }
        public int middleFingerMksLast { get; private set; }
        public int pinkyMksLast { get; private set; }
        public int wristTurnMksLast { get; private set; }

        private string comPort;
        private ArduinoComm arduinoComm = new ArduinoComm();

        public void Open(string args, int param)
        {
            comPort = args;

            arduinoComm.Open(comPort, param);
        }

        public void Close()
        {
            arduinoComm.Close();
        }

        #region Shoulder and Arm

        /// <summary>
        /// Shoulder pan
        /// </summary>
        /// <param name="degreesFromCenter"></param>
        public void setShoulderPan(double degreesFromCenter)
        {
            currentShoulderPan = degreesFromCenter;

            double mks = PanTiltAlignment.getInstance().mksPan(degreesFromCenter);

            setShoulderPanMks((int)mks);
        }

        public void setShoulderPanMks(int panMks)
        {
            if (panMks != shoulderPanMksLast)
            {
                Debug.WriteLine(string.Format("setPanMks: Pan: {0,4:0} -> {1,4:0} mks", shoulderPanMksLast, panMks));

                ToArduino toArduino = new ToArduino { channel = (int)HandChannels.SHOULDER_PAN, command = (int)HandCommands.SET_VALUE, commandValues = new int[] { panMks } };

                SendToArduino(toArduino);

                shoulderPanMksLast = panMks;
            }
        }

        /// <summary>
        /// Shoulder tilt
        /// </summary>
        /// <param name="degreesFromCenter"></param>
        public void setShoulderTilt(double degreesFromCenter)
        {
            currentShoulderTilt = degreesFromCenter;

            double mks = PanTiltAlignment.getInstance().mksTilt(degreesFromCenter);

            setShoulderTiltMks((int)mks);
        }

        public void setShoulderTiltMks(int tiltMks)
        {
            if (tiltMks != shoulderTiltMksLast)
            {
                Debug.WriteLine(string.Format("setShoulderTiltMks: Tilt: {0,4:0} -> {1,4:0} mks", shoulderTiltMksLast, tiltMks));

                ToArduino toArduino = new ToArduino { channel = (int)HandChannels.SHOULDER_TILT, command = (int)HandCommands.SET_VALUE, commandValues = new int[] { tiltMks } };

                SendToArduino(toArduino);

                shoulderTiltMksLast = tiltMks;
            }
        }

        public void setShoulderTurn(double degreesFromCenter)
        {
            currentShoulderTurn = degreesFromCenter;

            double mks = PanTiltAlignment.getInstance().mksTurn(degreesFromCenter);

            setShoulderTurnMks((int)mks);
        }

        public void setShoulderTurnMks(int turnMks)
        {
            if (turnMks != shoulderTurnMksLast)
            {
                Debug.WriteLine(string.Format("setShoulderTurnMks: Turn: {0,4:0} -> {1,4:0} mks", shoulderTurnMksLast, turnMks));

                ToArduino toArduino = new ToArduino { channel = (int)HandChannels.SHOULDER_TURN, command = (int)HandCommands.SET_VALUE, commandValues = new int[] { turnMks } };

                SendToArduino(toArduino);

                shoulderTurnMksLast = turnMks;
            }
        }

        public void setElbowAngle(double degreesFromCenter)
        {
            currentElbowAngle = degreesFromCenter;

            double mks = PanTiltAlignment.getInstance().mksTilt(degreesFromCenter);

            setElbowAngleMks((int)mks);
        }

        public void setElbowAngleMks(int elbowAngleMks)
        {
            if (elbowAngleMks != elbowAngleMksLast)
            {
                Debug.WriteLine(string.Format("setElbowAngleMks: Elbow: {0,4:0} -> {1,4:0} mks", elbowAngleMksLast, elbowAngleMks));

                ToArduino toArduino = new ToArduino { channel = (int)HandChannels.ELBOW_ANGLE, command = (int)HandCommands.SET_VALUE, commandValues = new int[] { elbowAngleMks } };

                SendToArduino(toArduino);

                elbowAngleMksLast = elbowAngleMks;
            }
        }

        #endregion // Shoulder and Arm

        #region Wrist and Hand

        public void setThumb(double degreesFromCenter)
        {
            currentThumb = degreesFromCenter;

            double mks = PanTiltAlignment.getInstance().mksTilt(degreesFromCenter);

            setThumbMks((int)mks);
        }

        public void setThumbMks(int thumbMks)
        {
            if (thumbMks != thumbMksLast)
            {
                Debug.WriteLine(string.Format("setThumbMks: Thumb: {0,4:0} -> {1,4:0} mks", thumbMksLast, thumbMks));

                ToArduino toArduino = new ToArduino { channel = (int)HandChannels.THUMB, command = (int)HandCommands.SET_VALUE, commandValues = new int[] { thumbMks } };

                SendToArduino(toArduino);

                thumbMksLast = thumbMks;
            }
        }

        public void setIndexFinger(double degreesFromCenter)
        {
            currentIndexFinger = degreesFromCenter;

            double mks = PanTiltAlignment.getInstance().mksTilt(degreesFromCenter);

            setIndexFingerMks((int)mks);
        }

        public void setIndexFingerMks(int indexFingerMks)
        {
            if (indexFingerMks != indexFingerMksLast)
            {
                Debug.WriteLine(string.Format("setIndexFingerMks: IndexFinger: {0,4:0} -> {1,4:0} mks", indexFingerMksLast, indexFingerMks));

                ToArduino toArduino = new ToArduino { channel = (int)HandChannels.INDEX_FINGER, command = (int)HandCommands.SET_VALUE, commandValues = new int[] { indexFingerMks } };

                SendToArduino(toArduino);

                indexFingerMksLast = indexFingerMks;
            }
        }

        public void setMiddleFinger(double degreesFromCenter)
        {
            currentMiddleFinger = degreesFromCenter;

            double mks = PanTiltAlignment.getInstance().mksTilt(degreesFromCenter);

            setMiddleFingerMks((int)mks);
        }

        public void setMiddleFingerMks(int middleFingerMks)
        {
            if (middleFingerMks != middleFingerMksLast)
            {
                Debug.WriteLine(string.Format("setMiddleFingerMks: MiddleFinger: {0,4:0} -> {1,4:0} mks", middleFingerMksLast, middleFingerMks));

                ToArduino toArduino = new ToArduino { channel = (int)HandChannels.MIDDLE_FINGER, command = (int)HandCommands.SET_VALUE, commandValues = new int[] { middleFingerMks } };

                SendToArduino(toArduino);

                middleFingerMksLast = middleFingerMks;
            }
        }

        public void setPinky(double degreesFromCenter)
        {
            currentPinky = degreesFromCenter;

            double mks = PanTiltAlignment.getInstance().mksTilt(degreesFromCenter);

            setPinkyMks((int)mks);
        }

        public void setPinkyMks(int pinkyMks)
        {
            if (pinkyMks != pinkyMksLast)
            {
                Debug.WriteLine(string.Format("setPinkyMks: Pinky: {0,4:0} -> {1,4:0} mks", pinkyMksLast, pinkyMks));

                ToArduino toArduino = new ToArduino { channel = (int)HandChannels.PINKY, command = (int)HandCommands.SET_VALUE, commandValues = new int[] { pinkyMks } };

                SendToArduino(toArduino);

                pinkyMksLast = pinkyMks;
            }
        }

        public void setWristTurn(double degreesFromCenter)
        {
            currentWristTurn = degreesFromCenter;

            double mks = PanTiltAlignment.getInstance().mksTilt(degreesFromCenter);

            setWristTurnMks((int)mks);
        }

        public void setWristTurnMks(int wristTurnMks)
        {
            if (wristTurnMks != wristTurnMksLast)
            {
                Debug.WriteLine(string.Format("setWristTurnMks: Wrist: {0,4:0} -> {1,4:0} mks", wristTurnMksLast, wristTurnMks));

                ToArduino toArduino = new ToArduino { channel = (int)HandChannels.WRIST_TURN, command = (int)HandCommands.SET_VALUE, commandValues = new int[] { wristTurnMks } };

                SendToArduino(toArduino);

                wristTurnMksLast = wristTurnMks;
            }
        }

        #endregion // Wrist and Hand

        #region Arduino sender

        private void SendToArduino(ToArduino toArduino)
        {
            lock (arduinoComm.outputQueue)
            {
                Debug.WriteLine("SendToArduino:  " + toArduino);
                arduinoComm.outputQueue.Enqueue(toArduino);
            }
        }

        //private void SendToArduino2(ToArduino toArduino1, ToArduino toArduino2)
        //{
        //    lock (arduinoComm.outputQueue)
        //    {
        //        arduinoComm.outputQueue.Enqueue(toArduino1);
        //        arduinoComm.outputQueue.Enqueue(toArduino2);
        //    }
        //}

        #endregion // Arduino sender
    }
}
