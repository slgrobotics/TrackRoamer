//#define TRACEDEBUG
//#define TRACEDEBUGTICKS
//#define TRACELOG

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Ccr.Core;
using Microsoft.Dss.ServiceModel.DsspServiceBase;

using TrackRoamer.Robotics.Utility.LibSystem;
using TrackRoamer.Robotics.LibBehavior;

using pololumaestro = TrackRoamer.Robotics.Hardware.PololuMaestroService.Proxy;


namespace TrackRoamer.Robotics.Services.TrackRoamerBehaviors
{
    public enum ServoChannelState
    {
        Unknown,
        Off,
        On
    }

    /// <summary>
    /// LightsHelper keeps track of all lights connected to two Pololu Mini Maestro 12 controllers.
    /// It turns them on/off based on commands from Collision module and Behaviors.
    /// </summary>
    public class LightsHelper : Dictionary<byte, ServoChannelState>
    {
        public LightsHelper()
        {
            for (byte i = 0; i < ServoChannelMap.channelsCount; i++)
            {
                this.Add(i, ServoChannelState.Unknown);
            }
        }

        public bool IsOn(byte channel)
        {
            return this[channel] == ServoChannelState.On;
        }

        public bool IsOff(byte channel)
        {
            return this[channel] == ServoChannelState.Off;
        }

        public void MarkOn(byte channel)
        {
            this[channel] = ServoChannelState.On;
        }

        public void MarkOff(byte channel)
        {
            this[channel] = ServoChannelState.Off;
        }
    }

    /// <summary>
    /// Lights related portion of Trackroamer Behaviors.
    /// We use Servo setters from the DriveBehaviorServoAndGun.cs
    /// </summary>
    partial class TrackRoamerBehaviorsService : DsspServiceBase
    {

        private void HeadlightsOn()
        {
            //Tracer.Trace("HeadlightsOn");

            HeadlightsOnOff(true, true);
        }

        private void HeadlightsOff()
        {
            //Tracer.Trace("HeadlightsOff");

            HeadlightsOnOff(false, false);
        }

        private void HeadlightsOnOff(bool onLeft, bool onRight)
        {
            List<pololumaestro.ChannelValuePair> channelValues = new List<pololumaestro.ChannelValuePair>();

            byte[] channels = new byte[] { ServoChannelMap.leftHeadlight, ServoChannelMap.rightHeadlight};
            bool[] ons = new bool[] { onLeft, onRight };

            for (int i = 0; i < 2; i++)
            {
                byte channel = channels[i];
                bool on = ons[i];
                ushort target = (ushort)((on ? 2000 : 1000) << 2);

                if (on && !_lightsHelper.IsOn(channel) || !on && !_lightsHelper.IsOff(channel))
                {
                    channelValues.Add(new pololumaestro.ChannelValuePair() { Channel = channel, Target = target });
                    if (on)
                    {
                        _lightsHelper.MarkOn(channel);
                    }
                    else
                    {
                        _lightsHelper.MarkOff(channel);
                    }
                }
            }

            if (channelValues.Any())
            {
                LightsSet(channelValues);
            }
        }

        private void AllLightsOff()
        {
            List<pololumaestro.ChannelValuePair> channelValues = new List<pololumaestro.ChannelValuePair>();

            ushort target = (ushort)(1000 << 2);

            for (byte j = 0; j < ServoChannelMap.channelsCount; j++)
            {
                if (!ServoChannelMap.notLightChannels.Contains(j))
                {
                    channelValues.Add(new pololumaestro.ChannelValuePair() { Channel = j, Target = target });
                    _lightsHelper.MarkOff(j);
                }
            }

            channelValues.Add(new pololumaestro.ChannelValuePair() { Channel = ServoChannelMap.leftHeadlight, Target = target });
            _lightsHelper.MarkOff(ServoChannelMap.leftHeadlight);
            channelValues.Add(new pololumaestro.ChannelValuePair() { Channel = ServoChannelMap.rightHeadlight, Target = target });
            _lightsHelper.MarkOff(ServoChannelMap.rightHeadlight);

            LightsSet(channelValues);
        }

        /// <summary>
        /// Attempts to set the target (width of pulses sent) for multiple channels.
        /// </summary>
        /// <param name="channelValues"></param>
        private void LightsSet(List<pololumaestro.ChannelValuePair> channelValues)
        {
            try
            {
                pololumaestro.PololuMaestroCommand cmd = new pololumaestro.PololuMaestroCommand() { Command = "set", ChannelValues = channelValues };

                _pololuMaestroPort.Post(new pololumaestro.SendPololuMaestroCommand(cmd));
            }
            catch (Exception exception)
            {
                LogError(exception);
            }
        }

        #region Domain specific lights actions

