
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
		#region constants and parameters
		// The explorer uses there constants to evaluate open space 
		// and adjust speed and rotation of the differential drive.
		// Change those values to match your robot.
		// You could expose them on the explorers state type and make the
		// service configurable.

        // SceneScale shrinks (<1) or expands (>1) all decision-related zone constants below.
        // when SceneScale=0.2, the robot thinks it is a mouse and can fit any gap, when SceneScale=5 - an elephant and nothing around is wide enough for him 
        const double SceneScale = 1.0d;
        //const double SceneScale = 0.7d;

		/// <summary>
		/// Amount to backup when hitting an obstacle.
		/// </summary>
		const int BackupDistanceMm = 300;       // mm
		const int BackupAngleDegrees = 45;      // degrees, will be random left or right; if 0 not turning after backup

		/// <summary>
		/// The width of the corridor that must be safe in order to get from mapping to moving.
		/// </summary>
        const int CorridorWidthMappingMm = (int)(750 * SceneScale); // mm   750

        /// <summary>
        /// The with of the corridor in which obstacles affect velocity.
        /// </summary>
        const int CorridorWidthMovingMm = (int)(1000 * SceneScale); // mm   1000

        /// <summary>
		/// If an obstacle comes within thisdistance the robot stops moving.
		/// </summary>
		const int ObstacleDistanceMm = (int)(600 * SceneScale); // mm  600

		/// <summary>
		/// If the free space is at least this distance the robot operates at 1/2 max. velocity otherwise 
		/// the robot slows down to 1/4 of max. vel.
		/// </summary>
        const int AwareOfObstacleDistanceMm = (int)(1300 * SceneScale); // mm  1300

		/// <summary>
		/// If the robot is mapping and has this much open space ahead it stops mapping
		/// and enters the space.
		/// </summary>
        const int SafeDistanceMm = (int)(1800 * SceneScale); // mm  1800

		/// <summary>
		/// The minimum free distance that is required to drive with max. velocity.
		/// </summary>
        const int FreeDistanceMm = (int)(5000 * SceneScale); // mm  2500

		#endregion

        #region Manual Control

        private int stopCount = 0;
        private DateTime lastStopCount;

        protected void ManualControl(SensorEventSource why)
        {
            if (!prevManualControl)
            {
                stopCount = 5;
                lastStopCount = DateTime.MinValue;
            }

            string command = _mapperVicinity.robotState.manualControlCommand;

            if (!string.IsNullOrEmpty(command))
            {
                Tracer.Trace("on manual control; command: " + command);

                if ("stop".Equals(command))
                {
                    stopCount = 5;
                    lastStopCount = DateTime.MinValue;
                }
                else if ("turn".Equals(command))
                {

                    int bestHeading = (int)_mapperVicinity.robotDirection.turnRelative;    // right - positive, left - negative

                    _state.NewHeading = bestHeading;

                    LogInfo("     bestHeading=" + bestHeading + currentCompass + "    (relative=" + bestHeading + ")");

                    rotateAngle = bestHeading;

                    SpawnIterator<TurnAndMoveParameters, Handler>(
                        new TurnAndMoveParameters()
                        {
                            rotateAngle = (int)(((double)_state.NewHeading) * turnHeadingFactor),
                            rotatePower = ModerateTurnPower,
                            speed = 0,
                            desiredMovingState = MovingState.LayingDown
                        },
                        delegate()
                        {
                        },
                        TurnByDegree);
                    //TurnAndMoveForward);
                }

                _mapperVicinity.robotState.manualControlCommand = string.Empty;
            }
            else if (why == SensorEventSource.LaserScanning)
            {
                Tracer.Trace("on manual control; listening");
            }
        }

        private void doStop()
        {
            if (stopCount > 0 && DateTime.Now > lastStopCount.AddSeconds(1.0d))
            {
                StopMoving();
                lastStopCount = DateTime.Now;
                stopCount--;
            }
        }

        #endregion // Manual Control

        #region Decide()

        private const double TurningWatchdogInterval = 8.0d;          // reset _state.IsTurning after so many seconds
        private const double InTransitionWatchdogInterval = 30.0d;    // reset MovingState.InTransition after so many seconds

        protected DateTime lastBumpedBackingUpStarted = DateTime.MinValue;

        private bool prevManualControl = true;

		/// <summary>
		/// called often, by the timer and any state change or incoming sensor notifications;
        /// make all decisions about immediate actions here. Normal decision making happens in DecisionMainLoop()
		/// </summary>
        /// <param name="why">0-laser, 1-Accelerometer, 2-Direction, 3-Proximity</param>
        protected void Decide(SensorEventSource why)
        {
            if (_state.collisionState == null)
            {
                _state.collisionState = new CollisionState();
            }

            if (_doDecisionDontMove)
            {
                return;
            }

            if (_mapperVicinity.robotState.manualControl)
            {
                ManualControl(why);

                prevManualControl = true;

                doStop();   // will only stop if stopCount > 0

                return;
            }
            prevManualControl = false;

            doStop();   // will only stop if stopCount > 0

            //LogInfo("TrackRoamerBehaviorsService: Decide()   MovingState=" + _state.MovingState);
            //Tracer.Trace("TrackRoamerBehaviorsService: Decide()   MovingState=" + _state.MovingState + " IsTurning=" + _state.IsTurning);

            //_laserData = simulatedLaser();     // you can call it here as well, so that real data frame, when it comes, is just replaced on the fly

            // consider resetting some state flags, if timing calls for it:

            // if the flag indicates Turning, but it's been a while since we initiated the turn - reset the flag:
            if (_state.IsTurning && ((DateTime.Now - _state.LastTurnCompleted).TotalSeconds > TurningWatchdogInterval || (DateTime.Now - _state.LastTurnStarted).TotalSeconds > TurningWatchdogInterval))
            {
                LogInfo("Decide(): reset isTurning flag");
                LogHistory(10, "Turning took too long");
                _state.IsTurning = false;
            }

            if (_state.MovingState == MovingState.BumpedBackingUp && (DateTime.Now - lastBumpedBackingUpStarted).TotalSeconds > TurningWatchdogInterval)
            {
                LogInfo("Decide(): reset MovingState.BumpedBackingUp  to Unknown");
                LogHistory(10, "BumpedBackingUp took too long");
                _state.MovingState = MovingState.Unknown;
            }

            if (_state.MovingState == MovingState.InTransition && (DateTime.Now - lastInTransitionStarted).TotalSeconds > InTransitionWatchdogInterval)
            {
                LogInfo("Decide(): reset MovingState.InTransition  to Unknown");
                LogHistory(10, "InTransition took too long");
                _state.MovingState = MovingState.Unknown;
            }

            // unit test behavior - just go forward
            if (_doDecisionStraightForward)
            {
                if (_state.MovingState != MovingState.BumpedBackingUp && _state.MovingState != MovingState.InTransition)
                {
                    LogHistory(10, "Decisions Disabled - FreeForwards");

                    SpawnIterator<TurnAndMoveParameters, Handler>(
                        new TurnAndMoveParameters()
                        {
                            speed = (int)Math.Round(ModerateForwardVelocityMmSec),
                            rotatePower = ModerateTurnPower,
                            desiredMovingState = MovingState.FreeForwards
                        },
                        delegate()
                        {
                        },
                        TurnAndMoveForward);
                }

                return;
            }
        }

        #endregion // Decide()

        #region Slam()

        /// <summary>
        /// SLAM takes all available measurements and produces probability based data that describes robot's location and surrounding objects.
        /// </summary>
        private void Slam()
        {
            // for now, take the most probable GPS reading and set robot location to it:

            GpsState gpsState = _state.gpsState;
            DateTime now = DateTime.Now;
            double gpsReadingTooOldSec = 3.0d;

            if (gpsState != null)
            {
                if (gpsState.GPGGA_LastUpdate.HasValue && (now - gpsState.GPGGA_LastUpdate.Value).TotalSeconds < gpsReadingTooOldSec)
                {
                    _mapperVicinity.robotPosition.moveTo(gpsState.GPGGA_Longitude, gpsState.GPGGA_Latitude, gpsState.GPGGA_AltitudeMeters);
                }
                else if (gpsState.GPGLL_LastUpdate.HasValue && (now - gpsState.GPGLL_LastUpdate.Value).TotalSeconds < gpsReadingTooOldSec)
                {
                    _mapperVicinity.robotPosition.moveTo(gpsState.GPGLL_Longitude, gpsState.GPGLL_Latitude);
                }
                else if (gpsState.GPRMC_LastUpdate.HasValue && (now - gpsState.GPRMC_LastUpdate.Value).TotalSeconds < gpsReadingTooOldSec)
                {
                    _mapperVicinity.robotPosition.moveTo(gpsState.GPRMC_Longitude, gpsState.GPRMC_Latitude);
                }

                // CourseDegrees:
                //state.GPVTG_CourseDegrees
            }
        }

        #endregion // Slam()

        #region Interaction()

        /// <summary>
        /// Interaction choses commands Strategy; it chooses what game (strategy) to play.
        /// </summary>
        private void Interaction()
        {
            // choose strategy:

            // set parameters so that the strategy can perform:
        }

        #endregion // Interaction()
    }
}
