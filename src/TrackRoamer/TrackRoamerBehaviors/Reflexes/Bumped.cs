
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.ComponentModel;

using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Net.Mime;
using W3C.Soap;

using Microsoft.Ccr.Core;
using Microsoft.Dss.Core;
using Microsoft.Dss.Core.Attributes;
using Microsoft.Dss.ServiceModel.Dssp;
using Microsoft.Dss.Services.Serializer;
using Microsoft.Dss.ServiceModel.DsspServiceBase;
using Microsoft.Dss.Core.DsspHttp;
using Microsoft.Dss.Core.DsspHttpUtilities;

using bumper = Microsoft.Robotics.Services.ContactSensor.Proxy;
//using bumper = TrackRoamer.Robotics.Services.TrackRoamerServices.Bumper.Proxy;
using drive = Microsoft.Robotics.Services.Drive.Proxy;
using sicklrf = Microsoft.Robotics.Services.Sensors.SickLRF.Proxy;
using kinect = Microsoft.Kinect;

using dssp = Microsoft.Dss.ServiceModel.Dssp;

using TrackRoamer.Robotics.Utility.LibSystem;
using TrackRoamer.Robotics.LibMapping;
using TrackRoamer.Robotics.LibBehavior;

namespace TrackRoamer.Robotics.Services.TrackRoamerBehaviors
{
    partial class TrackRoamerBehaviorsService : DsspServiceBase
    {
        #region Bumped()

        private Random btRand = new Random(DateTime.Now.Millisecond);

        private bool isSensingRearObstruction(bool leftWhiskerPressed, bool rightWhiskerPressed, bumper.ContactSensorArrayState bumpersState)
        {
            return !leftWhiskerPressed && !rightWhiskerPressed;
        }

        /// <summary>
        /// Stops the robot. If the robot was going forward it backs up.
        /// Keep in mind that a "hardware" stop has already been initiated in the Power Brick, so by the time we get here wheels are stopped.
        /// </summary>
        protected void Bumped(bool leftWhiskerPressed, bool rightWhiskerPressed, bumper.ContactSensorArrayState bumpersState)
        {
             LogInfo("TrackRoamerBehaviorsService: Bumped()   _state.Velocity=" + _state.Velocity + "   _state.MovingState=" + _state.MovingState);

            string whatIsBumped;
            int turnFactor; // = btRand.NextDouble() > 0.5d ? 1 : -1;

            if (leftWhiskerPressed && rightWhiskerPressed)
            {
                whatIsBumped = "Both whiskers";
                turnFactor = 0;    // straight back
            }
            if (leftWhiskerPressed)
            {
                whatIsBumped = "Left whisker";
                turnFactor = -1;    // turning a bit to the right, to avoid obstacle to the left
            }
            else if (rightWhiskerPressed)
            {
                whatIsBumped = "Right whisker";
                turnFactor = 1;    // turning a bit to the left, to avoid obstacle to the right
            }
            else
            {
                whatIsBumped = "Proximity array";
                turnFactor = btRand.NextDouble() > 0.5d ? 1 : -1;
            }

            Talker.Say(3, "bumped " + whatIsBumped);

            if (!_testBumpMode && _state.Velocity < 0)
            {
                // we are moving backwards,
                // front whiskers ignored when we move backwards. Others cause immediate stop:
                if (isSensingRearObstruction(leftWhiskerPressed, rightWhiskerPressed, bumpersState))
                {
                    LogInfo("TrackRoamerBehaviorsService: Bumped() - only Rear proximity sensors pressed while moving backwards, stopping...");

                    // either a rear bumper or both front and rear
                    // bumpers are pressed. STOP!
                    StopTurning();
                    //StopMoving();

                    // whatever it was, we didn't expect it. Let higher level decision-making take over, may be look around, do mapping: 
                    _state.MovingState = MovingState.Unknown;
                    _state.Countdown = 3;
                }
                else
                {
                    LogInfo("TrackRoamerBehaviorsService: Bumped() while moving backwards, whisker press ignored (robot stopped anyway)");

                    Talker.Say(4, whatIsBumped + " ignored");
                    // well, motors are stopped and we are not getting completion by encoders.
                    _state.MovingState = MovingState.Unknown;
                    _state.Countdown = 3;
                }
            }
            else
            {
                // we are moving forward, or in test mode.
                // _testBumpMode always ends here - even if we are stationary or moving backwards
                if (_state.MovingState != MovingState.BumpedBackingUp)
                {
                    _state.MovingState = MovingState.BumpedBackingUp;
                    lastBumpedBackingUpStarted = DateTime.Now;

                    int angle = BackupAngleDegrees * turnFactor;

                    Tracer.Trace("TrackRoamerBehaviorsService: Bumped() - " + whatIsBumped + " pressed, backing up by " + (-BackupDistanceMm) + " mm  turning " + angle + " degrees");

                    Talker.Say(4, "backing up");

                    // only a front bumper is pressed.
                    // move back <BackupDistance> mm;

                    TurnAndMoveParameters tamp = new TurnAndMoveParameters()
                    {
                        distance = BackupDistanceMm,
                        speed = (int)Math.Round(ModerateBackwardVelocityMmSec),
                        rotatePower = ModerateTurnPower,
                        rotateAngle = angle
                    };

                    Port<bool> completionPort = BackUpTurnWait(tamp);

                    // start movement
                    Activate(Arbiter.Receive(false, completionPort,
                            delegate(bool b)
                            {
                                LogInfo("TrackRoamerBehaviorsService: Bumped() delegate - ++++++++++++ BackUpTurn done ++++++++++++++++++++++++++++++++++++++++++++++++++++");
                                Talker.Say(4, "done backing up");

                                // done backing up; let the decision making process take over:
                                _state.MovingState = MovingState.Unknown;
                                _state.Countdown = 3;
                            }
                        )
                    );
                    // exiting here with MovingState.BumpedBackingUp, while the delegate waits for completion.
                }
                else
                {
                    Talker.Say(4, "whisker press ignored");
                    // well, motors are stopped and we are not getting completion by encoders.
                    _state.MovingState = MovingState.Unknown;
                    _state.Countdown = 3;
                }
            }

            LogInfo("TrackRoamerBehaviorsService: Bumped()  exiting - _state.MovingState=" + _state.MovingState);
        }

        #endregion // Bumped()
    }
}