        private void SetLightsMustStop(bool on)
        {
            lights[ServoChannelMap.centerLeftRed] = on;
            lights[ServoChannelMap.centerRightRed] = on;
        }

        private void SetLightsWhiskers()
        {
            lights[ServoChannelMap.centerLeftYellow]  = _state.MostRecentWhiskerLeft;
            lights[ServoChannelMap.centerRightYellow] = _state.MostRecentWhiskerRight;
        }

        private void SetLightsCanMoveForward(bool on)
        {
            lights[ServoChannelMap.centerLeftGreen] = on;
            lights[ServoChannelMap.centerRightGreen] = on;
        }

        private void SetLightsCanMoveBackwards(bool on)
        {
            lights[ServoChannelMap.rearLeftGreen] = on;
            lights[ServoChannelMap.rearRightGreen] = on;
            lights[ServoChannelMap.rearLeftRed] = !on;
            lights[ServoChannelMap.rearRightRed] = !on;
            lights[ServoChannelMap.platformLeftRed] = !on;
            lights[ServoChannelMap.platformRightRed] = !on;
        }

        private void SetLightsCanTurnLeft(bool on)
        {
            lights[ServoChannelMap.frontLeftGreen] = on;
        }

        private void SetLightsCanTurnRight(bool on)
        {
            lights[ServoChannelMap.frontRightGreen] = on;
        }

        private void SetLightsTrackingSkeleton(bool on)
        {
            lights[ServoChannelMap.platformLeftGreen] = on;
            lights[ServoChannelMap.platformRightGreen] = on;
        }

        private void SetLightsTrackingRedShirt(bool on)
        {
            lights[ServoChannelMap.platformLeftYellow] = on;
            lights[ServoChannelMap.platformRightYellow] = on;
        }

        #endregion // Domain specific lights actions

        #region LightsControlLoop()

        private int LightsControlWaitIntervalMs = 200;     // time to wait between lights control cycles.

        private int lightsTestMode = 0;

        public void SetLightsTest() { lightsTestMode = 1; }
        public void SetLightsNormal() { lightsTestMode = 0; }

        /// <summary>
        ///  set items here to true/false to have the corresponding lights lit.
        ///  See ServoChannelMap for channel assignments
        /// </summary>
        private bool[] lights = new bool[ServoChannelMap.channelsCount];

        /// <summary>
        /// Lights control loop - runs tests or mirrors the "lights" array
        /// </summary>
        /// <returns>A standard CCR iterator.</returns>
        private IEnumerator<ITask> LightsControlLoop()
        {
            int i = 0;

            List<pololumaestro.ChannelValuePair> channelValues = new List<pololumaestro.ChannelValuePair>();

            while (!_state.Dropping)
            {
                //lights[i % ServoChannelMap.channelsCount] = !lights[i % ServoChannelMap.channelsCount];         // test

                switch (lightsTestMode)
                {
                    case 0:     // not a test, normal operation - mirror "lights" array 

                        for (byte j = 0; j < ServoChannelMap.channelsCount; j++)
                        {
                            if (!ServoChannelMap.notLightChannels.Contains(j))
                            {
                                bool lightOn = lights[j];

                                // only act on the channels whose state is different from the "lights" array:
                                if (lightOn && !_lightsHelper.IsOn(j) || !lightOn && !_lightsHelper.IsOff(j))
                                {
                                    ushort target = (ushort)((lightOn ? 2000 : 1000) << 2);

                                    channelValues.Add(new pololumaestro.ChannelValuePair() { Channel = j, Target = target });
                                }
                            }
                        }

                        break;

                    case 1:     // test - blink all lights
                        {
                            ushort target = (ushort)(((i % 2 == 0) ? 2000 : 1000) << 2);

                            for (byte j = 0; j < ServoChannelMap.channelsCount; j++)
                            {
                                if (!ServoChannelMap.notLightChannels.Contains(j))
                                {
                                    channelValues.Add(new pololumaestro.ChannelValuePair() { Channel = j, Target = target });
                                }
                            }

                            //channelValues.Add(new pololumaestro.ChannelValuePair() { Channel = ServoChannelMap.rearLeftRed, Target = target });
                            //channelValues.Add(new pololumaestro.ChannelValuePair() { Channel = ServoChannelMap.rearLeftGreen, Target = target });

                            //Tracer.Trace("Light control loop: " + target);
                        }
                        break;
                }

                if (channelValues.Any())
                {
                    LightsSet(channelValues);
                    channelValues = new List<pololumaestro.ChannelValuePair>();
                }

                i++;

                // poll 5 times a sec
                yield return TimeoutPort(LightsControlWaitIntervalMs).Receive();
            }
        }

        #endregion // LightsControlLoop()

    }
}
