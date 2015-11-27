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

namespace Trackroamer.Library.LibAnimatronics
{
    public class BrickConnectorArduino : IBrickConnector
    {
        public double currentPan { get; private set; }
        public double currentTilt { get; private set; }
        public double currentJaw { get; private set; }

        public int panMksLast { get; private set; }
        public int tiltMksLast { get; private set; }
        public int jawMksLast { get; private set; }

        private string comPort;
        private ArduinoComm arduinoComm = new ArduinoComm();

        public void Open(string args)
        {
            comPort = args;

            arduinoComm.Open(comPort);
        }

        public void Close()
        {
            arduinoComm.Close();
        }

        /// <summary>
        /// preferred method for Head pan/tilt control
        /// </summary>
        /// <param name="panDegreesFromCenter"></param>
        /// <param name="tiltDegreesFromCenter"></param>
        public void setPanTilt(double panDegreesFromCenter, double tiltDegreesFromCenter)
        {
            currentPan = panDegreesFromCenter;

            double mksPan = PanTiltAlignment.getInstance().mksPan(panDegreesFromCenter);

            int panMks = (int)mksPan;


            currentTilt = tiltDegreesFromCenter;

            double mksTilt = PanTiltAlignment.getInstance().mksTilt(tiltDegreesFromCenter);

            int tiltMks = (int)mksTilt;


            if (panMks != panMksLast || tiltMks != tiltMksLast)
            {
                panMksLast = panMks;
                tiltMksLast = tiltMks;

                Debug.WriteLine("setPanTilt: panMks=" + panMks + "  tiltMks=" + tiltMks);

                ToArduino toArduino1 = new ToArduino { channel = (int)AnimationChannels.PAN, command = (int)AnimationCommands.SET_VALUE, commandValues = new int[] { panMks } };
                ToArduino toArduino2 = new ToArduino { channel = (int)AnimationChannels.TILT, command = (int)AnimationCommands.SET_VALUE, commandValues = new int[] { tiltMks } };

                SendToArduino2(toArduino1, toArduino2);
            }
        }


        /// <summary>
        /// Head pan
        /// </summary>
        /// <param name="degreesFromCenter"></param>
        public void setPan(double degreesFromCenter)
        {
            currentPan = degreesFromCenter;

            double mks = PanTiltAlignment.getInstance().mksPan(degreesFromCenter);

            setPanMks((int)mks);
        }

        public void setPanMks(int panMks)
        {
            if (panMks != panMksLast)
            {
                Debug.WriteLine(string.Format("setPanMks: Pan: {0,4:0} -> {1,4:0} mks", panMksLast, panMks));

                ToArduino toArduino = new ToArduino { channel = (int)AnimationChannels.PAN, command = (int)AnimationCommands.SET_VALUE, commandValues = new int[] { panMks } };

                SendToArduino(toArduino);

                panMksLast = panMks;
            }
        }

        /// <summary>
        /// Head tilt
        /// </summary>
        /// <param name="degreesFromCenter"></param>
        public void setTilt(double degreesFromCenter)
        {
            currentTilt = degreesFromCenter;

            double mks = PanTiltAlignment.getInstance().mksTilt(degreesFromCenter);

            setTiltMks((int)mks);
        }

        public void setTiltMks(int tiltMks)
        {
            if (tiltMks != tiltMksLast)
            {
                Debug.WriteLine(string.Format("setTiltMks: Tilt: {0,4:0} -> {1,4:0} mks", tiltMksLast, tiltMks));

                ToArduino toArduino = new ToArduino { channel = (int)AnimationChannels.TILT, command = (int)AnimationCommands.SET_VALUE, commandValues = new int[] { tiltMks } };

                SendToArduino(toArduino);

                tiltMksLast = tiltMks;
            }
        }

        /// <summary>
        /// Jaw open angle
        /// </summary>
        /// <param name="degreesFromCenter"></param>
        public void setJaw(double degreesFromCenter)
        {
            currentJaw = degreesFromCenter;

            double mks = PanTiltAlignment.getInstance().mksTilt(degreesFromCenter);

            setJawMks((int)mks);
        }

        public void setJawMks(int jawMks)
        {
            if (jawMks != jawMksLast)
            {
                Debug.WriteLine(string.Format("setJawMks: Jaw: {0,4:0} -> {1,4:0} mks", jawMksLast, jawMks));

                ToArduino toArduino = new ToArduino { channel = (int)AnimationChannels.JAW, command = (int)AnimationCommands.SET_VALUE, commandValues = new int[] { jawMks } };

                SendToArduino(toArduino);

                jawMksLast = jawMks;
            }
        }

        public void clearAnimations()
        {
            Debug.WriteLine("clearAnimations()");

            ToArduino toArduino = new ToArduino { channel = (int)AnimationChannels.ANIMATIONS, command = (int)AnimationCommands.ANIMATIONS_CLEAR };

            SendToArduino(toArduino);
        }

        public void setDefaultAnimations()
        {
            Debug.WriteLine("setDefaultAnimations()");

            ToArduino toArduino = new ToArduino { channel = (int)AnimationChannels.ANIMATIONS, command = (int)AnimationCommands.ANIMATIONS_DEFAULT };

            SendToArduino(toArduino);
        }

        public void setAnimation(Animation anim, double scale = 1.0d, bool doRepeat = false)
        {
            SendToArduino(anim.ToArduino(scale, doRepeat));
        }

        private void SendToArduino(ToArduino toArduino)
        {
            lock (arduinoComm.outputQueue)
            {
                Debug.WriteLine("SendToArduino:  " + toArduino);
                arduinoComm.outputQueue.Enqueue(toArduino);
            }
        }

        private void SendToArduino2(ToArduino toArduino1, ToArduino toArduino2)
        {
            lock (arduinoComm.outputQueue)
            {
                arduinoComm.outputQueue.Enqueue(toArduino1);
                arduinoComm.outputQueue.Enqueue(toArduino2);
            }
        }
    }
}
