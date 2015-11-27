//#define TRACEDEBUG
//#define TRACEDEBUGTICKS
//#define TRACELOG

using System;
using System.Collections.Generic;

using W3C.Soap;
using Microsoft.Ccr.Core;
using Microsoft.Dss.ServiceModel.Dssp;
using Microsoft.Dss.ServiceModel.DsspServiceBase;

using drive = Microsoft.Robotics.Services.Drive.Proxy;

namespace TrackRoamer.Robotics.Services.TrackRoamerBehaviors
{
    partial class TrackRoamerBehaviorsService : DsspServiceBase
    {
        #region Kata Runner

        protected IEnumerator<ITask> KataRunner(Kata kata, Handler onComplete)
        {
            LogInfo("DriveBehaviorServiceBase: KataRunner(" + kata.name + ") Started" + currentCompass);

            kata.success = false;
            kata.successfulStepsCount = 0;

            bool lastOpSuccess = false;

            _state.MovingState = MovingState.InTransition;
            // onComplete handler may set MovingState to whatever appropriate. We can set MovingState.Unknown on any error or interruption, and at the end to tamp.desiredMovingState
            lastInTransitionStarted = DateTime.Now;

            foreach (KataStep kataStep in kata)
            {
                LogInfo("IP: KataRunner(" + kata.name + ") started step " + (kata.successfulStepsCount + 1) + " " + kataStep.name + currentCompass);

                kataStep.success = false;
                Fault fault = null;
                int rotateAngle = kataStep.rotateAngle;
                int distance = kataStep.distance;

                if (Math.Abs(rotateAngle) > 1)    // "rotate" step
                {
                    CollisionState collisionState = _state.collisionState;

                    if (collisionState == null || !kataStep.CanPerform(collisionState))
                    {
                        LogInfo("Error: KataRunner cannot perform due to CollisionState - on turn");
                        break;  // kata interrupted
                    }

                    LogInfo("IP: KataRunner Turn " + rotateAngle);

                    yield return Arbiter.Choice(
                        TurnByAngle(rotateAngle, kataStep.rotatePower * PowerScale),
                        delegate(DefaultUpdateResponseType response)
                        {
                            LogInfo("IP: KataRunner TurnByAngle accepted" + currentCompass);
                            lastOpSuccess = true;
                        },
                        delegate(Fault f)
                        {
                            LogInfo("Error: KataRunner TurnByAngle rejected: " + fault);
                            lastOpSuccess = false;
                            _state.MovingState = MovingState.Unknown;
                            fault = f;
                        }
                    );

                    // If the RotateDegrees was accepted, then wait for it to complete.
                    // It is important not to wait if the request failed.
                    if (lastOpSuccess)
                    {
                        TrackRoamerBehaviorsState state = _state;
                        state.IsTurning = true;    // can't trust laser while turning
                        state.LastTurnStarted = state.LastTurnCompleted = DateTime.Now;     // reset watchdog

                        DriveStageContainer driveStage = new DriveStageContainer();
                        yield return WaitForCompletion(driveStage);

                        LogInfo("OK: WaitForCompletion() returned: " + driveStage.DriveStage);

                        lastOpSuccess = driveStage.DriveStage == drive.DriveStage.Completed;

                        if (lastOpSuccess)
                        {
                            if (_mapperVicinity.turnState != null)
                            {
                                _mapperVicinity.turnState.finished = DateTime.Now;
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
                            LogInfo("op failure");
                            state.MovingState = MovingState.Unknown;
                        }

                        state.IsTurning = false;
                        state.LastTurnCompleted = DateTime.Now;

                        // make sure we display zero power:
                        _mapperVicinity.robotState.leftPower = 0.0d;
                        _mapperVicinity.robotState.rightPower = 0.0d;
                    }
                    else
                    {
                        break;
                    }
                }

                if (Math.Abs(distance) > 1)    // "translate" step
                {
                    // while we were rotating, collisionState might have changed
                    CollisionState collisionState = _state.collisionState;

                    if (collisionState == null || !kataStep.CanPerform(collisionState))
                    {
                        LogInfo("Error: KataRunner cannot perform due to CollisionState - on translate");
                        break;  // kata interrupted
                    }

                    LogInfo("IP: KataRunner Translate " + distance);

                    yield return Arbiter.Choice(
                        Translate(distance, kataStep.speed * PowerScale),
                        delegate(DefaultUpdateResponseType response) { lastOpSuccess = true; },
                        delegate(Fault f)
                        {
                            lastOpSuccess = false;
                            _state.MovingState = MovingState.Unknown;
                            fault = f;
                        }
                    );

                    // If the Translate was accepted, then wait for it to complete.
                    // It is important not to wait if the request failed.
                    if (lastOpSuccess)
                    {
                        DriveStageContainer driveStage = new DriveStageContainer();
                        yield return WaitForCompletion(driveStage);
                        LogInfo("WaitForCompletion() returned: " + driveStage.DriveStage);

                        lastOpSuccess = driveStage.DriveStage == drive.DriveStage.Completed;

                        // make sure we display zero power:
                        _mapperVicinity.robotState.leftPower = 0.0d;
                        _mapperVicinity.robotState.rightPower = 0.0d;
                    }

                    if (!lastOpSuccess)
                    {
                        break;
                    }
                }

                LogInfo("KataRunner(" + kata.name + ") finished step=" + kataStep.name + currentCompass);
                kata.successfulStepsCount++;
            }

            kata.success = kata.Count == kata.successfulStepsCount;

            _state.MovingState = MovingState.Unknown;   // that's for now, onComplete may set it to whatever appropriate

            LogInfo("KataRunner - calling onComplete()");
            onComplete();   // check kata.success, will be false if DriveStage.Cancel or other interruption occured.

            // done
            LogInfo("DriveBehaviorServiceBase: KataRunner(" + kata.name + ") finished" + currentCompass);
            yield break;
        }

        #endregion // Kata runner

    }
}
