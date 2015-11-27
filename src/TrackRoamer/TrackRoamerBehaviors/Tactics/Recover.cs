
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
        #region Recover()

        /// <summary>
        /// Implements the "Recovering" state.
        /// </summary>
        private void Recover()
        {
            LogInfo("TrackRoamerBehaviorsService: Recover()");

            Kata kata = KataHelper.KataByCollisionState(_state.collisionState, "avoid to").FirstOrDefault();

            if (kata != null)
            {
                Talker.Say(5, "Kata " + kata.name);

                SpawnIterator<Kata, Handler>(
                    kata,
                    delegate()
                    {
                        //Talker.Say(5, "kata success: " + kata.success + "  count: " + kata.successfulStepsCount);
                        //Talker.Say(5, "kata success");
                        Tracer.Trace("kata " + kata.name + " success");
                    },
                    KataRunner);        // KataRunner sets MovingState to MovingState.InTransition
            }
            else
            {
                Talker.Say(10, "no kata!");

                LogError("DriveBehaviorServiceBase: BehaviorKata() - cannot find appropriate Kata");
            }

            LogInfo("DriveBehaviorServiceBase: BehaviorKata() finished");
        }

        #endregion // Recover()

        #region  Recovery and bumper-related moves

        protected Port<bool> BackUpTurnWait(TurnAndMoveParameters tamp)
        {
            Port<bool> butwCompletionPort = new Port<bool>();

            // start movement
            SpawnIterator<Port<bool>, TurnAndMoveParameters>(butwCompletionPort, tamp, BackUpTurnIterator);

            return butwCompletionPort;
        }

        protected IEnumerator<ITask> BackUpTurnIterator(Port<bool> butwCompletionPort, TurnAndMoveParameters tamp)
        {
            bool lastOpSuccess = true;
            Fault fault = null;

            LogInfo("[[[[[[[[[[[[[[[[[[[[ BackUpTurnIterator() starting ");

            // First backup a little.
            yield return Arbiter.Choice(
                Translate(-tamp.distance, tamp.speed),
                    delegate(DefaultUpdateResponseType response) { lastOpSuccess = true; },
                    delegate(Fault f) { lastOpSuccess = false; fault = f; }
            );

            // If the DriveDistance was accepted, then wait for it to complete, and start turn.
            if (!lastOpSuccess)
            {
                LogError("[[[[[[[[[[[[[[[[[[[[ BackUpTurnIterator() - Translate FAULT : " + fault);
            }
            else
            {
                DriveStageContainer driveStage = new DriveStageContainer();
                yield return WaitForCompletion(driveStage);
                LogInfo("WaitForCompletion() returned: " + driveStage.DriveStage);

                lastOpSuccess = driveStage.DriveStage == drive.DriveStage.Completed;

                if (lastOpSuccess)
                {
                    LogInfo("[[[[[[[[[[[[[[[[[[[[ BackUpTurnIterator() - BackUp portion completed");

                    if (Math.Abs(tamp.rotateAngle) > 5)
                    {
                        // Wait for settling time
                        //yield return Timeout(settlingTime);

                        // now we can Turn:
                        yield return Arbiter.Choice(
                            TurnByAngle(tamp.rotateAngle, ModerateTurnPower),
                            delegate(DefaultUpdateResponseType response) { lastOpSuccess = true; },
                            delegate(Fault f) { lastOpSuccess = false; fault = f; }
                        );

                        // If the RotateDegrees was accepted, then wait for it to complete.
                        // It is important not to wait if the request failed.
                        if (lastOpSuccess)
                        {
                            TrackRoamerBehaviorsState state = _state;
                            state.IsTurning = true;
                            state.LastTurnStarted = state.LastTurnCompleted = DateTime.Now;     // reset watchdog

                            yield return WaitForCompletion(driveStage);
                            LogInfo("WaitForCompletion() returned: " + driveStage.DriveStage);

                            state.IsTurning = false;
                            state.LastTurnCompleted = DateTime.Now;
                            if (_mapperVicinity.turnState != null)
                            {
                                _mapperVicinity.turnState.finished = state.LastTurnCompleted;
                                _mapperVicinity.turnState.hasFinished = true;
                            }
                        }
                        else
                        {
                            if (_mapperVicinity.turnState != null)
                            {
                                _mapperVicinity.turnState.finished = DateTime.Now;
                                _mapperVicinity.turnState.hasFinished = true;
                                _mapperVicinity.turnState.wasCanceled = true;
                            }
                            LogError("[[[[[[[[[[[[[[[[[[[[ BackUpTurnIterator() - Turn FAULT : " + fault);
                        }
                    }
                }
                else
                {
                    LogInfo("[[[[[[[[[[[[[[[[[[[[ BackUpTurnIterator() - BackUp portion canceled");
                }
            }

            LogInfo("[[[[[[[[[[[[[[[[[[[[ BackUpTurnIterator() completed");

            butwCompletionPort.Post(true);

            // done
            yield break;
        }

        #endregion // Recovery and bumper-related moves
    }
}
